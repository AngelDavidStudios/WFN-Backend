using Microsoft.AspNetCore.Mvc;
using WFNSystem.API.Models;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NominaController : ControllerBase
{
    private readonly INominaService _nominaService;
    private readonly ILogger<NominaController> _logger;

    public NominaController(INominaService nominaService, ILogger<NominaController> logger)
    {
        _nominaService = nominaService;
        _logger = logger;
    }

    // ============================================================
    // GET: api/nomina/empleado/{empleadoId}
    // Obtiene todas las nóminas de un empleado
    // ============================================================
    [HttpGet("empleado/{empleadoId}")]
    public async Task<IActionResult> GetByEmpleado(string empleadoId)
    {
        try
        {
            var nominas = await _nominaService.GetByEmpleadoAsync(empleadoId);
            return Ok(nominas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener nóminas del empleado {EmpleadoId}", empleadoId);
            return StatusCode(500, new { message = "Error al obtener las nóminas", error = ex.Message });
        }
    }

    // ============================================================
    // GET: api/nomina/empleado/{empleadoId}/periodo/{periodo}
    // Obtiene la nómina de un empleado en un periodo específico
    // ============================================================
    [HttpGet("empleado/{empleadoId}/periodo/{periodo}")]
    public async Task<IActionResult> GetByPeriodo(string empleadoId, string periodo)
    {
        try
        {
            var nomina = await _nominaService.GetByPeriodoAsync(empleadoId, periodo);
            
            if (nomina == null)
                return NotFound(new { message = $"No existe nómina para el empleado {empleadoId} en el periodo {periodo}" });

            return Ok(nomina);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener nómina del empleado {EmpleadoId} periodo {Periodo}", empleadoId, periodo);
            return StatusCode(500, new { message = "Error al obtener la nómina", error = ex.Message });
        }
    }

    // ============================================================
    // GET: api/nomina/periodo/{periodo}
    // Obtiene todas las nóminas de un periodo (todos los empleados)
    // ============================================================
    [HttpGet("periodo/{periodo}")]
    public async Task<IActionResult> GetByPeriodoGlobal(string periodo)
    {
        try
        {
            var nominas = await _nominaService.GetByPeriodoGlobalAsync(periodo);
            return Ok(nominas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener nóminas del periodo {Periodo}", periodo);
            return StatusCode(500, new { message = "Error al obtener las nóminas del periodo", error = ex.Message });
        }
    }

    // ============================================================
    // POST: api/nomina/generar
    // Body: { empleadoId: "EMP001", periodo: "2025-11" }
    // Genera una nueva nómina para un empleado en un periodo
    // ============================================================
    [HttpPost("generar")]
    public async Task<IActionResult> GenerarNomina([FromBody] GenerarNominaRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.EmpleadoId) || string.IsNullOrEmpty(request.Periodo))
                return BadRequest(new { message = "EmpleadoId y Periodo son requeridos" });

            var nomina = await _nominaService.GenerarNominaAsync(request.EmpleadoId, request.Periodo);
            
            return CreatedAtAction(
                nameof(GetByPeriodo),
                new { empleadoId = request.EmpleadoId, periodo = request.Periodo },
                nomina
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar nómina para {EmpleadoId} periodo {Periodo}", 
                request.EmpleadoId, request.Periodo);
            return StatusCode(500, new { message = "Error al generar la nómina", error = ex.Message });
        }
    }

    // ============================================================
    // POST: api/nomina/recalcular
    // Body: { empleadoId: "EMP001", periodo: "2025-11" }
    // Recalcula una nómina existente
    // ============================================================
    [HttpPost("recalcular")]
    public async Task<IActionResult> RecalcularNomina([FromBody] RecalcularNominaRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.EmpleadoId) || string.IsNullOrEmpty(request.Periodo))
                return BadRequest(new { message = "EmpleadoId y Periodo son requeridos" });

            var nomina = await _nominaService.RecalcularNominaAsync(request.EmpleadoId, request.Periodo);
            return Ok(nomina);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al recalcular nómina para {EmpleadoId} periodo {Periodo}", 
                request.EmpleadoId, request.Periodo);
            return StatusCode(500, new { message = "Error al recalcular la nómina", error = ex.Message });
        }
    }

    // ============================================================
    // DELETE: api/nomina/empleado/{empleadoId}/periodo/{periodo}
    // Elimina una nómina
    // ============================================================
    [HttpDelete("empleado/{empleadoId}/periodo/{periodo}")]
    public async Task<IActionResult> Delete(string empleadoId, string periodo)
    {
        try
        {
            var deleted = await _nominaService.DeleteNominaAsync(empleadoId, periodo);

            if (!deleted)
                return NotFound(new { message = "No existe la nómina especificada" });

            return Ok(new { message = "Nómina eliminada correctamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar nómina del empleado {EmpleadoId} periodo {Periodo}", empleadoId, periodo);
            return StatusCode(500, new { message = "Error al eliminar la nómina", error = ex.Message });
        }
    }
}

// DTOs para requests
public class GenerarNominaRequest
{
    public string EmpleadoId { get; set; } = string.Empty;
    public string Periodo { get; set; } = string.Empty;
}

public class RecalcularNominaRequest
{
    public string EmpleadoId { get; set; } = string.Empty;
    public string Periodo { get; set; } = string.Empty;
}