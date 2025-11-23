using WFNSystem.API.Models;

namespace WFNSystem.API.Services.Interfaces;

public interface IBankingService
{
    Task<IEnumerable<Banking>> GetByEmpleadoAsync(string empleadoId);
    Task<Banking?> GetByIdAsync(string empleadoId, string bankingId);

    Task<Banking> CreateAsync(string empleadoId, Banking banking);
    Task<Banking> UpdateAsync(string empleadoId, Banking banking);
    Task<bool> DeleteAsync(string empleadoId, string bankingId);
}