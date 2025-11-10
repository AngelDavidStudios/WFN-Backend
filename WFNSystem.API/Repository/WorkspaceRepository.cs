using Amazon.DynamoDBv2.DataModel;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;

namespace WFNSystem.API.Repository;

public class WorkspaceRepository: IRepository<Workspace>
{
    private readonly DynamoDBContext _context;
    
    public WorkspaceRepository(DynamoDBContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Workspace>> GetAllAsync()
    {
        var conditions = new List<ScanCondition>();
        var allWorkspaces = await _context.ScanAsync<Workspace>(conditions).GetRemainingAsync();
        return allWorkspaces;
    }
    
    public async Task<Workspace> GetByIdAsync(string id)
    {
        return await _context.LoadAsync<Workspace>(id);
    }
    
    public async Task AddAsync(Workspace workspace)
    {
        workspace.ID_Workspace = Guid.NewGuid().ToString();
        await _context.SaveAsync(workspace);
    }
    
    public async Task UpdateAsync(string id, Workspace workspace)
    {
        var existingWorkspace = await GetByIdAsync(id);
        if (existingWorkspace == null)
        {
            throw new Exception("Workspace not found");
        }
        
        workspace.ID_Workspace = id;
        await _context.SaveAsync(workspace);
    }
    
    public async Task DeleteAsync(string id)
    {
        var workspace = await GetByIdAsync(id);
        if (workspace == null)
        {
            throw new Exception("Workspace not found");
        }
        
        await _context.DeleteAsync<Workspace>(id);
    }
}