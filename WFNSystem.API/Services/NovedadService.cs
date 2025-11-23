using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Services;

public class NovedadService: INovedadService
{
    private readonly INovedadRepository _novedadRepo;
    private readonly IEmpleadoRepository _empleadoRepo;
    private readonly IParametroRepository _parametroRepo;

    public NovedadService(
        INovedadRepository novedadRepo,
        IEmpleadoRepository empleadoRepo,
        IParametroRepository parametroRepo)
    {
        _novedadRepo = novedadRepo;
        _empleadoRepo = empleadoRepo;
        _parametroRepo = parametroRepo;
    }

    public async Task<IEnumerable<Novedad>> GetByEmpleadoAsync(string empleadoId)
    {
        return await _novedadRepo.GetNovedadesByEmpleadoAsync(empleadoId);
    }

    public async Task<IEnumerable<Novedad>> GetByPeriodoAsync(string empleadoId, string periodo)
    {
        return await _novedadRepo.GetNovedadesByPeriodoAsync(empleadoId, periodo);
    }

    public async Task<Novedad?> GetByIdAsync(string empleadoId, string novedadId)
    {
        return await _novedadRepo.GetByIdAsync(empleadoId, novedadId);
    }

    public async Task<Novedad> CreateAsync(string empleadoId, Novedad novedad)
    {
        // 1. Validar existencia del empleado
        var empleado = await _empleadoRepo.GetByIdAsync(empleadoId);
        if (empleado == null)
            throw new ArgumentException("El empleado asociado a la novedad no existe.");

        // 2. Validar existencia del parámetro vinculado
        var parametro = await _parametroRepo.GetByIdAsync(novedad.ID_Parametro);
        if (parametro == null)
            throw new ArgumentException("El parámetro asociado a la novedad no existe.");

        // 3. Validaciones propias del dominio
        if (novedad.MontoAplicado < 0)
            throw new ArgumentException("El monto aplicado no puede ser negativo.");

        if (string.IsNullOrWhiteSpace(novedad.FechaIngresada))
            throw new ArgumentException("La fecha ingresada es obligatoria.");

        if (string.IsNullOrWhiteSpace(novedad.TipoNovedad))
            throw new ArgumentException("El tipo de novedad es obligatorio.");

        // 4. Generar ID de la novedad
        novedad.ID_Novedad = Guid.NewGuid().ToString();

        // 5. Asignar PK/SK
        novedad.PK = $"EMP#{empleadoId}";
        novedad.SK = $"NOV#{novedad.ID_Novedad}";

        // 6. Guardar
        await _novedadRepo.AddAsync(novedad);
        return novedad;
    }

    public async Task<Novedad> UpdateAsync(string empleadoId, Novedad novedad)
    {
        // 1. Validar existencia del empleado
        var empleado = await _empleadoRepo.GetByIdAsync(empleadoId);
        if (empleado == null)
            throw new ArgumentException("El empleado no existe.");

        // 2. Validar existencia de la novedad
        var existing = await _novedadRepo.GetByIdAsync(empleadoId, novedad.ID_Novedad);
        if (existing == null)
            throw new KeyNotFoundException("La novedad no existe.");

        // 3. Validar parámetro (en caso de que se cambie)
        var parametro = await _parametroRepo.GetByIdAsync(novedad.ID_Parametro);
        if (parametro == null)
            throw new ArgumentException("El parámetro asociado a la novedad no existe.");

        // 4. Asignar PK/SK correctos
        novedad.PK = $"EMP#{empleadoId}";
        novedad.SK = $"NOV#{novedad.ID_Novedad}";

        await _novedadRepo.UpdateAsync(novedad);
        return novedad;
    }

    public async Task<bool> DeleteAsync(string empleadoId, string novedadId)
    {
        var existing = await _novedadRepo.GetByIdAsync(empleadoId, novedadId);
        if (existing == null)
            return false;

        await _novedadRepo.DeleteAsync(empleadoId, novedadId);
        return true;
    }
}