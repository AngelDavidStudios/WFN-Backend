using WFNSystem.API.Models;

namespace WFNSystem.API.Repository.Interfaces;

public interface IParametroRepository
{
    Task<IEnumerable<Parametro>> GetAllAsync();
    Task<Parametro?> GetByIdAsync(string parametroId);
    Task AddAsync(Parametro parametro);
    Task UpdateAsync(Parametro parametro);
    Task DeleteAsync(string parametroId);

    // Obtener parámetros por tipo (Ingreso, Egreso, Provisión)
    Task<IEnumerable<Parametro>> GetByTipoAsync(string tipo);
}