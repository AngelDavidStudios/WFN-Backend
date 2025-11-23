using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;
using WFNSystem.API.Services.Interfaces;
using WFNSystem.API.Services.Strategies.Interfaces;

namespace WFNSystem.API.Services;

public class NominaService: INominaService
{
    private readonly IEmpleadoRepository _empleadoRepo;
    private readonly INovedadRepository _novedadRepo;
    private readonly IParametroRepository _parametroRepo;
    private readonly IWorkspaceRepository _workspaceRepo;
    private readonly INominaRepository _nominaRepo;
    private readonly IStrategyFactory _strategyFactory;
    

    public NominaService(
        IEmpleadoRepository empleadoRepo,
        INovedadRepository novedadRepo,
        IParametroRepository parametroRepo,
        IWorkspaceRepository workspaceRepo,
        INominaRepository nominaRepo,
        IStrategyFactory strategyFactory)
    {
        _empleadoRepo = empleadoRepo;
        _novedadRepo = novedadRepo;
        _parametroRepo = parametroRepo;
        _workspaceRepo = workspaceRepo;
        _nominaRepo = nominaRepo;
        _strategyFactory = strategyFactory;
    }
    
    // =============================================================
    // GETTERS
    // =============================================================
    public async Task<Nomina?> GetNominaAsync(string empleadoId, string periodo)
    {
        return await _nominaRepo.GetAsync(periodo, empleadoId);
    }

    public async Task<IEnumerable<Nomina>> GetNominasByEmpleadoAsync(string empleadoId)
    {
        return await _nominaRepo.GetByEmpleadoAsync(empleadoId);
    }

    public async Task<IEnumerable<Nomina>> GetNominasByPeriodoAsync(string periodo)
    {
        return await _nominaRepo.GetByPeriodoAsync(periodo);
    }
    
    // =============================================================
    // GENERAR NÓMINA
    // =============================================================
    public async Task<Nomina> GenerarNominaAsync(string empleadoId, string periodo)
    {
        return await ProcesarNominaInterno(empleadoId, periodo);
    }


    // =============================================================
    // RECALCULAR NÓMINA
    // =============================================================
    public async Task<Nomina> RecalcularNominaAsync(string empleadoId, string periodo)
    {
        await DeleteNominaAsync(empleadoId, periodo);
        return await GenerarNominaAsync(empleadoId, periodo);
    }
    
    // =============================================================
    // DELETE
    // =============================================================
    public async Task<bool> DeleteNominaAsync(string empleadoId, string periodo)
    {
        var existing = await _nominaRepo.GetAsync(periodo, empleadoId);
        if (existing == null)
            return false;

        await _nominaRepo.DeleteAsync(periodo, empleadoId);
        return true;
    }
    
    // ========================================================================
    // MOTOR INTERNO DE NÓMINA
    // ========================================================================
    public async Task<Nomina> ProcesarNominaInterno(string empleadoId, string periodo)
    {
        // 1. Validar employee
        var empleado = await _empleadoRepo.GetByIdAsync(empleadoId);
        if (empleado == null)
            throw new Exception("Empleado no encontrado.");

        // 2. Validar workspace
        var workspace = await _workspaceRepo.GetByPeriodoAsync(periodo);
        if (workspace == null)
            throw new Exception("El período no existe.");

        if (workspace.Estado == 1)
            throw new Exception("El período está cerrado. No puede generar nómina.");

        // 3. Obtener novedades del período
        var novedades = await _novedadRepo.GetNovedadesByPeriodoAsync(empleadoId, periodo);
        List<Ingresos> ingresosList = new();
        List<Egresos> egresosList = new();

        decimal totalIngresos = 0;
        decimal totalIngresosGravados = 0;
        decimal totalOtrosIngresos = 0;
        decimal totalEgresos = 0;
        
        // =====================================================
        // 4. CÁLCULO DE INGRESOS (con Strategy Pattern)
        // =====================================================
        foreach (var nov in novedades)
        {
            var parametro = await _parametroRepo.GetByIdAsync(nov.ID_Parametro);

            if (parametro.Tipo == "INGRESO")
            {
                var strategy = _strategyFactory.GetIngresoStrategy(parametro.Descripcion);
                decimal valor = strategy.Calcular(nov, empleado);

                bool esGravado = nov.Is_Gravable == true;

                if (esGravado)
                {
                    totalIngresosGravados += valor;
                }
                else
                {
                    totalOtrosIngresos += valor;
                }

                ingresosList.Add(new Ingresos
                {
                    ID_Ingreso = Guid.NewGuid().ToString(),
                    Novedades = new List<Novedad> { nov },
                    SubTotal_Gravado_IESS = esGravado ? valor : 0,
                    SubTotal_No_Gravado_IESS = !esGravado ? valor : 0,
                    TotalIngresos = valor,
                    DateCreated = DateTime.UtcNow.ToString("yyyy-MM-dd")
                });
            }
        }

        totalIngresos = totalIngresosGravados + totalOtrosIngresos;
        
        // =====================================================
        // 5. CÁLCULO AUTOMÁTICO DE IESS PERSONAL (9.45%)
        // =====================================================
        decimal iessPersonal = Math.Round(totalIngresosGravados * 0.0945m, 2);
        totalEgresos += iessPersonal;

        egresosList.Add(new Egresos
        {
            ID_Egreso = Guid.NewGuid().ToString(),
            Novedades = new List<Novedad>(),
            TotalEgresos = iessPersonal,
            DateCreated = DateTime.UtcNow.ToString("yyyy-MM-dd")
        });

        // =====================================================
        // 6. CÁLCULO DE EGRESOS (Strategy Pattern)
        // =====================================================
        foreach (var nov in novedades)
        {
            var parametro = await _parametroRepo.GetByIdAsync(nov.ID_Parametro);

            if (parametro.Tipo == "EGRESO")
            {
                var strategy = _strategyFactory.GetEgresoStrategy(parametro.Descripcion);
                decimal valor = strategy.Calcular(nov, empleado, totalIngresosGravados);

                totalEgresos += valor;

                egresosList.Add(new Egresos
                {
                    ID_Egreso = Guid.NewGuid().ToString(),
                    Novedades = new List<Novedad> { nov },
                    TotalEgresos = valor,
                    DateCreated = DateTime.UtcNow.ToString("yyyy-MM-dd")
                });
            }
        }
        
        // =====================================================
        // 7. NETO A PAGAR
        // =====================================================
        decimal netoAPagar = totalIngresos - totalEgresos;

        // 8. CREAR OBJETO NOMINA
        // =============================================================
        var nomina = new Nomina
        {
            ID_Nomina = Guid.NewGuid().ToString(),
            ID_Empleado = empleadoId,
            Periodo = periodo,
            Ingresos = ingresosList,
            Egresos = egresosList,
            TotalIngresos = totalIngresos,
            TotalEgresos = totalEgresos,
            NetoAPagar = netoAPagar,
            DateCreated = DateTime.UtcNow.ToString("yyyy-MM-dd")
        };


        // =============================================================
        // 9. GUARDAR EN DYNAMODB (NO EN WORKSPACE)
        // =============================================================
        await _nominaRepo.AddAsync(nomina);

        return nomina;
    }
}