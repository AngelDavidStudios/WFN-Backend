using WFNSystem.API.Models;

namespace WFNSystem.API.Services.Interfaces;

public interface IProvisionService
{
    Task<IEnumerable<Provision>> GetByEmpleadoAsync(string empleadoId);
    Task<IEnumerable<Provision>> GetByPeriodoAsync(string empleadoId, string periodo);
    Task<IEnumerable<Provision>> GetByTipoAsync(string empleadoId, string tipoProvision);

    Task<Provision?> GetByIdAsync(string empleadoId, string tipoProvision, string periodo);

    Task<Provision> CreateAsync(string empleadoId, Provision provision);
    Task<Provision> UpdateAsync(string empleadoId, Provision provision);
    Task<bool> DeleteAsync(string empleadoId, string tipoProvision, string periodo);

    // Procesar DECIMO 3/DECIMO 4/FONDOS cuando es anual
    Task ProcesarProvisionesAsync(string empleadoId, string periodo);
}