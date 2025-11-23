using WFNSystem.API.Models;

namespace WFNSystem.API.Repository.Interfaces;

public interface IProvisionRepository
{
    Task<IEnumerable<Provision>> GetByEmpleadoAsync(string empleadoId);
    Task<IEnumerable<Provision>> GetByPeriodoAsync(string empleadoId, string periodo);
    Task<IEnumerable<Provision>> GetByTipoAsync(string empleadoId, string tipoProvision);

    Task<Provision?> GetByIdAsync(string empleadoId, string tipoProvision, string periodo);

    Task AddAsync(Provision provision);
    Task UpdateAsync(Provision provision);
    Task DeleteAsync(string empleadoId, string tipoProvision, string periodo);
}