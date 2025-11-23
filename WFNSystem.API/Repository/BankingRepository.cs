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
    
    public async Task<IEnumerable<Banking>> GetBankingByEmpleadoIdAsync(string empleadoId)
    {
        string pk = $"EMP#{empleadoId}";
        return await _context.QueryAsync<Banking>(pk).GetRemainingAsync();
    }

    public async Task<Banking?> GetBankingByIdAsync(string empleadoId, string bankingId)
    {
        string pk = $"EMP#{empleadoId}";
        string sk = $"BANK#{bankingId}";
        return await _context.LoadAsync<Banking>(pk, sk);
    }

    public async Task AddBankingAsync(Banking banking)
    {
        await _context.SaveAsync(banking);
    }

    public async Task UpdateBankingAsync(Banking banking)
    {
        await _context.SaveAsync(banking);
    }

    public async Task DeleteBankingAsync(string empleadoId, string bankingId)
    {
        string pk = $"EMP#{empleadoId}";
        string sk = $"BANK#{bankingId}";
        await _context.DeleteAsync<Banking>(pk, sk);
    }
}