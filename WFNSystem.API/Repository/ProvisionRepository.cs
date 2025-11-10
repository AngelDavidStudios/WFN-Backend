using Amazon.DynamoDBv2.DataModel;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;

namespace WFNSystem.API.Repository;

public class ProvisionRepository: IRepository<Provision>
{
    private readonly IDynamoDBContext _context;
    
    public ProvisionRepository(IDynamoDBContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Provision>> GetAllAsync()
    {
        var conditions = new List<ScanCondition>();
        var allProvisions = await _context.ScanAsync<Provision>(conditions).GetRemainingAsync();
        return allProvisions;
    }
    
    public async Task<Provision> GetByIdAsync(string id)
    {
        return await _context.LoadAsync<Provision>(id);
    }
    
    public async Task AddAsync(Provision provision)
    {
        provision.ID_Provision = Guid.NewGuid().ToString();
        await _context.SaveAsync(provision);
    }
    
    public async Task UpdateAsync(string id, Provision provision)
    {
        var existingProvision = await GetByIdAsync(id);
        if (existingProvision == null)
        {
            throw new Exception("Provision not found");
        }
        
        provision.ID_Provision = id;
        await _context.SaveAsync(provision);
    }
    
    public async Task DeleteAsync(string id)
    {
        var provision = await GetByIdAsync(id);
        if (provision == null)
        {
            throw new Exception("Provision not found");
        }
        
        await _context.DeleteAsync<Provision>(id);
    }
}