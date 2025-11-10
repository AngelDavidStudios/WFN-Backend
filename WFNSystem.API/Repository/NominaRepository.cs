using Amazon.DynamoDBv2.DataModel;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;

namespace WFNSystem.API.Repository;

public class NominaRepository: IRepository<Nomina>
{
    private readonly IDynamoDBContext _context;
    
    public NominaRepository(IDynamoDBContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Nomina>> GetAllAsync()
    {
        var conditions = new List<ScanCondition>();
        var allNominas = await _context.ScanAsync<Nomina>(conditions).GetRemainingAsync();
        return allNominas;
    }
    
    public async Task<Nomina> GetByIdAsync(string id)
    {
        return await _context.LoadAsync<Nomina>(id);
    }
    
    public async Task AddAsync(Nomina nomina)
    {
        nomina.ID_Nomina = Guid.NewGuid().ToString();
        await _context.SaveAsync(nomina);
    }
    
    public async Task UpdateAsync(string id, Nomina nomina)
    {
        var existingNomina = await GetByIdAsync(id);
        if (existingNomina == null)
        {
            throw new Exception("Nomina not found");
        }
        
        nomina.ID_Nomina = id;
        await _context.SaveAsync(nomina);
    }
    
    public async Task DeleteAsync(string id)
    {
        var nomina = await GetByIdAsync(id);
        if (nomina == null)
        {
            throw new Exception("Nomina not found");
        }
        
        await _context.DeleteAsync<Nomina>(id);
    }
}