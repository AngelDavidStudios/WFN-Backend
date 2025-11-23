using WFNSystem.API.Models;

namespace WFNSystem.API.Services.Interfaces;

public interface IBankingService
{
    Task<IEnumerable<Parametro>> GetAllAsync();
    Task<Parametro?> GetByIdAsync(string parametroId);
    Task<Parametro?> GetByNombreAsync(string tipoSnakeCase);

    Task<Parametro> CreateAsync(Parametro parametro);
    Task<Parametro> UpdateAsync(Parametro parametro);
    Task<bool> DeleteAsync(string parametroId);
}