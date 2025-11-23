using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;

namespace WFNSystem.API.Repository;

public class ParametroRepository: IParametroRepository
{
    private readonly IDynamoDBContext _context;
    
    public ParametroRepository(IDynamoDBContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Parametro>> GetAllAsync()
    {
        string pk = "PARAM#GLOBAL";

        var query = _context.QueryAsync<Parametro>(pk);
        return await query.GetRemainingAsync();
    }
    
    public async Task<Parametro?> GetByIdAsync(string parametroId)
    {
        string pk = "PARAM#GLOBAL";
        string sk = $"PARAM#{parametroId}";

        return await _context.LoadAsync<Parametro>(pk, sk);
    }
    
    public async Task<Parametro?> GetByNombreAsync(string tipoSnakeCase)
    {
        string pk = "PARAM#GLOBAL";

        var conditions = new List<ScanCondition>
        {
            new ScanCondition("Tipo", ScanOperator.Equal, tipoSnakeCase)
        };

        var result = await _context.ScanAsync<Parametro>(conditions).GetRemainingAsync();

        return result.FirstOrDefault();
    }
    
    public async Task AddAsync(Parametro parametro)
    {
        await _context.SaveAsync(parametro);
    }
    
    public async Task UpdateAsync(Parametro parametro)
    {
        await _context.SaveAsync(parametro);
    }
    
    public async Task DeleteAsync(string parametroId)
    {
        string pk = "PARAM#GLOBAL";
        string sk = $"PARAM#{parametroId}";

        await _context.DeleteAsync<Parametro>(pk, sk);
    }
}