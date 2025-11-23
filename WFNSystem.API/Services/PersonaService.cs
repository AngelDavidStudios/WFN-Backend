using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Services;

public class PersonaService: IPersonaService
{
    private readonly IPersonaRepository _personaRepo;

    public PersonaService(IPersonaRepository personaRepo)
    {
        _personaRepo = personaRepo;
    }

    public async Task<IEnumerable<Persona>> GetAllAsync()
    {
        return await _personaRepo.GetAllAsync();
    }

    public async Task<Persona?> GetByIdAsync(string personaId)
    {
        return await _personaRepo.GetByIdAsync(personaId);
    }

    public async Task<Persona> CreateAsync(Persona persona)
    {
        // Validación simple, podrías expandirla después
        if (string.IsNullOrWhiteSpace(persona.DNI))
            throw new ArgumentException("El DNI no puede estar vacío.");

        if (string.IsNullOrWhiteSpace(persona.PrimerNombre))
            throw new ArgumentException("El primer nombre es obligatorio.");

        if (string.IsNullOrWhiteSpace(persona.ApellidoMaterno))
            throw new ArgumentException("El apellido materno es obligatorio.");

        // Generar ID
        persona.ID_Persona = Guid.NewGuid().ToString();

        // DateCreated desde backend
        persona.DateCreated = DateTime.UtcNow.ToString("yyyy-MM-dd");

        persona.PK = $"PERSONA#{persona.ID_Persona}";
        persona.SK = "META#PERSONA";

        await _personaRepo.AddAsync(persona);
        return persona;
    }

    public async Task<Persona> UpdateAsync(Persona persona)
    {
        var existing = await _personaRepo.GetByIdAsync(persona.ID_Persona);

        if (existing == null)
            throw new KeyNotFoundException("La persona no existe.");

        // Se podrían agregar validaciones adicionales

        persona.PK = $"PERSONA#{persona.ID_Persona}";
        persona.SK = "META#PERSONA";

        await _personaRepo.UpdateAsync(persona);
        return persona;
    }

    public async Task<bool> DeleteAsync(string personaId)
    {
        var existing = await _personaRepo.GetByIdAsync(personaId);

        if (existing == null)
            return false;

        await _personaRepo.DeleteAsync(personaId);
        return true;
    }
}