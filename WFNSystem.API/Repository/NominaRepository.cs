using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;

namespace WFNSystem.API.Repository;

public class NominaRepository: INominaRepository
{
    private readonly IDynamoDBContext _context;
    
    public NominaRepository(IDynamoDBContext context)
    {
        _context = context;
    }
    
    public async Task<Nomina?> GetByPeriodoAsync(string empleadoId, string periodo)
    {
        string pk = $"EMP#{empleadoId}";
        string sk = $"NOMINA#{periodo}";

        return await _context.LoadAsync<Nomina>(pk, sk);
    }
    
    public async Task<IEnumerable<Nomina>> GetByEmpleadoAsync(string empleadoId)
    {
        string pk = $"EMP#{empleadoId}";

        var query = _context.QueryAsync<Nomina>(pk);
        return await query.GetRemainingAsync();
    }
    
    public async Task<IEnumerable<Nomina>> GetByPeriodoGlobalAsync(string periodo)
    {
        string skPrefix = $"NOMINA#{periodo}";

        var conditions = new List<ScanCondition>
        {
            new ScanCondition("SK", ScanOperator.BeginsWith, skPrefix)
        };

        return await _context.ScanAsync<Nomina>(conditions).GetRemainingAsync();
    }
    
    public async Task AddAsync(Nomina nomina)
    {
        await _context.SaveAsync(nomina);
    }

    public async Task UpdateAsync(Nomina nomina)
    {
        await _context.SaveAsync(nomina);
    }

    public async Task DeleteAsync(string empleadoId, string periodo)
    {
        string pk = $"EMP#{empleadoId}";
        string sk = $"NOMINA#{periodo}";

        await _context.DeleteAsync<Nomina>(pk, sk);
    }
}