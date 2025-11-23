using WFNSystem.API.Models;

namespace WFNSystem.API.Repository.Interfaces;

public interface IPersonaRepository
{
    Task<IEnumerable<Persona>> GetAllAsync();
    Task<Persona?> GetByIdAsync(string personaId);
    Task AddAsync(Persona persona);
    Task UpdateAsync(Persona persona);
    Task DeleteAsync(string personaId);
}