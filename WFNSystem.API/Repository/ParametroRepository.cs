using Amazon.DynamoDBv2.DataModel;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;

namespace WFNSystem.API.Repository;

public class ParametroRepository: IRepository<Parametro>
{
    private readonly IDynamoDBContext _context;
    
    public ParametroRepository(IDynamoDBContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Parametro>> GetAllAsync()
    {
        var conditions = new List<ScanCondition>();
        var allParametros = await _context.ScanAsync<Parametro>(conditions).GetRemainingAsync();
        return allParametros;
    }
    
    public async Task<Parametro> GetByIdAsync(string id)
    {
        return await _context.LoadAsync<Parametro>(id);
    }
    
    public async Task AddAsync(Parametro parametro)
    {
        parametro.ID_Parametro = Guid.NewGuid().ToString();
        parametro.DateCreated = DateTime.UtcNow.ToString("yyyy-MM-dd");
        await _context.SaveAsync(parametro);
    }
    
    public async Task UpdateAsync(string id, Parametro parametro)
    {
        var existingParametro = await GetByIdAsync(id);
        if (existingParametro == null)
        {
            throw new Exception("Parametro not found");
        }
        
        parametro.ID_Parametro = id;
        await _context.SaveAsync(parametro);
    }
    
    public async Task DeleteAsync(string id)
    {
        var parametro = await GetByIdAsync(id);
        if (parametro == null)
        {
            throw new Exception("Parametro not found");
        }
        
        await _context.DeleteAsync<Parametro>(id);
    }
}