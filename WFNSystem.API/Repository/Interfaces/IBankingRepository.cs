using WFNSystem.API.Models;

namespace WFNSystem.API.Repository.Interfaces;

public interface IBankingRepository
{
    Task<IEnumerable<Banking>> GetByEmpleadoAsync(string empleadoId);
    Task<Banking?> GetByIdAsync(string empleadoId, string bankingId);

    Task AddAsync(Banking banking);
    Task UpdateAsync(Banking banking);
    Task DeleteAsync(string empleadoId, string bankingId);
}