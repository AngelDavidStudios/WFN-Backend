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
    
    public async Task<Nomina?> GetNominaAsync(string empleadoId, string periodo)
    {
        string pk = $"EMP#{empleadoId}";
        string sk = $"NOM#{periodo}";

        return await _context.LoadAsync<Nomina>(pk, sk);
    }

    public async Task<IEnumerable<Nomina>> GetNominasByEmpleadoAsync(string empleadoId)
    {
        string pk = $"EMP#{empleadoId}";

        var results = await _context
            .QueryAsync<Nomina>(pk)
            .GetRemainingAsync();

        return results.Where(n => n.SK.StartsWith("NOM#"));
    }

    public async Task<IEnumerable<Nomina>> GetNominasByPeriodoAsync(string periodo)
    {
        // Para obtener todas las nóminas de un mismo periodo,
        // se debe hacer SCAN porque PK varía por empleado.
        var conditions = new List<ScanCondition>
        {
            new ScanCondition("SK", ScanOperator.Equal, $"NOM#{periodo}")
        };

        return await _context.ScanAsync<Nomina>(conditions).GetRemainingAsync();
    }

    public async Task AddAsync(Nomina nomina)
    {
        nomina.PK = $"EMP#{nomina.ID_Empleado}";
        nomina.SK = $"NOM#{nomina.Periodo}";

        await _context.SaveAsync(nomina);
    }

    public async Task UpdateAsync(Nomina nomina)
    {
        nomina.PK = $"EMP#{nomina.ID_Empleado}";
        nomina.SK = $"NOM#{nomina.Periodo}";

        await _context.SaveAsync(nomina);
    }

    public async Task DeleteAsync(string empleadoId, string periodo)
    {
        string pk = $"EMP#{empleadoId}";
        string sk = $"NOM#{periodo}";

        await _context.DeleteAsync<Nomina>(pk, sk);
    }
}