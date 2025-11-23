using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using WFNSystem.API.Models;

namespace WFNSystem.API.Repository.Interfaces;

public class EmpleadoRepository: IEmpleadoRepository
{
    private readonly IDynamoDBContext _context;
    
    public EmpleadoRepository(IDynamoDBContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Empleado>> GetAllAsync()
    {
        var conditions = new List<ScanCondition>
        {
            new ScanCondition("SK", ScanOperator.Equal, "META#EMP")
        };

        return await _context.ScanAsync<Empleado>(conditions).GetRemainingAsync();
    }
    
    public async Task<Empleado?> GetByIdAsync(string empleadoId)
    {
        string pk = $"EMP#{empleadoId}";
        string sk = "META#EMP";
        return await _context.LoadAsync<Empleado>(pk, sk);
    }
    
    public async Task AddAsync(Empleado empleado)
    {
        empleado.PK = $"EMP#{empleado.ID_Empleado}";
        empleado.SK = "META#EMP";
        await _context.SaveAsync(empleado);
    }
    
    public async Task UpdateAsync(Empleado empleado)
    {
        empleado.PK = $"EMP#{empleado.ID_Empleado}";
        empleado.SK = "META#EMP";
        await _context.SaveAsync(empleado);
    }
    
    public async Task DeleteAsync(string empleadoId)
    {
        string pk = $"EMP#{empleadoId}";
        string sk = "META#EMP";
        await _context.DeleteAsync<Empleado>(pk, sk);
    }

    public async Task<object?> GetEmpleadoFullAsync(string empleadoId)
    {
        string pk = $"EMP#{empleadoId}";
        var results = await _context.QueryAsync<object>(pk).GetRemainingAsync();
        return results;
    }
}