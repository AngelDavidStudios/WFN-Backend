using Amazon.DynamoDBv2.DataModel;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;

namespace WFNSystem.API.Repository;

public class BankingRepository: IRepository<Banking>
{
    private readonly IDynamoDBContext _context;
    
    public BankingRepository(IDynamoDBContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Banking>> GetAllAsync()
    {
        var conditions = new List<ScanCondition>();
        var allBankings = await _context.ScanAsync<Banking>(conditions).GetRemainingAsync();
        return allBankings;
    }
    
    public async Task<Banking> GetByIdAsync(string id)
    {
        return await _context.LoadAsync<Banking>(id);
    }
    
    public async Task AddAsync(Banking banking)
    {
        banking.ID_Banking = Guid.NewGuid().ToString();
        await _context.SaveAsync(banking);
    }
    
    public async Task UpdateAsync(string id, Banking banking)
    {
        var existingBanking = await GetByIdAsync(id);
        if (existingBanking == null)
        {
            throw new Exception("Banking not found");
        }
        
        banking.ID_Banking = id;
        await _context.SaveAsync(banking);
    }
    
    public async Task DeleteAsync(string id)
    {
        var banking = await GetByIdAsync(id);
        if (banking == null)
        {
            throw new Exception("Banking not found");
        }
        
        await _context.DeleteAsync<Banking>(id);
    }
}