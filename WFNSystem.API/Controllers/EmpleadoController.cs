using Microsoft.AspNetCore.Mvc;
using WFNSystem.API.Models;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmpleadoController : ControllerBase
{
    private readonly IEmpleadoService _empleadoService;
    private readonly ILogger<EmpleadoController> _logger;

    public EmpleadoController(IEmpleadoService empleadoService, ILogger<EmpleadoController> logger)
    {
        _empleadoService = empleadoService;
        _logger = logger;
    }
    
    // ============================================================
    // GET: api/empleado
    // ============================================================
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var empleados = await _empleadoService.GetAllAsync();
            return Ok(empleados);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener empleados");
            return StatusCode(500, new { message = "Error al obtener los empleados", error = ex.Message });
        }
    }

    // ============================================================
    // GET: api/empleado/{id}
    // ============================================================
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        try
        {
            var empleado = await _empleadoService.GetByIdAsync(id);
            if (empleado == null)
                return NotFound(new { message = "Empleado no encontrado" });

            return Ok(empleado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener empleado {EmpleadoId}", id);
            return StatusCode(500, new { message = "Error al obtener el empleado", error = ex.Message });
        }
    }

    // ============================================================
    // POST: api/empleado
    // ============================================================
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Empleado empleado)
    {
        try
        {
            if (empleado == null)
                return BadRequest(new { message = "Datos inválidos" });

            var created = await _empleadoService.CreateAsync(empleado);
            return CreatedAtAction(nameof(GetById),
                new { id = created.ID_Empleado },
                created);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validación fallida al crear empleado");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear empleado");
            return StatusCode(500, new { message = "Error al crear el empleado", error = ex.Message });
        }
    }

    // ============================================================
    // PUT: api/empleado/{id}
    // ============================================================
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] Empleado empleado)
    {
        try
        {
            var exists = await _empleadoService.GetByIdAsync(id);
            if (exists == null)
                return NotFound(new { message = "Empleado no encontrado" });

            empleado.ID_Empleado = id;

            var updated = await _empleadoService.UpdateAsync(empleado);
            return Ok(updated);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validación fallida al actualizar empleado {EmpleadoId}", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar empleado {EmpleadoId}", id);
            return StatusCode(500, new { message = "Error al actualizar el empleado", error = ex.Message });
        }
    }

    // ============================================================
    // DELETE: api/empleado/{id}
    // ============================================================
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            var deleted = await _empleadoService.DeleteAsync(id);

            if (!deleted) 
                return NotFound(new { message = "No existe el empleado" });

            return Ok(new { message = "Empleado eliminado correctamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar empleado {EmpleadoId}", id);
            return StatusCode(500, new { message = "Error al eliminar el empleado", error = ex.Message });
        }
    }
}