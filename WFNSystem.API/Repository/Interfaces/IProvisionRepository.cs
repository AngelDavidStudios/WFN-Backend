using WFNSystem.API.Models;

namespace WFNSystem.API.Repository.Interfaces;

public interface IProvisionRepository: IRepository<Provision>
{
    Task<IEnumerable<Provision>> GetByTipoAsync(string tipo);
    Task<IEnumerable<Provision>> GetByEmpleadoAsync(string empleadoId);
}