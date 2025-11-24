using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;

namespace WFNSystem.API.Repository;

public class EmpleadoRepository: IEmpleadoRepository
{
    private readonly IDynamoDBContext _context;
    
    public EmpleadoRepository(IDynamoDBContext context)
    {
        _context = context;
    }
    
    private string BuildPK(string empleadoId) => $"EMP#{empleadoId}";
    private const string SK = "META#EMP";

    public async Task<Empleado?> GetByIdAsync(string empleadoId)
    {
        return await _context.LoadAsync<Empleado>(BuildPK(empleadoId), SK);
    }

    public async Task<IEnumerable<Empleado>> GetAllAsync()
    {
        var conditions = new List<ScanCondition>
        {
            new ScanCondition("PK", ScanOperator.BeginsWith, "EMP#")
        };

        return await _context.ScanAsync<Empleado>(conditions).GetRemainingAsync();
    }

    public async Task<IEnumerable<Empleado>> GetByDepartamentoAsync(string departamentoId)
    {
        var allEmployees = await GetAllAsync();
        return allEmployees.Where(e => e.ID_Departamento == departamentoId);
    }

    public async Task AddAsync(Empleado empleado)
    {
        empleado.PK = BuildPK(empleado.ID_Empleado);
        empleado.SK = SK;

        await _context.SaveAsync(empleado);
    }

    public async Task UpdateAsync(Empleado empleado)
    {
        empleado.PK = BuildPK(empleado.ID_Empleado);
        empleado.SK = SK;

        await _context.SaveAsync(empleado);
    }

    public async Task DeleteAsync(string empleadoId)
    {
        await _context.DeleteAsync<Empleado>(BuildPK(empleadoId), SK);
    }
}