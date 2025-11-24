using Microsoft.AspNetCore.Mvc;
using WFNSystem.API.Models;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NovedadController : ControllerBase
{
    private readonly INovedadService _novedadService;
    private readonly ILogger<NovedadController> _logger;

    public NovedadController(INovedadService novedadService, ILogger<NovedadController> logger)
    {
        _novedadService = novedadService;
        _logger = logger;
    }

    // ============================================================
    // GET: api/novedad/empleado/{empleadoId}
    // Obtiene todas las novedades de un empleado
    // ============================================================
    [HttpGet("empleado/{empleadoId}")]
    public async Task<IActionResult> GetByEmpleado(string empleadoId)
    {
        try
        {
            var novedades = await _novedadService.GetByEmpleadoAsync(empleadoId);
            return Ok(novedades);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener novedades del empleado {EmpleadoId}", empleadoId);
            return StatusCode(500, new { message = "Error al obtener las novedades", error = ex.Message });
        }
    }

    // ============================================================
    // GET: api/novedad/empleado/{empleadoId}/periodo/{periodo}
    // Obtiene las novedades de un empleado en un periodo específico
    // ============================================================
    [HttpGet("empleado/{empleadoId}/periodo/{periodo}")]
    public async Task<IActionResult> GetByPeriodo(string empleadoId, string periodo)
    {
        try
        {
            var novedades = await _novedadService.GetByPeriodoAsync(empleadoId, periodo);
            return Ok(novedades);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener novedades del empleado {EmpleadoId} periodo {Periodo}", empleadoId, periodo);
            return StatusCode(500, new { message = "Error al obtener las novedades", error = ex.Message });
        }
    }

    // ============================================================
    // GET: api/novedad/empleado/{empleadoId}/periodo/{periodo}/{novedadId}
    // Obtiene una novedad específica
    // ============================================================
    [HttpGet("empleado/{empleadoId}/periodo/{periodo}/{novedadId}")]
    public async Task<IActionResult> GetById(string empleadoId, string periodo, string novedadId)
    {
        try
        {
            var novedad = await _novedadService.GetByIdAsync(empleadoId, periodo, novedadId);

            if (novedad == null)
                return NotFound(new { message = "Novedad no encontrada" });

            return Ok(novedad);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener novedad {NovedadId}", novedadId);
            return StatusCode(500, new { message = "Error al obtener la novedad", error = ex.Message });
        }
    }

    // ============================================================
    // POST: api/novedad/empleado/{empleadoId}
    // Crea una nueva novedad para un empleado
    // ============================================================
    [HttpPost("empleado/{empleadoId}")]
    public async Task<IActionResult> Create(string empleadoId, [FromBody] Novedad novedad)
    {
        try
        {
            if (novedad == null)
                return BadRequest(new { message = "Datos inválidos" });

            if (string.IsNullOrEmpty(novedad.Periodo))
                return BadRequest(new { message = "El periodo es requerido" });

            if (string.IsNullOrEmpty(novedad.ID_Parametro))
                return BadRequest(new { message = "El parámetro es requerido" });

            if (string.IsNullOrEmpty(novedad.TipoNovedad))
                return BadRequest(new { message = "El tipo de novedad es requerido" });

            var created = await _novedadService.CreateAsync(empleadoId, novedad);

            return CreatedAtAction(
                nameof(GetById),
                new { empleadoId, periodo = created.Periodo, novedadId = created.ID_Novedad },
                created
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear novedad para empleado {EmpleadoId}", empleadoId);
            return StatusCode(500, new { message = "Error al crear la novedad", error = ex.Message });
        }
    }

    // ============================================================
    // PUT: api/novedad/empleado/{empleadoId}/periodo/{periodo}/{novedadId}
    // Actualiza una novedad existente
    // ============================================================
    [HttpPut("empleado/{empleadoId}/periodo/{periodo}/{novedadId}")]
    public async Task<IActionResult> Update(string empleadoId, string periodo, string novedadId, [FromBody] Novedad novedad)
    {
        try
        {
            var exists = await _novedadService.GetByIdAsync(empleadoId, periodo, novedadId);
            if (exists == null)
                return NotFound(new { message = "Novedad no encontrada" });

            novedad.ID_Novedad = novedadId;
            novedad.Periodo = periodo;

            var updated = await _novedadService.UpdateAsync(empleadoId, novedad);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar novedad {NovedadId}", novedadId);
            return StatusCode(500, new { message = "Error al actualizar la novedad", error = ex.Message });
        }
    }

    // ============================================================
    // DELETE: api/novedad/empleado/{empleadoId}/periodo/{periodo}/{novedadId}
    // Elimina una novedad
    // ============================================================
    [HttpDelete("empleado/{empleadoId}/periodo/{periodo}/{novedadId}")]
    public async Task<IActionResult> Delete(string empleadoId, string periodo, string novedadId)
    {
        try
        {
            var deleted = await _novedadService.DeleteAsync(empleadoId, novedadId, periodo);

            if (!deleted)
                return NotFound(new { message = "No existe la novedad especificada" });

            return Ok(new { message = "Novedad eliminada correctamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar novedad {NovedadId}", novedadId);
            return StatusCode(500, new { message = "Error al eliminar la novedad", error = ex.Message });
        }
    }
}