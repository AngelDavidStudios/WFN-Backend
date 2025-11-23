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
        var conditions = new List<ScanCondition>
        {
            new ScanCondition("SK", ScanOperator.Equal, "META#WS")
        };

        return await _context
            .ScanAsync<WorkspaceNomina>(conditions)
            .GetRemainingAsync();
    }

    public async Task<WorkspaceNomina?> GetByPeriodoAsync(string periodo)
    {
        string pk = $"WS#{periodo}";
        string sk = "META#WS";

        return await _context.LoadAsync<WorkspaceNomina>(pk, sk);
    }

    public async Task AddAsync(WorkspaceNomina workspace)
    {
        workspace.PK = $"WS#{workspace.Periodo}";
        workspace.SK = "META#WS";

        await _context.SaveAsync(workspace);
    }

    public async Task UpdateAsync(WorkspaceNomina workspace)
    {
        workspace.PK = $"WS#{workspace.Periodo}";
        workspace.SK = "META#WS";

        await _context.SaveAsync(workspace);
    }

    public async Task DeleteAsync(string periodo)
    {
        string pk = $"WS#{periodo}";
        string sk = "META#WS";

        await _context.DeleteAsync<WorkspaceNomina>(pk, sk);
    }
}