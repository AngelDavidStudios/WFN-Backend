using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Services;

public class PersonaService: IPersonaService
{
    private readonly IPersonaRepository _repo;

    public PersonaService(IPersonaRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<Persona>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<Persona?> GetByIdAsync(string personaId)
    {
        return await _repo.GetByIdAsync(personaId);
    }

    public async Task<Persona> CreateAsync(Persona persona)
    {
        // Crear ID
        persona.ID_Persona = Guid.NewGuid().ToString();

        // Construir claves
        persona.PK = $"PERSONA#{persona.ID_Persona}";
        persona.SK = "META#PERSONA";

        // Fecha de creación
        persona.DateCreated = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

        await _repo.AddAsync(persona);

        return persona;
    }

    public async Task<Persona> UpdateAsync(Persona persona)
    {
        // Validación: debe existir
        var exists = await _repo.GetByIdAsync(persona.ID_Persona);
        if (exists == null)
            throw new Exception("Persona no encontrada.");

        // Mantener claves correctas
        persona.PK = $"PERSONA#{persona.ID_Persona}";
        persona.SK = "META#PERSONA";

        await _repo.UpdateAsync(persona);
        return persona;
    }

    public async Task<bool> DeleteAsync(string personaId)
    {
        var exists = await _repo.GetByIdAsync(personaId);
        if (exists == null)
            return false;

        await _repo.DeleteAsync(personaId);
        return true;
    }
}