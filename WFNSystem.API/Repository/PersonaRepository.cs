using Amazon.DynamoDBv2.DataModel;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;

namespace WFNSystem.API.Repository;

public class PersonaRepository: IRepository<Persona>
{
    private readonly IDynamoDBContext _context;
    
    public PersonaRepository(IDynamoDBContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Persona>> GetAllAsync()
    {
        var conditions = new List<ScanCondition>();
        var allPersonas = await _context.ScanAsync<Persona>(conditions).GetRemainingAsync();
        return allPersonas;
    }
    
    public async Task<Persona> GetByIdAsync(string id)
    {
        return await _context.LoadAsync<Persona>(id);
    }
    
    public async Task AddAsync(Persona persona)
    {
        persona.ID_Persona = Guid.NewGuid().ToString();
        persona.DateCreated = DateTime.UtcNow.ToString("yyyy-MM-dd");
        await _context.SaveAsync(persona);
    }
    
    public async Task UpdateAsync(string id, Persona persona)
    {
        var existingPersona = await GetByIdAsync(id);
        if (existingPersona == null)
        {
            throw new Exception("Persona not found");
        }
        
        persona.ID_Persona = id;
        await _context.SaveAsync(persona);
    }
    
    public async Task DeleteAsync(string id)
    {
        var persona = await GetByIdAsync(id);
        if (persona == null)
        {
            throw new Exception("Persona not found");
        }
        
        await _context.DeleteAsync<Persona>(id);
    }
}