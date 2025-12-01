using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;

namespace WFNSystem.API.Repository;

public class DepartamentoRepository: IDepartamentoRepository
{
    private readonly IDynamoDBContext _context;
    
    public DepartamentoRepository(IDynamoDBContext context)
    {
        _context = context;
    }
    
    private string BuildPK(string departamentoId) => $"DEP#{departamentoId}";
    private const string SK = "META#DEP";

    public async Task<Departamento?> GetByIdAsync(string departamentoId)
    {
        return await _context.LoadAsync<Departamento>(BuildPK(departamentoId), SK);
    }

    public async Task<IEnumerable<Departamento>> GetAllAsync()
    {
        var conditions = new List<ScanCondition>
        {
            new ScanCondition("PK", ScanOperator.BeginsWith, "DEP#"),
            new ScanCondition("SK", ScanOperator.Equal, SK) // Solo registros META#DEP
        };

        return await _context.ScanAsync<Departamento>(conditions).GetRemainingAsync();
    }

    public async Task AddAsync(Departamento departamento)
    {
        departamento.PK = BuildPK(departamento.ID_Departamento);
        departamento.SK = SK;

        await _context.SaveAsync(departamento);
    }

    public async Task UpdateAsync(Departamento departamento)
    {
        departamento.PK = BuildPK(departamento.ID_Departamento);
        departamento.SK = SK;

        await _context.SaveAsync(departamento);
    }

    public async Task DeleteAsync(string departamentoId)
    {
        await _context.DeleteAsync<Departamento>(BuildPK(departamentoId), SK);
    }
}