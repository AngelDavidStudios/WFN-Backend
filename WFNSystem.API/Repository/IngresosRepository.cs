using Amazon.DynamoDBv2.DataModel;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;

namespace WFNSystem.API.Repository;

public class IngresosRepository: IRepository<Ingresos>
{
    private readonly IDynamoDBContext _context;
    
    public IngresosRepository(IDynamoDBContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Ingresos>> GetAllAsync()
    {
        var conditions = new List<ScanCondition>();
        var allIngresos = await _context.ScanAsync<Ingresos>(conditions).GetRemainingAsync();
        return allIngresos;
    }
    
    public async Task<Ingresos> GetByIdAsync(string id)
    {
        return await _context.LoadAsync<Ingresos>(id);
    }
    
    public async Task AddAsync(Ingresos ingreso)
    {
        ingreso.ID_Ingreso = Guid.NewGuid().ToString();
        ingreso.DateCreated = DateTime.UtcNow.ToString("yyyy-MM-dd");
        await _context.SaveAsync(ingreso);
    }
    
    public async Task UpdateAsync(string id, Ingresos ingreso)
    {
        var existingIngreso = await GetByIdAsync(id);
        if (existingIngreso == null)
        {
            throw new Exception("Ingreso not found");
        }
        
        ingreso.ID_Ingreso = id;
        await _context.SaveAsync(ingreso);
    }
    
    public async Task DeleteAsync(string id)
    {
        var ingreso = await GetByIdAsync(id);
        if (ingreso == null)
        {
            throw new Exception("Ingreso not found");
        }
        
        await _context.DeleteAsync<Ingresos>(id);
    }
}