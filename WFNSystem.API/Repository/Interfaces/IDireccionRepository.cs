using WFNSystem.API.Models;

namespace WFNSystem.API.Repository.Interfaces;

public interface IDireccionRepository
{
    Task<IEnumerable<Direccion>> GetDireccionesByPersonaIdAsync(string personaId);
    Task<Direccion> GetDireccionByIdAsync(string personaId, string direccionId);
    Task AddDireccionAsync(string personaId, Direccion direccion);
    Task UpdateDireccionAsync(string personaId, string direccionId, Direccion direccion);
    Task DeleteDireccionAsync(string personaId, string direccionId);
}