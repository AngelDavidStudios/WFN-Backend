using Amazon.DynamoDBv2.DataModel;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;

namespace WFNSystem.API.Repository;

public class EgresosRepository: IRepository<Egresos>
{
    private readonly IDynamoDBContext _context;
    
    public EgresosRepository(IDynamoDBContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Egresos>> GetAllAsync()
    {
        var conditions = new List<ScanCondition>();
        var allEgresos = await _context.ScanAsync<Egresos>(conditions).GetRemainingAsync();
        return allEgresos;
    }
    
    public async Task<Egresos> GetByIdAsync(string id)
    {
        return await _context.LoadAsync<Egresos>(id);
    }
    
    public async Task AddAsync(Egresos egreso)
    {
        egreso.ID_Egreso = Guid.NewGuid().ToString();
        egreso.DateCreated = DateTime.UtcNow.ToString("yyyy-MM-dd");
        await _context.SaveAsync(egreso);
    }
    
    public async Task UpdateAsync(string id, Egresos egreso)
    {
        var existingEgreso = await GetByIdAsync(id);
        if (existingEgreso == null)
        {
            throw new Exception("Egreso not found");
        }
        
        egreso.ID_Egreso = id;
        await _context.SaveAsync(egreso);
    }
    
    public async Task DeleteAsync(string id)
    {
        var egreso = await GetByIdAsync(id);
        if (egreso == null)
        {
            throw new Exception("Egreso not found");
        }
        
        await _context.DeleteAsync<Egresos>(id);
    }
}