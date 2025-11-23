using Microsoft.AspNetCore.Mvc;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NominaController: ControllerBase
{
    private readonly INominaService _nominaService;
    private readonly IWorkspaceService _workspaceService;

    public NominaController(
        INominaService nominaService,
        IWorkspaceService workspaceService)
    {
        _nominaService = nominaService;
        _workspaceService = workspaceService;
    }
    
    // ============================================================
    // GET: api/nomina/empleado/{empleadoId}
    // ============================================================
    [HttpGet("empleado/{empleadoId}")]
    public async Task<IActionResult> GetByEmpleado(string empleadoId)
    {
        var nominas = await _nominaService.GetNominasByEmpleadoAsync(empleadoId);
        return Ok(nominas);
    }

    // ============================================================
    // GET: api/nomina/periodo/{periodo}
    // ============================================================
    [HttpGet("periodo/{periodo}")]
    public async Task<IActionResult> GetByPeriodo(string periodo)
    {
        var nominas = await _nominaService.GetNominasByPeriodoAsync(periodo);
        return Ok(nominas);
    }

    // ============================================================
    // GET: api/nomina/empleado/{empleadoId}/periodo/{periodo}
    // ============================================================
    [HttpGet("empleado/{empleadoId}/periodo/{periodo}")]
    public async Task<IActionResult> GetNomina(string empleadoId, string periodo)
    {
        var nomina = await _nominaService.GetNominaAsync(empleadoId, periodo);

        if (nomina == null)
            return NotFound("No existe una nómina para ese empleado y período.");

        return Ok(nomina);
    }

    // ============================================================
    // POST: api/nomina/generar
    // BODY: { empleadoId, periodo }
    // ============================================================
    [HttpPost("generar")]
    public async Task<IActionResult> GenerarNomina([FromBody] dynamic body)
    {
        string empleadoId = body?.empleadoId;
        string periodo     = body?.periodo;

        if (string.IsNullOrWhiteSpace(empleadoId) || string.IsNullOrWhiteSpace(periodo))
            return BadRequest("empleadoId y periodo son obligatorios.");

        // Validar si el período está abierto
        var ws = await _workspaceService.GetByPeriodoAsync(periodo);
        if (ws == null)
            return BadRequest("El período no existe. Debe crear primero un workspace.");

        if (ws.Estado == 1)
            return BadRequest("El período ya está cerrado.");

        // Generar la nómina
        var nomina = await _nominaService.GenerarNominaAsync(empleadoId, periodo);

        return Ok(nomina);
    }

    // ============================================================
    // POST: api/nomina/recalcular
    // BODY: { empleadoId, periodo }
    // ============================================================
    [HttpPost("recalcular")]
    public async Task<IActionResult> RecalcularNomina([FromBody] dynamic body)
    {
        string empleadoId = body?.empleadoId;
        string periodo     = body?.periodo;

        if (string.IsNullOrWhiteSpace(empleadoId) || string.IsNullOrWhiteSpace(periodo))
            return BadRequest("empleadoId y periodo son obligatorios.");

        // Validar período abierto
        var ws = await _workspaceService.GetByPeriodoAsync(periodo);
        if (ws == null)
            return BadRequest("El período no existe.");

        if (ws.Estado == 1)
            return BadRequest("El período está cerrado y no puede recalcularse.");

        var nomina = await _nominaService.RecalcularNominaAsync(empleadoId, periodo);
        return Ok(nomina);
    }

    // ============================================================
    // DELETE: api/nomina/empleado/{empleadoId}/periodo/{periodo}
    // ============================================================
    [HttpDelete("empleado/{empleadoId}/periodo/{periodo}")]
    public async Task<IActionResult> Delete(string empleadoId, string periodo)
    {
        var deleted = await _nominaService.DeleteNominaAsync(empleadoId, periodo);

        if (!deleted)
            return NotFound("No se encontró la nómina a eliminar.");

        return Ok(new { message = "Nómina eliminada correctamente." });
    }
}