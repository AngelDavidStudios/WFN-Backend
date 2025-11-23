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
    
    public async Task<IEnumerable<Departamento>> GetAllAsync()
    {
        var conditions = new List<ScanCondition>
        {
            new ScanCondition("SK", ScanOperator.Equal, "META#DEP")
        };

        return await _context.ScanAsync<Departamento>(conditions).GetRemainingAsync();
    }

    public async Task<Departamento?> GetByIdAsync(string deptoId)
    {
        string pk = $"DEP#{deptoId}";
        string sk = "META#DEP";

        return await _context.LoadAsync<Departamento>(pk, sk);
    }

    public async Task AddAsync(Departamento depto)
    {
        depto.PK = $"DEP#{depto.ID_Departamento}";
        depto.SK = "META#DEP";

        await _context.SaveAsync(depto);
    }

    public async Task UpdateAsync(Departamento depto)
    {
        depto.PK = $"DEP#{depto.ID_Departamento}";
        depto.SK = "META#DEP";

        await _context.SaveAsync(depto);
    }

    public async Task DeleteAsync(string deptoId)
    {
        string pk = $"DEP#{deptoId}";
        string sk = "META#DEP";

        await _context.DeleteAsync<Departamento>(pk, sk);
    }
}