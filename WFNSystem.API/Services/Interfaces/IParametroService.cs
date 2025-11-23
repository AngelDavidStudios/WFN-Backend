using WFNSystem.API.Models;

namespace WFNSystem.API.Services.Interfaces;

public interface IParametroService
{
    Task<IEnumerable<Parametro>> GetAllAsync();
    Task<Parametro?> GetByIdAsync(string parametroId);
    Task<IEnumerable<Parametro>> GetByTipoAsync(string tipo);

    Task<Parametro> CreateAsync(Parametro parametro);
    Task<Parametro> UpdateAsync(Parametro parametro);
    Task<bool> DeleteAsync(string parametroId);   
}