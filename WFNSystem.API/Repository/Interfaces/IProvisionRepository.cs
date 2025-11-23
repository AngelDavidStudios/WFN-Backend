using WFNSystem.API.Models;

namespace WFNSystem.API.Repository.Interfaces;

public interface IProvisionRepository
{
    Task<IEnumerable<Provision>> GetProvisionesByEmpleadoAsync(string empleadoId);
    Task<IEnumerable<Provision>> GetProvisionesByPeriodoAsync(string empleadoId, string periodo);

    Task<Provision?> GetByIdAsync(string empleadoId, string provisionId);

    Task AddAsync(Provision provision);
    Task UpdateAsync(Provision provision);
    Task DeleteAsync(string empleadoId, string provisionId);
}