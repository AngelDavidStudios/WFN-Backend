using Amazon.DynamoDBv2.DataModel;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;

namespace WFNSystem.API.Repository;

public class NovedadRepository: IRepository<Novedad>
{
    private readonly IDynamoDBContext _context;
    
    public NovedadRepository(IDynamoDBContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Novedad>> GetAllAsync()
    {
        var conditions = new List<ScanCondition>();
        var allNovedades = await _context.ScanAsync<Novedad>(conditions).GetRemainingAsync();
        return allNovedades;
    }
    
    public async Task<Novedad> GetByIdAsync(string id)
    {
        return await _context.LoadAsync<Novedad>(id);
    }
    
    public async Task AddAsync(Novedad novedad)
    {
        novedad.ID_Novedad = Guid.NewGuid().ToString();
        await _context.SaveAsync(novedad);
    }
    
    public async Task UpdateAsync(string id, Novedad novedad)
    {
        var existingNovedad = await GetByIdAsync(id);
        if (existingNovedad == null)
        {
            throw new Exception("Novedad not found");
        }
        
        novedad.ID_Novedad = id;
        await _context.SaveAsync(novedad);
    }
    
    public async Task DeleteAsync(string id)
    {
        var novedad = await GetByIdAsync(id);
        if (novedad == null)
        {
            throw new Exception("Novedad not found");
        }
        
        await _context.DeleteAsync<Novedad>(id);
    }
}