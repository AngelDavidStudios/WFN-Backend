using WFNSystem.API.Models;

namespace WFNSystem.API.Services.Interfaces;

public interface IProvisionService
{
    Task<IEnumerable<Provision>> GetByEmpleadoAsync(string empleadoId);
    Task<IEnumerable<Provision>> GetByPeriodoAsync(string empleadoId, string periodo);

    Task<Provision?> GetByIdAsync(string empleadoId, string provisionId);

    Task<Provision> CreateAsync(Provision provision);
    Task<Provision> UpdateAsync(Provision provision);
    Task<bool> DeleteAsync(string empleadoId, string provisionId);

    // Proceso que actualiza:
    // ValorMensual, Acumulado, Total y DetalleCalculo
    Task ProcesarProvisionesAsync(string empleadoId, string periodo);
}