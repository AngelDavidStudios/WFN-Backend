using Microsoft.AspNetCore.Mvc;
using WFNSystem.API.Models;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProvisionController : ControllerBase
{
    private readonly IProvisionService _provisionService;
    private readonly ILogger<ProvisionController> _logger;

    public ProvisionController(IProvisionService provisionService, ILogger<ProvisionController> logger)
    {
        _provisionService = provisionService;
        _logger = logger;
    }

    // ============================================================
    // GET: api/provision/empleado/{empleadoId}
    // Obtiene todas las provisiones de un empleado
    // ============================================================
    [HttpGet("empleado/{empleadoId}")]
    public async Task<IActionResult> GetByEmpleado(string empleadoId)
    {
        try
        {
            var provisiones = await _provisionService.GetByEmpleadoAsync(empleadoId);
            return Ok(provisiones);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener provisiones del empleado {EmpleadoId}", empleadoId);
            return StatusCode(500, new { message = "Error al obtener las provisiones", error = ex.Message });
        }
    }

    // ============================================================
    // GET: api/provision/empleado/{empleadoId}/periodo/{periodo}
    // Obtiene las provisiones de un empleado en un periodo específico
    // ============================================================
    [HttpGet("empleado/{empleadoId}/periodo/{periodo}")]
    public async Task<IActionResult> GetByPeriodo(string empleadoId, string periodo)
    {
        try
        {
            var provisiones = await _provisionService.GetByPeriodoAsync(empleadoId, periodo);
            return Ok(provisiones);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener provisiones del empleado {EmpleadoId} periodo {Periodo}", empleadoId, periodo);
            return StatusCode(500, new { message = "Error al obtener las provisiones", error = ex.Message });
        }
    }

    // ============================================================
    // GET: api/provision/empleado/{empleadoId}/tipo/{tipoProvision}
    // Obtiene las provisiones de un empleado por tipo
    // ============================================================
    [HttpGet("empleado/{empleadoId}/tipo/{tipoProvision}")]
    public async Task<IActionResult> GetByTipo(string empleadoId, string tipoProvision)
    {
        try
        {
            var provisiones = await _provisionService.GetByTipoAsync(empleadoId, tipoProvision);
            return Ok(provisiones);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener provisiones tipo {TipoProvision} del empleado {EmpleadoId}", tipoProvision, empleadoId);
            return StatusCode(500, new { message = "Error al obtener las provisiones", error = ex.Message });
        }
    }

    // ============================================================
    // GET: api/provision/empleado/{empleadoId}/tipo/{tipoProvision}/periodo/{periodo}
    // Obtiene una provisión específica
    // ============================================================
    [HttpGet("empleado/{empleadoId}/tipo/{tipoProvision}/periodo/{periodo}")]
    public async Task<IActionResult> GetById(string empleadoId, string tipoProvision, string periodo)
    {
        try
        {
            var provision = await _provisionService.GetByIdAsync(empleadoId, tipoProvision, periodo);

            if (provision == null)
                return NotFound(new { message = "Provisión no encontrada" });

            return Ok(provision);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener provisión {TipoProvision} del empleado {EmpleadoId} periodo {Periodo}", 
                tipoProvision, empleadoId, periodo);
            return StatusCode(500, new { message = "Error al obtener la provisión", error = ex.Message });
        }
    }

    // ============================================================
    // POST: api/provision/empleado/{empleadoId}
    // Crea una nueva provisión para un empleado
    // ============================================================
    [HttpPost("empleado/{empleadoId}")]
    public async Task<IActionResult> Create(string empleadoId, [FromBody] Provision provision)
    {
        try
        {
            if (provision == null)
                return BadRequest(new { message = "Datos inválidos" });

            if (string.IsNullOrEmpty(provision.TipoProvision))
                return BadRequest(new { message = "El tipo de provisión es requerido" });

            if (string.IsNullOrEmpty(provision.Periodo))
                return BadRequest(new { message = "El periodo es requerido" });

            var created = await _provisionService.CreateAsync(empleadoId, provision);

            return CreatedAtAction(
                nameof(GetById),
                new { empleadoId, tipoProvision = created.TipoProvision, periodo = created.Periodo },
                created
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear provisión para empleado {EmpleadoId}", empleadoId);
            return StatusCode(500, new { message = "Error al crear la provisión", error = ex.Message });
        }
    }

    // ============================================================
    // PUT: api/provision/empleado/{empleadoId}/tipo/{tipoProvision}/periodo/{periodo}
    // Actualiza una provisión existente
    // ============================================================
    [HttpPut("empleado/{empleadoId}/tipo/{tipoProvision}/periodo/{periodo}")]
    public async Task<IActionResult> Update(string empleadoId, string tipoProvision, string periodo, [FromBody] Provision provision)
    {
        try
        {
            var exists = await _provisionService.GetByIdAsync(empleadoId, tipoProvision, periodo);
            if (exists == null)
                return NotFound(new { message = "Provisión no encontrada" });

            provision.TipoProvision = tipoProvision;
            provision.Periodo = periodo;

            var updated = await _provisionService.UpdateAsync(empleadoId, provision);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar provisión {TipoProvision} del empleado {EmpleadoId}", tipoProvision, empleadoId);
            return StatusCode(500, new { message = "Error al actualizar la provisión", error = ex.Message });
        }
    }

    // ============================================================
    // DELETE: api/provision/empleado/{empleadoId}/tipo/{tipoProvision}/periodo/{periodo}
    // Elimina una provisión
    // ============================================================
    [HttpDelete("empleado/{empleadoId}/tipo/{tipoProvision}/periodo/{periodo}")]
    public async Task<IActionResult> Delete(string empleadoId, string tipoProvision, string periodo)
    {
        try
        {
            var deleted = await _provisionService.DeleteAsync(empleadoId, tipoProvision, periodo);

            if (!deleted)
                return NotFound(new { message = "No existe la provisión especificada" });

            return Ok(new { message = "Provisión eliminada correctamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar provisión {TipoProvision} del empleado {EmpleadoId}", tipoProvision, empleadoId);
            return StatusCode(500, new { message = "Error al eliminar la provisión", error = ex.Message });
        }
    }

    // ============================================================
    // POST: api/provision/procesar
    // Body: { empleadoId: "EMP001", periodo: "2025-11" }
    // Procesa y calcula todas las provisiones de un empleado en un periodo
    // ============================================================
    [HttpPost("procesar")]
    public async Task<IActionResult> ProcesarProvisiones([FromBody] ProcesarProvisionesRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.EmpleadoId) || string.IsNullOrEmpty(request.Periodo))
                return BadRequest(new { message = "EmpleadoId y Periodo son requeridos" });

            await _provisionService.ProcesarProvisionesAsync(request.EmpleadoId, request.Periodo);

            return Ok(new { message = "Provisiones procesadas correctamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar provisiones para {EmpleadoId} periodo {Periodo}", 
                request.EmpleadoId, request.Periodo);
            return StatusCode(500, new { message = "Error al procesar las provisiones", error = ex.Message });
        }
    }
}

// DTO para request
public class ProcesarProvisionesRequest
{
    public string EmpleadoId { get; set; } = string.Empty;
    public string Periodo { get; set; } = string.Empty;
}