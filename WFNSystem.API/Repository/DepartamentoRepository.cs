using Amazon.DynamoDBv2.DataModel;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;

namespace WFNSystem.API.Repository;

public class DepartamentoRepository: IRepository<Departamento>
{
    private readonly IDynamoDBContext _context;
    
    public DepartamentoRepository(IDynamoDBContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Departamento>> GetAllAsync()
    {
        var conditions = new List<ScanCondition>();
        var allDepartamentos = await _context.ScanAsync<Departamento>(conditions).GetRemainingAsync();
        return allDepartamentos;
    }
    
    public async Task<Departamento> GetByIdAsync(string id)
    {
        return await _context.LoadAsync<Departamento>(id);
    }
    
    public async Task AddAsync(Departamento departamento)
    {
        departamento.ID_Departamento = Guid.NewGuid().ToString();
        await _context.SaveAsync(departamento);
    }
    
    public async Task UpdateAsync(string id, Departamento departamento)
    {
        var existingDepartamento = await GetByIdAsync(id);
        if (existingDepartamento == null)
        {
            throw new Exception("Departamento not found");
        }
        
        departamento.ID_Departamento = id;
        await _context.SaveAsync(departamento);
    }
    
    public async Task DeleteAsync(string id)
    {
        var departamento = await GetByIdAsync(id);
        if (departamento == null)
        {
            throw new Exception("Departamento not found");
        }
        
        await _context.DeleteAsync<Departamento>(id);
    }
}