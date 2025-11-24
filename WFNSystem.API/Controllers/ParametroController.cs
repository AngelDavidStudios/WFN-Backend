using Microsoft.AspNetCore.Mvc;
using WFNSystem.API.Models;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ParametroController : ControllerBase
{
    private readonly IParametroService _parametroService;
    private readonly ILogger<ParametroController> _logger;

    public ParametroController(IParametroService parametroService, ILogger<ParametroController> logger)
    {
        _parametroService = parametroService;
        _logger = logger;
    }
    
    // ============================================================
    // GET: api/parametro
    // ============================================================
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var parametros = await _parametroService.GetAllAsync();
            return Ok(parametros);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener parámetros");
            return StatusCode(500, new { message = "Error al obtener los parámetros", error = ex.Message });
        }
    }

    // ============================================================
    // GET: api/parametro/{id}
    // ============================================================
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        try
        {
            var parametro = await _parametroService.GetByIdAsync(id);
            if (parametro == null)
                return NotFound(new { message = "Parámetro no encontrado" });

            return Ok(parametro);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener parámetro {ParametroId}", id);
            return StatusCode(500, new { message = "Error al obtener el parámetro", error = ex.Message });
        }
    }

    // ============================================================
    // GET: api/parametro/tipo/{tipo}
    // Obtiene parámetros por tipo (INGRESO, EGRESO, PROVISION)
    // ============================================================
    [HttpGet("tipo/{tipo}")]
    public async Task<IActionResult> GetByTipo(string tipo)
    {
        try
        {
            var parametros = await _parametroService.GetByTipoAsync(tipo);
            return Ok(parametros);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener parámetros del tipo {Tipo}", tipo);
            return StatusCode(500, new { message = "Error al obtener los parámetros", error = ex.Message });
        }
    }

    // ============================================================
    // POST: api/parametro
    // ============================================================
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Parametro parametro)
    {
        try
        {
            if (parametro == null)
                return BadRequest(new { message = "Datos inválidos" });

            var created = await _parametroService.CreateAsync(parametro);

            return CreatedAtAction(nameof(GetById),
                new { id = created.ID_Parametro }, created);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validación fallida al crear parámetro");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear parámetro");
            return StatusCode(500, new { message = "Error al crear el parámetro", error = ex.Message });
        }
    }

    // ============================================================
    // PUT: api/parametro/{id}
    // ============================================================
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] Parametro parametro)
    {
        try
        {
            var exists = await _parametroService.GetByIdAsync(id);

            if (exists == null)
                return NotFound(new { message = "Parámetro no encontrado" });

            parametro.ID_Parametro = id;

            var updated = await _parametroService.UpdateAsync(parametro);
            return Ok(updated);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validación fallida al actualizar parámetro {ParametroId}", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar parámetro {ParametroId}", id);
            return StatusCode(500, new { message = "Error al actualizar el parámetro", error = ex.Message });
        }
    }

    // ============================================================
    // DELETE: api/parametro/{id}
    // ============================================================
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            var deleted = await _parametroService.DeleteAsync(id);

            if (!deleted)
                return NotFound(new { message = "No existe el parámetro" });

            return Ok(new { message = "Parámetro eliminado correctamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar parámetro {ParametroId}", id);
            return StatusCode(500, new { message = "Error al eliminar el parámetro", error = ex.Message });
        }
    }
}