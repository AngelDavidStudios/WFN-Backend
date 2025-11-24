using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;

namespace WFNSystem.API.Repository;

public class NovedadRepository: INovedadRepository
{
    private readonly IDynamoDBContext _context;
    
    public NovedadRepository(IDynamoDBContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Novedad>> GetByEmpleadoAsync(string empleadoId)
    {
        string pk = $"EMP#{empleadoId}";

        var result = await _context.QueryAsync<Novedad>(pk).GetRemainingAsync();
        return result;
    }
    
    public async Task<IEnumerable<Novedad>> GetByPeriodoAsync(string empleadoId, string periodo)
    {
        string pk = $"EMP#{empleadoId}";
        string skPrefix = $"NOV#{periodo}#";

        var query = _context.QueryAsync<Novedad>(pk, QueryOperator.BeginsWith, new[] { skPrefix });

        return await query.GetRemainingAsync();
    }
    
    public async Task<Novedad?> GetByIdAsync(string empleadoId, string novedadId, string periodo)
    {
        string pk = $"EMP#{empleadoId}";
        string sk = $"NOV#{periodo}#{novedadId}";

        return await _context.LoadAsync<Novedad>(pk, sk);
    }
    
    public async Task AddAsync(Novedad novedad)
    {
        await _context.SaveAsync(novedad);
    }

    public async Task UpdateAsync(Novedad novedad)
    {
        await _context.SaveAsync(novedad);
    }
    
    public async Task DeleteAsync(string empleadoId, string periodo, string novedadId)
    {
        string pk = $"EMP#{empleadoId}";
        string sk = $"NOV#{periodo}#{novedadId}";

        await _context.DeleteAsync<Novedad>(pk, sk);
    }
}