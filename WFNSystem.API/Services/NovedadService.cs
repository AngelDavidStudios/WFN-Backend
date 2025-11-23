using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Services;

public class NovedadService: INovedadService
{
    private readonly INovedadRepository _repo;

    public NovedadService(INovedadRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<Novedad>> GetByEmpleadoAsync(string empleadoId)
    {
        return await _repo.GetByEmpleadoAsync(empleadoId);
    }

    public async Task<IEnumerable<Novedad>> GetByPeriodoAsync(string empleadoId, string periodo)
    {
        return await _repo.GetByPeriodoAsync(empleadoId, periodo);
    }

    public async Task<Novedad?> GetByIdAsync(string empleadoId, string novedadId, string periodo)
    {
        return await _repo.GetByIdAsync(empleadoId, novedadId, periodo);
    }

    public async Task<Novedad> CreateAsync(string empleadoId, Novedad novedad)
    {
        // Crear ID
        novedad.ID_Novedad = Guid.NewGuid().ToString();

        // Normalizar periodo
        if (string.IsNullOrWhiteSpace(novedad.Periodo))
            throw new Exception("La novedad debe incluir el periodo.");

        string periodo = novedad.Periodo.Trim().ToUpper();

        // Construcción de claves
        novedad.PK = $"EMP#{empleadoId}";
        novedad.SK = $"NOV#{periodo}#{novedad.ID_Novedad}";

        // Timestamp
        novedad.FechaIngresada = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

        // Sanitizar tipos
        novedad.TipoNovedad = novedad.TipoNovedad.Trim().ToUpper().Replace(" ", "_");
        novedad.ID_Parametro = novedad.ID_Parametro.Trim();

        await _repo.AddAsync(novedad);
        return novedad;
    }

    public async Task<Novedad> UpdateAsync(string empleadoId, Novedad novedad)
    {
        if (string.IsNullOrWhiteSpace(novedad.ID_Novedad))
            throw new Exception("La novedad debe incluir ID_Novedad para actualizar.");

        if (string.IsNullOrWhiteSpace(novedad.Periodo))
            throw new Exception("La novedad debe incluir Periodo para actualizar.");

        string periodo = novedad.Periodo.Trim().ToUpper();

        var exists = await _repo.GetByIdAsync(empleadoId, novedad.ID_Novedad, periodo);
        if (exists == null)
            throw new Exception("La novedad no existe.");

        // Reconstruimos PK & SK
        novedad.PK = $"EMP#{empleadoId}";
        novedad.SK = $"NOV#{periodo}#{novedad.ID_Novedad}";

        // Normalizar tipo
        novedad.TipoNovedad = novedad.TipoNovedad.Trim().ToUpper().Replace(" ", "_");

        await _repo.UpdateAsync(novedad);
        return novedad;
    }

    public async Task<bool> DeleteAsync(string empleadoId, string novedadId, string periodo)
    {
        var exists = await _repo.GetByIdAsync(empleadoId, novedadId, periodo);
        if (exists == null)
            return false;

        await _repo.DeleteAsync(empleadoId, periodo, novedadId);
        return true;
    }
}