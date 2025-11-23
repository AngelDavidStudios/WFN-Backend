using WFNSystem.API.Models;

namespace WFNSystem.API.Services.Interfaces;

public interface IPersonaService
{
    Task<IEnumerable<Persona>> GetAllAsync();
    Task<Persona?> GetByIdAsync(string personaId);

    Task<Persona> CreateAsync(Persona persona);
    Task<Persona> UpdateAsync(Persona persona);
    Task<bool> DeleteAsync(string personaId);
}