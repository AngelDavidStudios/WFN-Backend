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
        // Normalizar tipo a SNAKE_CASE antes de buscar
        tipo = NormalizarTipo(tipo);
        return await _repo.GetByTipoAsync(tipo);
    }

    public async Task<Parametro> CreateAsync(Parametro parametro)
    {
        // Validaciones de negocio
        ValidarParametro(parametro);

        parametro.ID_Parametro = Guid.NewGuid().ToString();

        parametro.PK = "PARAM#GLOBAL";
        parametro.SK = $"PARAM#{parametro.ID_Parametro}";

        // Normalizar nombre a SNAKE_CASE
        parametro.Nombre = NormalizarNombre(parametro.Nombre);

        // Normalizar tipo a SNAKE_CASE
        parametro.Tipo = NormalizarTipo(parametro.Tipo);

        // Normalizar tipo de cálculo
        parametro.TipoCalculo = NormalizarTipo(parametro.TipoCalculo);

        parametro.DateCreated = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

        await _repo.AddAsync(parametro);
        return parametro;
    }

    public async Task<Parametro> UpdateAsync(Parametro parametro)
    {
        var exists = await _repo.GetByIdAsync(parametro.ID_Parametro);
        if (exists == null)
            throw new Exception("El parámetro que intenta actualizar no existe.");

        // Validaciones de negocio
        ValidarParametro(parametro);

        parametro.PK = "PARAM#GLOBAL";
        parametro.SK = $"PARAM#{parametro.ID_Parametro}";

        // Normalizar nombre a SNAKE_CASE
        parametro.Nombre = NormalizarNombre(parametro.Nombre);

        // Normalizar tipo a SNAKE_CASE
        parametro.Tipo = NormalizarTipo(parametro.Tipo);

        // Normalizar tipo de cálculo
        parametro.TipoCalculo = NormalizarTipo(parametro.TipoCalculo);
        
        // Preservar fecha de creación
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

    // ============================================================
    // MÉTODOS PRIVADOS DE VALIDACIÓN Y NORMALIZACIÓN
    // ============================================================

    private void ValidarParametro(Parametro parametro)
    {
        // Validar nombre
        if (string.IsNullOrWhiteSpace(parametro.Nombre))
            throw new ArgumentException("El nombre del parámetro es requerido.");

        if (parametro.Nombre.Length < 3)
            throw new ArgumentException("El nombre del parámetro debe tener al menos 3 caracteres.");

        // Validar tipo
        if (string.IsNullOrWhiteSpace(parametro.Tipo))
            throw new ArgumentException("El tipo del parámetro es requerido.");

        var tipoNormalizado = parametro.Tipo.Trim().ToUpper();
        if (tipoNormalizado != "INGRESO" && tipoNormalizado != "EGRESO" && tipoNormalizado != "PROVISION")
            throw new ArgumentException("El tipo debe ser 'INGRESO', 'EGRESO' o 'PROVISION'.");

        // Validar tipo de cálculo
        if (string.IsNullOrWhiteSpace(parametro.TipoCalculo))
            throw new ArgumentException("El tipo de cálculo es requerido.");

        if (parametro.TipoCalculo.Length < 3)
            throw new ArgumentException("El tipo de cálculo debe tener al menos 3 caracteres.");
    }

    private string NormalizarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return string.Empty;

        return nombre.Trim().ToUpper().Replace(" ", "_");
    }

    private string NormalizarTipo(string tipo)
    {
        if (string.IsNullOrWhiteSpace(tipo))
            return string.Empty;

        return tipo.Trim().ToUpper().Replace(" ", "_");
    }
}
