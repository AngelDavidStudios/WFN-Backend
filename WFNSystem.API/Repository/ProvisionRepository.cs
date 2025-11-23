using Amazon.DynamoDBv2.DataModel;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;

namespace WFNSystem.API.Repository;

public class ProvisionRepository: IProvisionRepository
{
    private readonly IDynamoDBContext _context;
    
    public ProvisionRepository(IDynamoDBContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Provision>> GetProvisionesByEmpleadoAsync(string empleadoId)
    {
        string pk = $"EMP#{empleadoId}";

        var results = await _context
            .QueryAsync<Provision>(pk)
            .GetRemainingAsync();

        return results.Where(p => p.SK.StartsWith("PROV#"));
    }

    public async Task<IEnumerable<Provision>> GetProvisionesByPeriodoAsync(string empleadoId, string periodo)
    {
        string pk = $"EMP#{empleadoId}";

        var results = await _context
            .QueryAsync<Provision>(pk)
            .GetRemainingAsync();

        return results.Where(p => p.Periodo == periodo);
    }

    public async Task<Provision?> GetByIdAsync(string empleadoId, string provisionId)
    {
        string pk = $"EMP#{empleadoId}";
        string sk = $"PROV#{provisionId}";

        return await _context.LoadAsync<Provision>(pk, sk);
    }

    public async Task AddAsync(Provision provision)
    {
        provision.PK = $"EMP#{provision.ID_Empleado}";
        provision.SK = $"PROV#{provision.ID_Provision}";

        await _context.SaveAsync(provision);
    }

    public async Task UpdateAsync(Provision provision)
    {
        provision.PK = $"EMP#{provision.ID_Empleado}";
        provision.SK = $"PROV#{provision.ID_Provision}";

        await _context.SaveAsync(provision);
    }

    public async Task DeleteAsync(string empleadoId, string provisionId)
    {
        string pk = $"EMP#{empleadoId}";
        string sk = $"PROV#{provisionId}";

        await _context.DeleteAsync<Provision>(pk, sk);
    }
}