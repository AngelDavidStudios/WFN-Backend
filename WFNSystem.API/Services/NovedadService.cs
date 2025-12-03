using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Services;

public class NovedadService: INovedadService
{
    private readonly INovedadRepository _repo;
    private readonly IEmpleadoRepository _empleadoRepo;
    private readonly IParametroRepository _parametroRepo;
    private readonly INominaService _nominaService;
    private readonly ILogger<NovedadService> _logger;

    public NovedadService(
        INovedadRepository repo, 
        IEmpleadoRepository empleadoRepo,
        IParametroRepository parametroRepo,
        INominaService nominaService,
        ILogger<NovedadService> logger)
    {
        _repo = repo;
        _empleadoRepo = empleadoRepo;
        _parametroRepo = parametroRepo;
        _nominaService = nominaService;
        _logger = logger;
    }

    public async Task<IEnumerable<Novedad>> GetByEmpleadoAsync(string empleadoId)
    {
        return await _repo.GetByEmpleadoAsync(empleadoId);
    }

    public async Task<IEnumerable<Novedad>> GetByPeriodoAsync(string empleadoId, string periodo)
    {
        return await _repo.GetByPeriodoAsync(empleadoId, periodo);
    }

    public async Task<Novedad?> GetByIdAsync(string empleadoId, string periodo, string novedadId)
    {
        return await _repo.GetByIdAsync(empleadoId, novedadId, periodo);
    }

    public async Task<Novedad> CreateAsync(string empleadoId, Novedad novedad)
    {
        // Validar que el empleado exista
        var empleado = await _empleadoRepo.GetByIdAsync(empleadoId);
        if (empleado == null)
            throw new Exception("El empleado no existe.");

        // Validaciones de negocio
        ValidarNovedad(novedad);

        // Validar que el parámetro exista
        var parametro = await _parametroRepo.GetByIdAsync(novedad.ID_Parametro);
        if (parametro == null)
            throw new ArgumentException($"El parámetro '{novedad.ID_Parametro}' no existe.");

        // Validar coherencia entre TipoNovedad y Tipo de Parámetro
        if (!string.Equals(novedad.TipoNovedad, parametro.Tipo, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException($"El tipo de novedad '{novedad.TipoNovedad}' no coincide con el tipo del parámetro '{parametro.Tipo}'.");

        // Crear ID
        novedad.ID_Novedad = Guid.NewGuid().ToString();

        // Normalizar periodo (YYYY-MM)
        novedad.Periodo = NormalizarPeriodo(novedad.Periodo);

        // Construcción de claves
        novedad.PK = $"EMP#{empleadoId}";
        novedad.SK = $"NOV#{novedad.Periodo}#{novedad.ID_Novedad}";

        // Timestamp
        novedad.FechaIngresada = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

        // Normalizar tipo
        novedad.TipoNovedad = novedad.TipoNovedad.Trim().ToUpper();

        await _repo.AddAsync(novedad);

        // ============================================================
        // RECALCULAR NÓMINA AUTOMÁTICAMENTE
        // ============================================================
        try
        {
            _logger.LogInformation($"Recalculando nómina para empleado {empleadoId}, periodo {novedad.Periodo} después de crear novedad");
            await _nominaService.RecalcularNominaAsync(empleadoId, novedad.Periodo);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"No se pudo recalcular la nómina automáticamente. La novedad fue creada pero la nómina debe recalcularse manualmente.");
            // No lanzamos excepción para no fallar la creación de la novedad
        }

        return novedad;
    }

    public async Task<Novedad> UpdateAsync(string empleadoId, Novedad novedad)
    {
        if (string.IsNullOrWhiteSpace(novedad.ID_Novedad))
            throw new Exception("La novedad debe incluir ID_Novedad para actualizar.");

        if (string.IsNullOrWhiteSpace(novedad.Periodo))
            throw new Exception("La novedad debe incluir Periodo para actualizar.");

        // Normalizar periodo
        novedad.Periodo = NormalizarPeriodo(novedad.Periodo);

        var exists = await _repo.GetByIdAsync(empleadoId, novedad.ID_Novedad, novedad.Periodo);
        if (exists == null)
            throw new Exception("La novedad no existe.");

        // Validaciones de negocio
        ValidarNovedad(novedad);

        // Validar que el parámetro exista
        var parametro = await _parametroRepo.GetByIdAsync(novedad.ID_Parametro);
        if (parametro == null)
            throw new ArgumentException($"El parámetro '{novedad.ID_Parametro}' no existe.");

        // Validar coherencia entre TipoNovedad y Tipo de Parámetro
        if (!string.Equals(novedad.TipoNovedad, parametro.Tipo, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException($"El tipo de novedad '{novedad.TipoNovedad}' no coincide con el tipo del parámetro '{parametro.Tipo}'.");

        // Reconstruir PK & SK
        novedad.PK = $"EMP#{empleadoId}";
        novedad.SK = $"NOV#{novedad.Periodo}#{novedad.ID_Novedad}";

        // Normalizar tipo
        novedad.TipoNovedad = novedad.TipoNovedad.Trim().ToUpper();

        // Preservar fecha de ingreso original
        novedad.FechaIngresada = exists.FechaIngresada;

        await _repo.UpdateAsync(novedad);

        // ============================================================
        // RECALCULAR NÓMINA AUTOMÁTICAMENTE
        // ============================================================
        try
        {
            _logger.LogInformation($"Recalculando nómina para empleado {empleadoId}, periodo {novedad.Periodo} después de actualizar novedad");
            await _nominaService.RecalcularNominaAsync(empleadoId, novedad.Periodo);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"No se pudo recalcular la nómina automáticamente. La novedad fue actualizada pero la nómina debe recalcularse manualmente.");
            // No lanzamos excepción para no fallar la actualización de la novedad
        }

        return novedad;
    }

    public async Task<bool> DeleteAsync(string empleadoId, string novedadId, string periodo)
    {
        var exists = await _repo.GetByIdAsync(empleadoId, novedadId, periodo);
        if (exists == null)
            return false;

        await _repo.DeleteAsync(empleadoId, periodo, novedadId);

        // ============================================================
        // RECALCULAR NÓMINA AUTOMÁTICAMENTE
        // ============================================================
        try
        {
            _logger.LogInformation($"Recalculando nómina para empleado {empleadoId}, periodo {periodo} después de eliminar novedad");
            await _nominaService.RecalcularNominaAsync(empleadoId, periodo);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"No se pudo recalcular la nómina automáticamente. La novedad fue eliminada pero la nómina debe recalcularse manualmente.");
            // No lanzamos excepción para no fallar la eliminación de la novedad
        }

        return true;
    }

    // ============================================================
    // MÉTODOS PRIVADOS DE VALIDACIÓN
    // ============================================================

    private void ValidarNovedad(Novedad novedad)
    {
        // Validar periodo
        if (string.IsNullOrWhiteSpace(novedad.Periodo))
            throw new ArgumentException("El periodo es requerido.");

        // Validar formato de periodo (YYYY-MM)
        if (!System.Text.RegularExpressions.Regex.IsMatch(novedad.Periodo, @"^\d{4}-\d{2}$"))
            throw new ArgumentException("El periodo debe tener el formato YYYY-MM (ejemplo: 2025-11).");

        // Validar parámetro
        if (string.IsNullOrWhiteSpace(novedad.ID_Parametro))
            throw new ArgumentException("El ID del parámetro es requerido.");

        // Validar tipo de novedad
        if (string.IsNullOrWhiteSpace(novedad.TipoNovedad))
            throw new ArgumentException("El tipo de novedad es requerido.");

        var tipoNormalizado = novedad.TipoNovedad.Trim().ToUpper();
        if (tipoNormalizado != "INGRESO" && tipoNormalizado != "EGRESO")
            throw new ArgumentException("El tipo de novedad debe ser 'INGRESO' o 'EGRESO'.");

        // Validar monto aplicado
        if (novedad.MontoAplicado < 0)
            throw new ArgumentException("El monto aplicado no puede ser negativo.");
    }

    private string NormalizarPeriodo(string periodo)
    {
        if (string.IsNullOrWhiteSpace(periodo))
            throw new ArgumentException("El periodo es requerido.");

        // Normalizar formato: eliminar espacios, convertir a mayúsculas
        periodo = periodo.Trim().ToUpper();

        // Validar formato YYYY-MM
        if (!System.Text.RegularExpressions.Regex.IsMatch(periodo, @"^\d{4}-\d{2}$"))
            throw new ArgumentException("El periodo debe tener el formato YYYY-MM (ejemplo: 2025-11).");

        return periodo;
    }
}