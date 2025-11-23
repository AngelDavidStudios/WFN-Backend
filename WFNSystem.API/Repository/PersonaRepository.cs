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
    
    private string BuildPK(string personaId) => $"PERSONA#{personaId}";
    private const string SK = "META#PERSONA";

    public async Task<Persona?> GetByIdAsync(string personaId)
    {
        return await _context.LoadAsync<Persona>(BuildPK(personaId), SK);
    }

    public async Task<IEnumerable<Persona>> GetAllAsync()
    {
        var conditions = new List<ScanCondition>
        {
            new ScanCondition("PK", ScanOperator.BeginsWith, "PERSONA#")
        };

        return await _context.ScanAsync<Persona>(conditions).GetRemainingAsync();
    }

    public async Task AddAsync(Persona persona)
    {
        persona.PK = BuildPK(persona.ID_Persona);
        persona.SK = SK;

        await _context.SaveAsync(persona);
    }

    public async Task UpdateAsync(Persona persona)
    {
        persona.PK = BuildPK(persona.ID_Persona);
        persona.SK = SK;

        await _context.SaveAsync(persona);
    }

    public async Task DeleteAsync(string personaId)
    {
        await _context.DeleteAsync<Persona>(BuildPK(personaId), SK);
    }
}