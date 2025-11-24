using Amazon.DynamoDBv2.DataModel;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;

namespace WFNSystem.API.Repository;

public class BankingRepository: IBankingRepository
{
    private readonly IDynamoDBContext _context;
    
    public BankingRepository(IDynamoDBContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Banking>> GetByPersonaAsync(string personaId)
    {
        string pk = $"PERSONA#{personaId}";
        var query = _context.QueryAsync<Banking>(pk);
        return await query.GetRemainingAsync();
    }

    public async Task<Banking?> GetByIdAsync(string personaId, string bankingId)
    {
        string pk = $"PERSONA#{personaId}";
        string sk = $"BANK#{bankingId}";

        return await _context.LoadAsync<Banking>(pk, sk);
    }

    public async Task AddAsync(Banking banking)
    {
        await _context.SaveAsync(banking);
    }

    public async Task UpdateAsync(Banking banking)
    {
        await _context.SaveAsync(banking);
    }

    public async Task DeleteAsync(string personaId, string bankingId)
    {
        string pk = $"PERSONA#{personaId}";
        string sk = $"BANK#{bankingId}";

        await _context.DeleteAsync<Banking>(pk, sk);
    }
}