using WFNSystem.API.Models;

namespace WFNSystem.API.Repository.Interfaces;

public interface IParametroRepository: IRepository<Parametro>
{
    Task<Parametro?> GetByCodigoAsync(string IdParametro);
    Task<IEnumerable<Parametro>> GetAllByTipoAsync(string Tipo);
}