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
    
    // ======================================================
    // OBTENER UNA NÓMINA ESPECÍFICA (PERIODO + EMPLEADO)
    // ======================================================
    public async Task<Nomina?> GetAsync(string periodo, string empleadoId)
    {
        string pk = $"NOMINA#{periodo}";
        string sk = $"EMPLEADO#{empleadoId}";

        return await _context.LoadAsync<Nomina>(pk, sk);
    }
    
    // ======================================================
    // OBTENER TODAS LAS NÓMINAS DEL PERÍODO
    // ======================================================
    public async Task<IEnumerable<Nomina>> GetByPeriodoAsync(string periodo)
    {
        string pk = $"NOMINA#{periodo}";

        return await _context.QueryAsync<Nomina>(pk).GetRemainingAsync();
    }
    
    // ======================================================
    // OBTENER TODAS LAS NÓMINAS DE UN EMPLEADO (USANDO GSI)
    // Requiere un GSI con:
    // PartitionKey = SK  |  SortKey = PK
    // ======================================================
    public async Task<IEnumerable<Nomina>> GetByEmpleadoAsync(string empleadoId)
    {
        string sk = $"EMPLEADO#{empleadoId}";

        var config = new DynamoDBOperationConfig
        {
            IndexName = "GSI_SK" // <- Debe existir en DynamoDB
        };

        return await _context.QueryAsync<Nomina>(sk, config).GetRemainingAsync();
    }
    
    // ======================================================
    // CREAR NUEVA NÓMINA
    // ======================================================
    public async Task AddAsync(Nomina nomina)
    {
        // PK
        nomina.PK = $"NOMINA#{nomina.Periodo}";
        // SK
        nomina.SK = $"EMPLEADO#{nomina.ID_Empleado}";

        nomina.DateCreated = DateTime.UtcNow.ToString("yyyy-MM-dd");

        await _context.SaveAsync(nomina);
    }

    // ======================================================
    // UPDATE (OVERWRITE)
    // ======================================================
    public async Task UpdateAsync(Nomina nomina)
    {
        // En DynamoDB SaveAsync actúa como insert/update.
        await _context.SaveAsync(nomina);
    }

    // ======================================================
    // DELETE
    // ======================================================
    public async Task DeleteAsync(string periodo, string empleadoId)
    {
        string pk = $"NOMINA#{periodo}";
        string sk = $"EMPLEADO#{empleadoId}";

        await _context.DeleteAsync<Nomina>(pk, sk);
    }
}