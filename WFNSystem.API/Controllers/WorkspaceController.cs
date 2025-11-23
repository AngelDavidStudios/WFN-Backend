using Microsoft.AspNetCore.Mvc;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WorkspaceController: ControllerBase
{
    private readonly IWorkspaceService _workspaceService;

    public WorkspaceController(IWorkspaceService workspaceService)
    {
        _workspaceService = workspaceService;
    }
    
    // ============================================================
    // GET: api/workspace
    // ============================================================
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var workspaces = await _workspaceService.GetAllAsync();
        return Ok(workspaces);
    }

    // ============================================================
    // GET: api/workspace/{periodo}
    // ============================================================
    [HttpGet("{periodo}")]
    public async Task<IActionResult> GetByPeriodo(string periodo)
    {
        var ws = await _workspaceService.GetByPeriodoAsync(periodo);

        if (ws == null)
            return NotFound("No existe un workspace para ese período.");

        return Ok(ws);
    }

    // ============================================================
    // POST: api/workspace/{periodo}
    // ============================================================
    [HttpPost("{periodo}")]
    public async Task<IActionResult> CrearPeriodo(string periodo)
    {
        var creado = await _workspaceService.CrearPeriodoAsync(periodo);
        return Ok(creado);
    }

    // ============================================================
    // POST: api/workspace/{periodo}/cerrar
    // ============================================================
    [HttpPost("{periodo}/cerrar")]
    public async Task<IActionResult> CerrarPeriodo(string periodo)
    {
        var cerrado = await _workspaceService.CerrarPeriodoAsync(periodo);
        return Ok(cerrado);
    }

    // ============================================================
    // PUT: api/workspace
    // BODY contiene el WorkspaceNomina completo
    // ============================================================
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] WorkspaceNomina workspace)
    {
        if (workspace == null)
            return BadRequest("Datos inválidos.");

        var updated = await _workspaceService.UpdateAsync(workspace);
        return Ok(updated);
    }

    // ============================================================
    // DELETE: api/workspace/{periodo}
    // ============================================================
    [HttpDelete("{periodo}")]
    public async Task<IActionResult> Delete(string periodo)
    {
        var deleted = await _workspaceService.DeleteAsync(periodo);

        if (!deleted)
            return NotFound("No existe un workspace para ese período.");

        return Ok(new { message = "Workspace eliminado correctamente." });
    }

    // ============================================================
    // GET: api/workspace/{periodo}/estado
    // ============================================================
    [HttpGet("{periodo}/estado")]
    public async Task<IActionResult> VerificarEstado(string periodo)
    {
        var abierto = await _workspaceService.VerificarPeriodoAbiertoAsync(periodo);

        return Ok(new
        {
            periodo,
            abierto
        });
    }
}