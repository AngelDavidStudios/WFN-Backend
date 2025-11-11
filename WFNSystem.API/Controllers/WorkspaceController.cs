using Microsoft.AspNetCore.Mvc;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;

namespace WFNSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WorkspaceController: ControllerBase
{
    private readonly IRepository<Workspace> _workspaceRepository;
    
    public WorkspaceController(IRepository<Workspace> workspaceRepository)
    {
        _workspaceRepository = workspaceRepository;
    }
    
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var workspaces = await _workspaceRepository.GetAllAsync();
        return Ok(workspaces);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var workspace = await _workspaceRepository.GetByIdAsync(id);
        return Ok(workspace);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Workspace workspace)
    {
        if (workspace.ID_Workspace != null)
        {
            workspace.ID_Workspace = null;
        }

        await _workspaceRepository.AddAsync(workspace);
        return Ok("Workspace created successfully");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] Workspace workspace)
    {
        await _workspaceRepository.UpdateAsync(id, workspace);
        return Ok("Workspace updated successfully");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _workspaceRepository.DeleteAsync(id);
        return Ok("Workspace deleted successfully");
    }
}