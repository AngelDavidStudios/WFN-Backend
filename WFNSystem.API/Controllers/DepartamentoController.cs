using Microsoft.AspNetCore.Mvc;
using WFNSystem.API.Models;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DepartamentoController : ControllerBase
{
    private readonly IDepartamentoService _departamentoService;
    private readonly ILogger<DepartamentoController> _logger;

    public DepartamentoController(IDepartamentoService departamentoService, ILogger<DepartamentoController> logger)
    {
        _departamentoService = departamentoService;
        _logger = logger;
    }
    
    // ============================================================
    // GET: api/departamento
    // ============================================================
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await _departamentoService.GetAllAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener departamentos");
            return StatusCode(500, new { message = "Error al obtener los departamentos", error = ex.Message });
        }
    }

    // ============================================================
    // GET: api/departamento/{id}
    // ============================================================
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        try
        {
            var dep = await _departamentoService.GetByIdAsync(id);
            if (dep == null)
                return NotFound(new { message = "Departamento no encontrado" });

            return Ok(dep);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener departamento {DepartamentoId}", id);
            return StatusCode(500, new { message = "Error al obtener el departamento", error = ex.Message });
        }
    }

    // ============================================================
    // POST: api/departamento
    // ============================================================
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Departamento dep)
    {
        try
        {
            if (dep == null)
                return BadRequest(new { message = "Datos inválidos" });

            var created = await _departamentoService.CreateAsync(dep);
            return CreatedAtAction(nameof(GetById), new { id = created.ID_Departamento }, created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear departamento");
            return StatusCode(500, new { message = "Error al crear el departamento", error = ex.Message });
        }
    }

    // ============================================================
    // PUT: api/departamento/{id}
    // ============================================================
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] Departamento dep)
    {
        try
        {
            var exists = await _departamentoService.GetByIdAsync(id);
            if (exists == null)
                return NotFound(new { message = "Departamento no encontrado" });

            dep.ID_Departamento = id;
            var updated = await _departamentoService.UpdateAsync(dep);

            return Ok(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar departamento {DepartamentoId}", id);
            return StatusCode(500, new { message = "Error al actualizar el departamento", error = ex.Message });
        }
    }

    // ============================================================
    // DELETE: api/departamento/{id}
    // ============================================================
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            var deleted = await _departamentoService.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { message = "No existe el departamento" });

            return Ok(new { message = "Departamento eliminado correctamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar departamento {DepartamentoId}", id);
            return StatusCode(500, new { message = "Error al eliminar el departamento", error = ex.Message });
        }
    }
}