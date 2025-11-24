using Microsoft.AspNetCore.Mvc;
using WFNSystem.API.Models;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WorkspaceController : ControllerBase
{
    private readonly IWorkspaceService _workspaceService;
    private readonly ILogger<WorkspaceController> _logger;

    public WorkspaceController(IWorkspaceService workspaceService, ILogger<WorkspaceController> logger)
    {
        _workspaceService = workspaceService;
        _logger = logger;
    }
    
    // ============================================================
    // GET: api/workspace
    // ============================================================
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var workspaces = await _workspaceService.GetAllAsync();
            return Ok(workspaces);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener workspaces");
            return StatusCode(500, new { message = "Error al obtener los workspaces", error = ex.Message });
        }
    }

    // ============================================================
    // GET: api/workspace/{periodo}
    // ============================================================
    [HttpGet("{periodo}")]
    public async Task<IActionResult> GetByPeriodo(string periodo)
    {
        try
        {
            var ws = await _workspaceService.GetByPeriodoAsync(periodo);

            if (ws == null)
                return NotFound(new { message = $"No existe un workspace para el período {periodo}" });

            return Ok(ws);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener workspace del periodo {Periodo}", periodo);
            return StatusCode(500, new { message = "Error al obtener el workspace", error = ex.Message });
        }
    }

    // ============================================================
    // POST: api/workspace/{periodo}
    // ============================================================
    [HttpPost("{periodo}")]
    public async Task<IActionResult> CrearPeriodo(string periodo)
    {
        try
        {
            var creado = await _workspaceService.CrearPeriodoAsync(periodo);
            return CreatedAtAction(nameof(GetByPeriodo), new { periodo = creado.Periodo }, creado);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validación fallida al crear workspace para periodo {Periodo}", periodo);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear workspace para periodo {Periodo}", periodo);
            return StatusCode(500, new { message = "Error al crear el workspace", error = ex.Message });
        }
    }

    // ============================================================
    // POST: api/workspace/{periodo}/cerrar
    // ============================================================
    [HttpPost("{periodo}/cerrar")]
    public async Task<IActionResult> CerrarPeriodo(string periodo)
    {
        try
        {
            var cerrado = await _workspaceService.CerrarPeriodoAsync(periodo);
            return Ok(cerrado);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validación fallida al cerrar workspace del periodo {Periodo}", periodo);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cerrar workspace del periodo {Periodo}", periodo);
            return StatusCode(500, new { message = "Error al cerrar el workspace", error = ex.Message });
        }
    }

    // ============================================================
    // PUT: api/workspace
    // BODY contiene el WorkspaceNomina completo
    // ============================================================
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] WorkspaceNomina workspace)
    {
        try
        {
            if (workspace == null)
                return BadRequest(new { message = "Datos inválidos" });

            var updated = await _workspaceService.UpdateAsync(workspace);
            return Ok(updated);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validación fallida al actualizar workspace");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar workspace");
            return StatusCode(500, new { message = "Error al actualizar el workspace", error = ex.Message });
        }
    }

    // ============================================================
    // DELETE: api/workspace/{periodo}
    // ============================================================
    [HttpDelete("{periodo}")]
    public async Task<IActionResult> Delete(string periodo)
    {
        try
        {
            var deleted = await _workspaceService.DeleteAsync(periodo);

            if (!deleted)
                return NotFound(new { message = $"No existe un workspace para el período {periodo}" });

            return Ok(new { message = "Workspace eliminado correctamente" });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validación fallida al eliminar workspace del periodo {Periodo}", periodo);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar workspace del periodo {Periodo}", periodo);
            return StatusCode(500, new { message = "Error al eliminar el workspace", error = ex.Message });
        }
    }

    // ============================================================
    // GET: api/workspace/{periodo}/estado
    // ============================================================
    [HttpGet("{periodo}/estado")]
    public async Task<IActionResult> VerificarEstado(string periodo)
    {
        try
        {
            var abierto = await _workspaceService.VerificarPeriodoAbiertoAsync(periodo);

            return Ok(new
            {
                periodo,
                abierto,
                estado = abierto ? "ABIERTO" : "CERRADO"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar estado del workspace periodo {Periodo}", periodo);
            return StatusCode(500, new { message = "Error al verificar el estado", error = ex.Message });
        }
    }
}