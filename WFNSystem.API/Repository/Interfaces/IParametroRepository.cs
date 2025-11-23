using WFNSystem.API.Models;

namespace WFNSystem.API.Repository.Interfaces;

public interface IParametroRepository
{
    Task<Parametro?> GetByIdAsync(string parametroId);
    Task<Parametro?> GetByNombreAsync(string tipoSnakeCase);
    Task<IEnumerable<Parametro>> GetAllAsync();

    Task AddAsync(Parametro parametro);
    Task UpdateAsync(Parametro parametro);
    Task DeleteAsync(string parametroId);
}