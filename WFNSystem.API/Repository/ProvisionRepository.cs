using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
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
    
    public async Task<IEnumerable<Provision>> GetByEmpleadoAsync(string empleadoId)
    {
        string pk = $"EMP#{empleadoId}";

        var query = _context.QueryAsync<Provision>(pk);
        return await query.GetRemainingAsync();
    }
    
    public async Task<IEnumerable<Provision>> GetByPeriodoAsync(string empleadoId, string periodo)
    {
        string pk = $"EMP#{empleadoId}";
        
        var search = _context.QueryAsync<Provision>(pk, QueryOperator.BeginsWith, new[] { "PROV#" });
        var items = await search.GetRemainingAsync();

        return items.Where(x => x.SK is not null && x.SK.EndsWith($"#{periodo}"));
    }

    // Todas las provisiones por tipo (DECIMO_TERCERO, VACACIONES, etc.)
    public async Task<IEnumerable<Provision>> GetByTipoAsync(string empleadoId, string tipoProvision)
    {
        string pk = $"EMP#{empleadoId}";
        string skPrefix = $"PROV#{tipoProvision}";

        var query = _context.QueryAsync<Provision>(pk, QueryOperator.BeginsWith, new[] { skPrefix });

        return await query.GetRemainingAsync();
    }
    
    public async Task<Provision?> GetByIdAsync(string empleadoId, string tipoProvision, string periodo)
    {
        string pk = $"EMP#{empleadoId}";
        string sk = $"PROV#{tipoProvision}#{periodo}";

        return await _context.LoadAsync<Provision>(pk, sk);
    }
    
    public async Task AddAsync(Provision provision)
    {
        await _context.SaveAsync(provision);
    }

    public async Task UpdateAsync(Provision provision)
    {
        await _context.SaveAsync(provision);
    }

    public async Task DeleteAsync(string empleadoId, string tipoProvision, string periodo)
    {
        string pk = $"EMP#{empleadoId}";
        string sk = $"PROV#{tipoProvision}#{periodo}";

        await _context.DeleteAsync<Provision>(pk, sk);
    }
}