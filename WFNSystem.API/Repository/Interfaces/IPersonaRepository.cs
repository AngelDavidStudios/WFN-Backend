using WFNSystem.API.Models;

namespace WFNSystem.API.Repository.Interfaces;

public interface IPersonaRepository
{
    Task<Persona?> GetByIdAsync(string personaId);
    Task<IEnumerable<Persona>> GetAllAsync();

    Task AddAsync(Persona persona);
    Task UpdateAsync(Persona persona);
    Task DeleteAsync(string personaId);
}