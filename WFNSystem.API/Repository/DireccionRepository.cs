using Amazon.DynamoDBv2.DataModel;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;

namespace WFNSystem.API.Repository;

public class DireccionRepository: IDireccionRepository
{
    private readonly IDynamoDBContext _context;
    
    public DireccionRepository(IDynamoDBContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Direccion>> GetDireccionesByPersonaIdAsync(string personaId)
    {
        var persona = await _context.LoadAsync<Models.Persona>(personaId);
        if (persona == null)
        {
            throw new Exception("Persona not found");
        }

        return persona.Direcciones ?? new List<Direccion>();
    }
    
    public async Task<Direccion> GetDireccionByIdAsync(string personaId, string direccionId)
    {
        var persona = await _context.LoadAsync<Models.Persona>(personaId);
        if (persona == null)
        {
            throw new Exception("Persona not found");
        }

        var direccion = persona.Direcciones?.FirstOrDefault(d => d.ID_Direccion == direccionId);
        if (direccion == null)
        {
            throw new Exception("Direccion not found");
        }

        return direccion;
    }
    
    public async Task AddDireccionAsync(string personaId, Direccion direccion)
    {
        var persona = await _context.LoadAsync<Models.Persona>(personaId);
        if (persona == null)
        {
            throw new Exception("Persona not found");
        }

        direccion.ID_Direccion = Guid.NewGuid().ToString();
        persona.Direcciones ??= new List<Direccion>();
        persona.Direcciones.Add(direccion);
        
        await _context.SaveAsync(persona);
    }
    
    public async Task UpdateDireccionAsync(string personaId, string direccionId, Direccion direccion)
    {
        var persona = await _context.LoadAsync<Models.Persona>(personaId);
        if (persona == null)
        {
            throw new Exception("Persona not found");
        }

        var existingDireccion = persona.Direcciones?.FirstOrDefault(d => d.ID_Direccion == direccionId);
        if (existingDireccion == null)
        {
            throw new Exception("Direccion not found");
        }

        direccion.ID_Direccion = direccionId;
        var index = persona.Direcciones.IndexOf(existingDireccion);
        persona.Direcciones[index] = direccion;
        
        await _context.SaveAsync(persona);
    }
    
    public async Task DeleteDireccionAsync(string personaId, string direccionId)
    {
        var persona = await _context.LoadAsync<Models.Persona>(personaId);
        if (persona == null)
        {
            throw new Exception("Persona not found");
        }

        var direccion = persona.Direcciones?.FirstOrDefault(d => d.ID_Direccion == direccionId);
        if (direccion == null)
        {
            throw new Exception("Direccion not found");
        }

        persona.Direcciones.Remove(direccion);
        
        await _context.SaveAsync(persona);
    }
}