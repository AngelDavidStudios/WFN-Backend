using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Services;

public class ParametroService: IParametroService
{
    private readonly IParametroRepository _parametroRepo;

    public ParametroService(IParametroRepository parametroRepo)
    {
        _parametroRepo = parametroRepo;
    }

    public async Task<IEnumerable<Parametro>> GetAllAsync()
    {
        return await _parametroRepo.GetAllAsync();
    }

    public async Task<Parametro?> GetByIdAsync(string parametroId)
    {
        return await _parametroRepo.GetByIdAsync(parametroId);
    }

    public async Task<IEnumerable<Parametro>> GetByTipoAsync(string tipo)
    {
        if (string.IsNullOrWhiteSpace(tipo))
            throw new ArgumentException("El tipo de parámetro no puede estar vacío.");

        return await _parametroRepo.GetByTipoAsync(tipo);
    }

    public async Task<Parametro> CreateAsync(Parametro parametro)
    {
        if (string.IsNullOrWhiteSpace(parametro.Tipo))
            throw new ArgumentException("El tipo del parámetro es obligatorio.");

        if (string.IsNullOrWhiteSpace(parametro.Descripcion))
            throw new ArgumentException("La descripción del parámetro es obligatoria.");

        // Generar ID
        parametro.ID_Parametro = Guid.NewGuid().ToString();

        // Fecha creada desde backend
        parametro.DateCreated = DateTime.UtcNow.ToString("yyyy-MM-dd");

        // PK/SK asignados para la tabla unificada
        parametro.PK = "PARAMETRO#GLOBAL";
        parametro.SK = $"PARAM#{parametro.ID_Parametro}";

        await _parametroRepo.AddAsync(parametro);
        return parametro;
    }

    public async Task<Parametro> UpdateAsync(Parametro parametro)
    {
        var existing = await _parametroRepo.GetByIdAsync(parametro.ID_Parametro);

        if (existing == null)
            throw new KeyNotFoundException("El parámetro no existe.");

        parametro.PK = "PARAMETRO#GLOBAL";
        parametro.SK = $"PARAM#{parametro.ID_Parametro}";

        await _parametroRepo.UpdateAsync(parametro);
        return parametro;
    }

    public async Task<bool> DeleteAsync(string parametroId)
    {
        var existing = await _parametroRepo.GetByIdAsync(parametroId);

        if (existing == null)
            return false;

        await _parametroRepo.DeleteAsync(parametroId);
        return true;
    }
}