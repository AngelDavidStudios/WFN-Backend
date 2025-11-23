using Microsoft.AspNetCore.Mvc;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NovedadController: ControllerBase
{
    private readonly INovedadService _novedadService;

    public NovedadController(INovedadService novedadService)
    {
        _novedadService = novedadService;
    }
    
    // ============================================================
    // GET: api/novedad/empleado/{empleadoId}
    // ============================================================
    [HttpGet("empleado/{empleadoId}")]
    public async Task<IActionResult> GetByEmpleado(string empleadoId)
    {
        var novedades = await _novedadService.GetByEmpleadoAsync(empleadoId);
        return Ok(novedades);
    }

    // ============================================================
    // GET: api/novedad/empleado/{empleadoId}/periodo/{periodo}
    // ============================================================
    [HttpGet("empleado/{empleadoId}/periodo/{periodo}")]
    public async Task<IActionResult> GetByEmpleadoPeriodo(string empleadoId, string periodo)
    {
        var novedades = await _novedadService.GetByPeriodoAsync(empleadoId, periodo);

        if (novedades == null || !novedades.Any())
            return NotFound("No existen novedades para este empleado en este período.");

        return Ok(novedades);
    }

    // ============================================================
    // GET: api/novedad/empleado/{empleadoId}/novedad/{novedadId}
    // ============================================================
    [HttpGet("empleado/{empleadoId}/novedad/{novedadId}")]
    public async Task<IActionResult> GetById(string empleadoId, string novedadId)
    {
        var nov = await _novedadService.GetByIdAsync(empleadoId, novedadId);
        if (nov == null)
            return NotFound("Novedad no encontrada.");

        return Ok(nov);
    }

    // ============================================================
    // POST: api/novedad/empleado/{empleadoId}
    // ============================================================
    [HttpPost("empleado/{empleadoId}")]
    public async Task<IActionResult> Create(string empleadoId, [FromBody] Novedad novedad)
    {
        if (novedad == null)
            return BadRequest("Datos inválidos.");

        var created = await _novedadService.CreateAsync(empleadoId, novedad);

        return CreatedAtAction(nameof(GetById),
            new { empleadoId = empleadoId, novedadId = created.ID_Novedad },
            created);
    }

    // ============================================================
    // PUT: api/novedad/empleado/{empleadoId}/{novedadId}
    // ============================================================
    [HttpPut("empleado/{empleadoId}/{novedadId}")]
    public async Task<IActionResult> Update(string empleadoId, string novedadId, [FromBody] Novedad novedad)
    {
        var exists = await _novedadService.GetByIdAsync(empleadoId, novedadId);

        if (exists == null)
            return NotFound("Novedad no encontrada.");

        novedad.ID_Novedad = novedadId;

        var updated = await _novedadService.UpdateAsync(empleadoId, novedad);
        return Ok(updated);
    }

    // ============================================================
    // DELETE: api/novedad/empleado/{empleadoId}/{novedadId}
    // ============================================================
    [HttpDelete("empleado/{empleadoId}/{novedadId}")]
    public async Task<IActionResult> Delete(string empleadoId, string novedadId)
    {
        var deleted = await _novedadService.DeleteAsync(empleadoId, novedadId);

        if (!deleted)
            return NotFound("La novedad no existe.");

        return Ok(new { message = "Novedad eliminada correctamente." });
    }
}