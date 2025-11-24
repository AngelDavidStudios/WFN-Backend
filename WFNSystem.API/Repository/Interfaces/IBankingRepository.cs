using WFNSystem.API.Models;

namespace WFNSystem.API.Repository.Interfaces;

public interface IBankingRepository
{
    Task<IEnumerable<Banking>> GetByPersonaAsync(string personaId);
    Task<Banking?> GetByIdAsync(string personaId, string bankingId);

    Task AddAsync(Banking banking);
    Task UpdateAsync(Banking banking);
    Task DeleteAsync(string personaId, string bankingId);
}