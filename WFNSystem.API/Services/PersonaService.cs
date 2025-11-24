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
        // Validaciones
        ValidarPersona(persona);

        // Crear ID
        persona.ID_Persona = Guid.NewGuid().ToString();

        // Construir claves
        persona.PK = $"PERSONA#{persona.ID_Persona}";
        persona.SK = "META#PERSONA";

        // Calcular edad automáticamente
        persona.Edad = CalcularEdad(persona.DateBirthday);

        // Fecha de creación
        persona.DateCreated = DateTime.UtcNow.ToString("yyyy-MM-dd");

        await _repo.AddAsync(persona);

        return persona;
    }

    public async Task<Persona> UpdateAsync(Persona persona)
    {
        // Validación: debe existir
        var exists = await _repo.GetByIdAsync(persona.ID_Persona);
        if (exists == null)
            throw new Exception("Persona no encontrada.");

        // Validaciones de negocio
        ValidarPersona(persona);

        // Mantener claves correctas
        persona.PK = $"PERSONA#{persona.ID_Persona}";
        persona.SK = "META#PERSONA";

        // Recalcular edad
        persona.Edad = CalcularEdad(persona.DateBirthday);

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

    // ============================================================
    // MÉTODOS PRIVADOS DE VALIDACIÓN Y CÁLCULO
    // ============================================================

    private void ValidarPersona(Persona persona)
    {
        // Validar DNI
        if (string.IsNullOrWhiteSpace(persona.DNI))
            throw new ArgumentException("El DNI es requerido.");

        if (persona.DNI.Length < 10)
            throw new ArgumentException("El DNI debe tener al menos 10 caracteres.");

        // Validar nombres
        if (string.IsNullOrWhiteSpace(persona.PrimerNombre))
            throw new ArgumentException("El primer nombre es requerido.");

        if (string.IsNullOrWhiteSpace(persona.ApellidoPaterno))
            throw new ArgumentException("El apellido paterno es requerido.");

        // Validar fecha de nacimiento
        if (persona.DateBirthday == default || persona.DateBirthday >= DateTime.UtcNow)
            throw new ArgumentException("La fecha de nacimiento es inválida.");

        // Validar género
        if (!string.IsNullOrWhiteSpace(persona.Gender))
        {
            var genderNormalized = persona.Gender.ToUpper();
            if (genderNormalized != "M" && genderNormalized != "F" && genderNormalized != "OTRO")
                throw new ArgumentException("El género debe ser 'M', 'F' o 'OTRO'.");
        }

        // Validar correos (formato básico)
        if (persona.Correo != null && persona.Correo.Any())
        {
            foreach (var correo in persona.Correo)
            {
                if (!correo.Contains("@") || !correo.Contains("."))
                    throw new ArgumentException($"El correo '{correo}' no es válido.");
            }
        }
    }

    private int CalcularEdad(DateTime fechaNacimiento)
    {
        var hoy = DateTime.UtcNow;
        var edad = hoy.Year - fechaNacimiento.Year;

        // Ajustar si aún no ha cumplido años este año
        if (fechaNacimiento.Date > hoy.AddYears(-edad))
            edad--;

        return edad;
    }
}
