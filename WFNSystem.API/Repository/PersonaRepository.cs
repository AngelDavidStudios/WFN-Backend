using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;

namespace WFNSystem.API.Repository;

public class PersonaRepository: IPersonaRepository
{
    private readonly IDynamoDBContext _context;
    
    public PersonaRepository(IDynamoDBContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Persona>> GetAllAsync()
    {
        var conditions = new List<ScanCondition>
        {
            new ScanCondition("SK", ScanOperator.Equal, "META#PERSONA")
        };

        return await _context.ScanAsync<Persona>(conditions).GetRemainingAsync();
    }
    
    public async Task<Persona?> GetByIdAsync(string personaId)
    {
        string pk = $"PERSONA#{personaId}";
        string sk = "META#PERSONA";
        return await _context.LoadAsync<Persona>(pk, sk);
    }
    
    public async Task AddAsync(Persona persona)
    {
        persona.PK = $"PERSONA#{persona.ID_Persona}";
        persona.SK = "META#PERSONA";
        await _context.SaveAsync(persona);
    }
    
    public async Task UpdateAsync(Persona persona)
    {
        persona.PK = $"PERSONA#{persona.ID_Persona}";
        persona.SK = "META#PERSONA";
        await _context.SaveAsync(persona);
    }
    
    public async Task DeleteAsync(string personaId)
    {
        string pk = $"PERSONA#{personaId}";
        string sk = "META#PERSONA";
        await _context.DeleteAsync<Persona>(pk, sk);
    }
}