using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Services;

public class ParametroService : IParametroService
{
    private readonly IParametroRepository _repo;

    public ParametroService(IParametroRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<Parametro>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<Parametro?> GetByIdAsync(string parametroId)
    {
        return await _repo.GetByIdAsync(parametroId);
    }

    public async Task<IEnumerable<Parametro>> GetByTipoAsync(string tipo)
    {
        tipo = (tipo ?? string.Empty).Trim().ToUpper().Replace(" ", "_");
        return await _repo.GetByTipoAsync(tipo);
    }

    public async Task<Parametro> CreateAsync(Parametro parametro)
    {
        parametro.ID_Parametro = Guid.NewGuid().ToString();

        parametro.PK = "PARAM#GLOBAL";
        parametro.SK = $"PARAM#{parametro.ID_Parametro}";

        parametro.Tipo = (parametro.Tipo ?? string.Empty)
            .Trim()
            .ToUpper()
            .Replace(" ", "_");

        parametro.DateCreated = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

        await _repo.AddAsync(parametro);
        return parametro;
    }

    public async Task<Parametro> UpdateAsync(Parametro parametro)
    {
        var exists = await _repo.GetByIdAsync(parametro.ID_Parametro);
        if (exists == null)
            throw new Exception("El parámetro que intenta actualizar no existe.");

        parametro.PK = "PARAM#GLOBAL";
        parametro.SK = $"PARAM#{parametro.ID_Parametro}";

        parametro.Tipo = (parametro.Tipo ?? string.Empty)
            .Trim()
            .ToUpper()
            .Replace(" ", "_");
        
        parametro.DateCreated = exists.DateCreated;

        await _repo.UpdateAsync(parametro);
        return parametro;
    }

    public async Task<bool> DeleteAsync(string parametroId)
    {
        var exists = await _repo.GetByIdAsync(parametroId);
        if (exists == null)
            return false;

        await _repo.DeleteAsync(parametroId);
        return true;
    }
}
