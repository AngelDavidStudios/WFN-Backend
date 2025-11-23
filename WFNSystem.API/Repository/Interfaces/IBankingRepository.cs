using WFNSystem.API.Models;

namespace WFNSystem.API.Repository.Interfaces;

public interface IBankingRepository
{
    Task<IEnumerable<Banking>> GetBankingByEmpleadoIdAsync(string empleadoId);
    Task<Banking?> GetBankingByIdAsync(string empleadoId, string bankingId);

    Task AddBankingAsync(Banking banking);
    Task UpdateBankingAsync(Banking banking);
    Task DeleteBankingAsync(string empleadoId, string bankingId);
}