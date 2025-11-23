using Amazon.DynamoDBv2.DataModel;
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
        string pk = "PARAMETRO#GLOBAL";

        return await _context
            .QueryAsync<Parametro>(pk)
            .GetRemainingAsync();
    }

    public async Task<Parametro?> GetByIdAsync(string parametroId)
    {
        string pk = "PARAMETRO#GLOBAL";
        string sk = $"PARAM#{parametroId}";

        return await _context.LoadAsync<Parametro>(pk, sk);
    }

    public async Task AddAsync(Parametro parametro)
    {
        parametro.PK = "PARAMETRO#GLOBAL";
        parametro.SK = $"PARAM#{parametro.ID_Parametro}";

        await _context.SaveAsync(parametro);
    }

    public async Task UpdateAsync(Parametro parametro)
    {
        parametro.PK = "PARAMETRO#GLOBAL";
        parametro.SK = $"PARAM#{parametro.ID_Parametro}";

        await _context.SaveAsync(parametro);
    }

    public async Task DeleteAsync(string parametroId)
    {
        string pk = "PARAMETRO#GLOBAL";
        string sk = $"PARAM#{parametroId}";

        await _context.DeleteAsync<Parametro>(pk, sk);
    }

    public async Task<IEnumerable<Parametro>> GetByTipoAsync(string tipo)
    {
        string pk = "PARAMETRO#GLOBAL";

        var results = await _context.QueryAsync<Parametro>(pk).GetRemainingAsync();

        return results.Where(p => p.Tipo.Equals(tipo, StringComparison.OrdinalIgnoreCase));
    }
}