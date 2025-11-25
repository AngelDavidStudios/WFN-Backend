using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;
using WFNSystem.API.Services.Interfaces;
using WFNSystem.API.Services.Strategies.Interfaces;

namespace WFNSystem.API.Services;

public class NominaService : INominaService
{
    private readonly INominaRepository _nominaRepo;
    private readonly INovedadRepository _novedadRepo;
    private readonly IEmpleadoRepository _empleadoRepo;
    private readonly IParametroRepository _parametroRepo;
    private readonly IProvisionRepository _provisionRepo;

    private readonly IProvisionService _provisionService;
    private readonly IStrategyFactory _strategyFactory;
    private readonly IWorkspaceService _workspaceService;
    private readonly ILogger<NominaService> _logger;

    public NominaService(
        INominaRepository nominaRepo,
        INovedadRepository novedadRepo,
        IEmpleadoRepository empleadoRepo,
        IParametroRepository parametroRepo,
        IProvisionRepository provisionRepo,
        IProvisionService provisionService,
        IStrategyFactory strategyFactory,
        IWorkspaceService workspaceService,
        ILogger<NominaService> logger)
    {
        _nominaRepo = nominaRepo;
        _novedadRepo = novedadRepo;
        _empleadoRepo = empleadoRepo;
        _parametroRepo = parametroRepo;
        _provisionRepo = provisionRepo;
        _provisionService = provisionService;
        _strategyFactory = strategyFactory;
        _workspaceService = workspaceService;
        _logger = logger;
    }

    public async Task<Nomina?> GetByPeriodoAsync(string empleadoId, string periodo)
    {
        if (string.IsNullOrWhiteSpace(empleadoId) || string.IsNullOrWhiteSpace(periodo))
            return null;

        return await _nominaRepo.GetByPeriodoAsync(empleadoId, periodo);
    }

    public async Task<IEnumerable<Nomina>> GetByEmpleadoAsync(string empleadoId)
    {
        if (string.IsNullOrWhiteSpace(empleadoId))
            return Enumerable.Empty<Nomina>();

        return await _nominaRepo.GetByEmpleadoAsync(empleadoId);
    }

    public async Task<IEnumerable<Nomina>> GetByPeriodoGlobalAsync(string periodo)
    {
        if (string.IsNullOrWhiteSpace(periodo))
            return Enumerable.Empty<Nomina>();

        return await _nominaRepo.GetByPeriodoGlobalAsync(periodo);
    }

    public async Task<bool> DeleteNominaAsync(string empleadoId, string periodo)
    {
        var exists = await _nominaRepo.GetByPeriodoAsync(empleadoId, periodo);
        if (exists == null)
            return false;

        await _nominaRepo.DeleteAsync(empleadoId, periodo);
        return true;
    }

    public async Task<Nomina> GenerarNominaAsync(string empleadoId, string periodo)
    {
        // ========== 1. Validar periodo abierto ==========
        var workspace = await _workspaceService.GetByPeriodoAsync(periodo);
        if (workspace == null || workspace.Estado != 0)
            throw new Exception($"El periodo {periodo} no está abierto.");

        // ========== 2. Cargar empleado ==========
        var empleado = await _empleadoRepo.GetByIdAsync(empleadoId);
        if (empleado == null)
            throw new Exception("El empleado no existe.");

        // ========== 3. Cargar novedades y parámetros ==========
        var novedades = (await _novedadRepo.GetByPeriodoAsync(empleadoId, periodo)).ToList();
        var parametros = (await _parametroRepo.GetAllAsync()).ToDictionary(p => p.ID_Parametro);

        // ========== 4. Crear nómina base ==========
        var nomina = new Nomina
        {
            PK = $"EMP#{empleadoId}",
            SK = $"NOM#{periodo}",
            ID_Nomina = Guid.NewGuid().ToString(),
            ID_Empleado = empleadoId,
            Periodo = periodo,
            FechaCalculo = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            IsCerrada = false
        };

        // ========== 5. Calcular ingresos ==========
        await CalcularIngresosAsync(nomina, empleado, novedades, parametros);

        // ========== 6. Calcular egresos ==========
        await CalcularEgresosAsync(nomina, empleado, novedades, parametros);

        // ========== 7. Procesar provisiones ==========
        await CalcularProvisionesAsync(nomina, empleado);

        // ========== 8. Calcular neto a pagar ==========
        CalcularNetoAPagar(nomina);

        // ========== 9. Guardar la nómina ==========
        await _nominaRepo.AddAsync(nomina);

        return nomina;
    }
    
    private async Task CalcularIngresosAsync(
        Nomina nomina,
        Empleado empleado,
        List<Novedad> novedades,
        Dictionary<string, Parametro> parametros)
    {
        // Inicializar con salario base como ingreso gravable
        decimal totalGravado = empleado.SalarioBase;
        decimal totalNoGravado = 0;

        _logger.LogInformation($"Iniciando cálculo de ingresos - Salario Base: {empleado.SalarioBase}");

        // ============================================================
        // PASO 1: Procesar NOVEDADES PRIMERO (horas extras, comisiones, etc.)
        // Para calcular el TOTAL INGRESOS GRAVADOS IESS correcto
        // ============================================================
        foreach (var novedad in novedades.Where(n => n.TipoNovedad == "INGRESO"))
        {
            if (!parametros.TryGetValue(novedad.ID_Parametro, out var parametro))
            {
                _logger.LogWarning($"Parámetro {novedad.ID_Parametro} no encontrado para novedad {novedad.ID_Novedad}");
                continue;
            }

            // Obtener estrategia de cálculo
            var strategy = _strategyFactory.GetIngresoStrategy(parametro.TipoCalculo);
            if (strategy == null)
            {
                _logger.LogWarning($"No existe estrategia para tipo de cálculo: {parametro.TipoCalculo}");
                continue;
            }

            // Calcular valor usando la estrategia
            var valor = strategy.Calcular(
                novedad,
                empleado,
                empleado.SalarioBase,
                totalGravado,  // Total acumulado hasta ahora
                new Dictionary<string, decimal> { { "DECIMO_CUARTO_BASE", 470m } }
            );

            _logger.LogInformation($"Novedad {parametro.Nombre}: {valor:F2} (Gravable: {novedad.Is_Gravable})");

            // Clasificar según si es gravable o no
            if (novedad.Is_Gravable)
                totalGravado += valor;
            else
                totalNoGravado += valor;
        }

        _logger.LogInformation($"Total Ingresos Gravados IESS (antes de décimos): {totalGravado:F2}");

        // ============================================================
        // PASO 2: Calcular DÉCIMOS MENSUALIZADOS sobre el total gravado
        // IMPORTANTE: Los décimos son INGRESOS NO GRAVABLES
        // ============================================================
        
        // Décimo Tercero Mensual - NO GRAVABLE
        if (empleado.Is_DecimoTercMensual)
        {
            // Se calcula sobre el TOTAL INGRESOS GRAVADOS IESS
            decimal decimoTerceroMensual = totalGravado / 12m;
            totalNoGravado += decimoTerceroMensual;  // ✅ NO GRAVABLE
            
            _logger.LogInformation($"Décimo Tercero Mensualizado (NO GRAVABLE): {decimoTerceroMensual:F2}");
        }
        
        // Décimo Cuarto Mensual - NO GRAVABLE
        if (empleado.Is_DecimoCuartoMensual)
        {
            // SBU actual Ecuador 2025
            decimal sbu = 470m;
            decimal decimoCuartoMensual = sbu / 12m;
            totalNoGravado += decimoCuartoMensual;  // ✅ NO GRAVABLE
            
            _logger.LogInformation($"Décimo Cuarto Mensualizado (NO GRAVABLE): {decimoCuartoMensual:F2}");
        }
        
        // Fondos de Reserva Mensual - NO GRAVABLE
        if (empleado.Is_FondoReserva)
        {
            // Se calcula sobre el TOTAL INGRESOS GRAVADOS IESS
            decimal fondosReservaMensual = totalGravado * 0.0833m;
            totalNoGravado += fondosReservaMensual;  // ✅ NO GRAVABLE
            
            _logger.LogInformation($"Fondos de Reserva Mensualizado (NO GRAVABLE): {fondosReservaMensual:F2}");
        }

        // ============================================================
        // PASO 3: TRANSFERIR PROVISIONES ACUMULADAS en mes correspondiente
        // ============================================================
        var partes = nomina.Periodo.Split('-');
        int mes = int.Parse(partes[1]);
        
        // DÉCIMO TERCERO: Se transfiere en NOVIEMBRE (mes 11)
        if (mes == 11 && !empleado.Is_DecimoTercMensual)
        {
            var provD13 = await ObtenerProvisionAcumuladaAsync(empleado.ID_Empleado, "DECIMO_TERCERO_ACUMULADO");
            if (provD13 != null && provD13.Acumulado > 0)
            {
                totalNoGravado += provD13.Acumulado;  // ✅ NO GRAVABLE
                
                // Marcar como transferida
                provD13.IsTransferred = true;
                provD13.Total = provD13.Acumulado;
                await _provisionRepo.UpdateAsync(provD13);
                
                _logger.LogInformation($"✅ Décimo Tercero Acumulado transferido al ROL (NO GRAVABLE): {provD13.Acumulado:F2}");
            }
        }
        
        // DÉCIMO CUARTO: Se transfiere en JULIO (mes 7)
        if (mes == 7 && !empleado.Is_DecimoCuartoMensual)
        {
            var provD14 = await ObtenerProvisionAcumuladaAsync(empleado.ID_Empleado, "DECIMO_CUARTO_ACUMULADO");
            if (provD14 != null && provD14.Acumulado > 0)
            {
                totalNoGravado += provD14.Acumulado;  // ✅ NO GRAVABLE
                
                // Marcar como transferida
                provD14.IsTransferred = true;
                provD14.Total = provD14.Acumulado;
                await _provisionRepo.UpdateAsync(provD14);
                
                _logger.LogInformation($"✅ Décimo Cuarto Acumulado transferido al ROL (NO GRAVABLE): {provD14.Acumulado:F2}");
            }
        }

        // Asignar totales a la nómina
        nomina.TotalIngresosGravados = Math.Round(totalGravado, 2);
        nomina.TotalIngresosNoGravados = Math.Round(totalNoGravado, 2);
        nomina.TotalIngresos = Math.Round(totalGravado + totalNoGravado, 2);

        _logger.LogInformation($"✅ Total Ingresos Gravados IESS: {nomina.TotalIngresosGravados:F2}");
        _logger.LogInformation($"✅ Total Ingresos No Gravados: {nomina.TotalIngresosNoGravados:F2}");
        _logger.LogInformation($"✅ Total Ingresos: {nomina.TotalIngresos:F2}");
    }
    
    private async Task CalcularEgresosAsync(
        Nomina nomina,
        Empleado empleado,
        List<Novedad> novedades,
        Dictionary<string, Parametro> parametros)
    {
        decimal totalEgresos = 0;
        decimal aporteIESS_Personal = 0;
        decimal impuestoRenta = 0;

        foreach (var novedad in novedades.Where(n => n.TipoNovedad == "EGRESO"))
        {
            if (!parametros.TryGetValue(novedad.ID_Parametro, out var parametro))
            {
                _logger.LogWarning($"Parámetro {novedad.ID_Parametro} no encontrado para la novedad {novedad.ID_Novedad}");
                continue;
            }

            var strategy = _strategyFactory.GetEgresoStrategy(parametro.Tipo);
            var monto = strategy.Calcular(
                novedad,
                empleado,
                empleado.SalarioBase,
                nomina.TotalIngresosGravados,
                new Dictionary<string, decimal>()
            );

            totalEgresos += monto;

            // ↳ Reglas especiales del modelo
            switch (parametro.Tipo)
            {
                case "IESS_PERSONAL":
                    aporteIESS_Personal = monto;
                    break;

                case "IMPUESTO_RENTA":
                    impuestoRenta = monto;
                    break;
            }
        }

        nomina.TotalEgresos = totalEgresos;
        nomina.IESS_AportePersonal = aporteIESS_Personal;
        nomina.IR_Retenido = impuestoRenta;
    }
    
    private async Task CalcularProvisionesAsync(Nomina nomina, Empleado empleado)
    {
        decimal totalGravados = nomina.TotalIngresosGravados;
        string empleadoId = nomina.ID_Empleado;
        string periodo = nomina.Periodo;

        // 1. Cargar provisiones registradas previamente
        var provisionesPrevias = await _provisionRepo.GetByEmpleadoAsync(empleadoId);
        var listaPrevias = provisionesPrevias.ToList();

        // 2. Lista final con valores generados para el período actual
        var nuevasProvisiones = new List<Provision>();

        // 3. Tipos soportados
        var tipos = new[]
        {
            "PROVISION_VACACIONES",
            "DECIMO_TERCERO_ACUMULADO",
            "DECIMO_CUARTO_ACUMULADO",
            "FONDOS_RESERVA_ACUMULADO",
            "IESS_PATRONAL"
        };

        foreach (var tipo in tipos)
        {
            Provision provisionAnterior = listaPrevias
                .Where(p => p.TipoProvision == tipo)
                .OrderByDescending(p => p.Periodo)
                .FirstOrDefault();

            decimal acumuladoPrevio = provisionAnterior?.Acumulado ?? 0;
            bool pagadoPrevio = provisionAnterior?.IsTransferred ?? false;

            // 4. Si se pagó la provisión anterior, reiniciar el acumulado
            if (pagadoPrevio)
            {
                acumuladoPrevio = 0;
            }

            // 5. Crear provisión temporal con el acumulado previo
            var provisionTemp = new Provision
            {
                TipoProvision = tipo,
                Acumulado = acumuladoPrevio
            };

            // 6. Calcular nuevo acumulado usando la estrategia
            var strategy = _strategyFactory.GetProvisionStrategy(tipo);
            decimal nuevoAcumulado = strategy.Calcular(
                empleado,
                empleado.SalarioBase,
                totalGravados,
                provisionTemp
            );

            // 7. El valor mensual es la diferencia entre el nuevo acumulado y el anterior
            decimal valorMensual = nuevoAcumulado - acumuladoPrevio;
            decimal acumuladoActual = nuevoAcumulado;

            // 8. Crear provisión actual
            var provision = new Provision
            {
                PK = $"EMP#{empleadoId}",
                SK = $"PROV#{tipo}#{periodo}",

                ID_Provision = Guid.NewGuid().ToString(),
                ID_Empleado = empleadoId,
                TipoProvision = tipo,
                Periodo = periodo,

                ValorMensual = valorMensual,
                Acumulado = acumuladoActual,

                Total = 0,
                IsTransferred = false,

                FechaCalculo = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

            nuevasProvisiones.Add(provision);
            await _provisionRepo.AddAsync(provision);
        }

        _logger.LogInformation($"Procesadas {nuevasProvisiones.Count} provisiones para EMP#{empleadoId}");
    }
    
    private void CalcularNetoAPagar(Nomina nomina)
    {
        var ingresos = nomina.TotalIngresos;
        var egresos = nomina.TotalEgresos;

        nomina.NetoAPagar = ingresos - egresos;

        _logger.LogInformation($"Neto a pagar calculado: {nomina.NetoAPagar}");
    }

    // ============================================================
    // MÉTODO AUXILIAR: Obtener provisión acumulada no transferida
    // ============================================================
    private async Task<Provision?> ObtenerProvisionAcumuladaAsync(string empleadoId, string tipo)
    {
        var provisiones = await _provisionRepo.GetByTipoAsync(empleadoId, tipo);
        
        return provisiones
            .Where(p => !p.IsTransferred)
            .OrderByDescending(p => p.Periodo)
            .FirstOrDefault();
    }
    
    private async Task GuardarNominaAsync(Nomina nomina)
    {
        _logger.LogInformation($"Guardando nómina para EMP#{nomina.ID_Empleado} periodo {nomina.Periodo}");

        // 1. Verificar si ya existe una nómina previa
        var existente = await _nominaRepo.GetByPeriodoAsync(nomina.ID_Empleado, nomina.Periodo);

        if (existente == null)
        {
            _logger.LogInformation("No existe nómina previa → realizando INSERT.");
            await _nominaRepo.AddAsync(nomina);
        }
        else
        {
            _logger.LogInformation("Existe nómina previa → realizando UPDATE.");
            await _nominaRepo.UpdateAsync(nomina);
        }

        _logger.LogInformation("Nómina guardada correctamente.");
    }
    
    public async Task<Nomina> RecalcularNominaAsync(string empleadoId, string periodo)
    {
        _logger.LogInformation($"Recalculando nómina para EMP#{empleadoId} periodo {periodo}");

        // 1. Cargar la nómina base (o generarla si no existe)
        var nomina = await _nominaRepo.GetByPeriodoAsync(empleadoId, periodo);

        if (nomina == null)
        {
            nomina = await GenerarNominaAsync(empleadoId, periodo);
        }

        // 2. Cargar datos necesarios
        var empleado = await _empleadoRepo.GetByIdAsync(empleadoId);
        if (empleado == null)
            throw new Exception($"Empleado {empleadoId} no encontrado");

        var novedades = (await _novedadRepo.GetByPeriodoAsync(empleadoId, periodo)).ToList();
        var parametros = (await _parametroRepo.GetAllAsync()).ToDictionary(p => p.ID_Parametro);

        // 3. Recalcular bloques
        await CalcularIngresosAsync(nomina, empleado, novedades, parametros);
        await CalcularEgresosAsync(nomina, empleado, novedades, parametros);
        await CalcularProvisionesAsync(nomina, empleado);
        CalcularNetoAPagar(nomina);

        // 4. Guardar
        await GuardarNominaAsync(nomina);

        return nomina;
    }
    
}