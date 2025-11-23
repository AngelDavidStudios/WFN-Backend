using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;

namespace WFNSystem.API.Repository;

public class WorkspaceRepository: IWorkspaceRepository
{
    private readonly IDynamoDBContext _context;
    
    public WorkspaceRepository(IDynamoDBContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<WorkspaceNomina>> GetAllAsync()
    {
        string pk = "WORKSPACE#GLOBAL";

        var query = _context.QueryAsync<WorkspaceNomina>(pk);
        return await query.GetRemainingAsync();
    }
    
    public async Task<WorkspaceNomina?> GetByPeriodoAsync(string periodo)
    {
        string pk = "WORKSPACE#GLOBAL";
        string sk = $"WORK#{periodo}";

        return await _context.LoadAsync<WorkspaceNomina>(pk, sk);
    }
    
    public async Task AddAsync(WorkspaceNomina workspace)
    {
        await _context.SaveAsync(workspace);
    }

    public async Task UpdateAsync(WorkspaceNomina workspace)
    {
        await _context.SaveAsync(workspace);
    }

    public async Task DeleteAsync(string periodo)
    {
        string pk = "WORKSPACE#GLOBAL";
        string sk = $"WORK#{periodo}";

        await _context.DeleteAsync<WorkspaceNomina>(pk, sk);
    }
}