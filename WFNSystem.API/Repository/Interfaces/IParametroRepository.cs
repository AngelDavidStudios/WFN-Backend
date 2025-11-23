using WFNSystem.API.Models;

namespace WFNSystem.API.Repository.Interfaces;

public interface IParametroRepository
{
    Task<IEnumerable<Parametro>> GetAllAsync();
    Task<Parametro?> GetByIdAsync(string parametroId);
    Task<IEnumerable<Parametro>> GetByTipoAsync(string tipoSnakeCase);

    Task AddAsync(Parametro parametro);
    Task UpdateAsync(Parametro parametro);
    Task DeleteAsync(string parametroId);
}