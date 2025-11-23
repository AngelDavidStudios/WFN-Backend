using Amazon.DynamoDBv2.DataModel;
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
    
    public async Task<IEnumerable<Novedad>> GetNovedadesByEmpleadoAsync(string empleadoId)
    {
        string pk = $"EMP#{empleadoId}";

        var results = await _context
            .QueryAsync<Novedad>(pk)
            .GetRemainingAsync();

        return results.Where(n => n.SK.StartsWith("NOV#"));
    }

    public async Task<IEnumerable<Novedad>> GetNovedadesByPeriodoAsync(string empleadoId, string periodo)
    {
        string pk = $"EMP#{empleadoId}";

        var results = await _context
            .QueryAsync<Novedad>(pk)
            .GetRemainingAsync();

        return results.Where(n => n.FechaIngresada.StartsWith(periodo));
    }

    public async Task<Novedad?> GetByIdAsync(string empleadoId, string novedadId)
    {
        string pk = $"EMP#{empleadoId}";
        string sk = $"NOV#{novedadId}";

        return await _context.LoadAsync<Novedad>(pk, sk);
    }

    public async Task AddAsync(Novedad novedad)
    {
        novedad.PK = $"EMP#{novedad.ID_Empleado}";
        novedad.SK = $"NOV#{novedad.ID_Novedad}";

        await _context.SaveAsync(novedad);
    }

    public async Task UpdateAsync(Novedad novedad)
    {
        novedad.PK = $"EMP#{novedad.ID_Empleado}";
        novedad.SK = $"NOV#{novedad.ID_Novedad}";

        await _context.SaveAsync(novedad);
    }

    public async Task DeleteAsync(string empleadoId, string novedadId)
    {
        string pk = $"EMP#{empleadoId}";
        string sk = $"NOV#{novedadId}";

        await _context.DeleteAsync<Novedad>(pk, sk);
    }
}