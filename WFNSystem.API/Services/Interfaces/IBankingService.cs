using WFNSystem.API.Models;

namespace WFNSystem.API.Services.Interfaces;

public interface IBankingService
{
    Task<IEnumerable<Banking>> GetByPersonaAsync(string personaId);
    Task<Banking?> GetByIdAsync(string personaId, string bankingId);

    Task<Banking> CreateAsync(string personaId, Banking banking);
    Task<Banking> UpdateAsync(string personaId, Banking banking);
    Task<bool> DeleteAsync(string personaId, string bankingId);
}