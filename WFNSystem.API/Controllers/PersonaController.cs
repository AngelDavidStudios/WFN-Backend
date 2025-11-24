using Microsoft.AspNetCore.Mvc;
using WFNSystem.API.Models;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PersonaController : ControllerBase
{
    private readonly IPersonaService _personaService;
    private readonly ILogger<PersonaController> _logger;

    public PersonaController(IPersonaService personaService, ILogger<PersonaController> logger)
    {
        _personaService = personaService;
        _logger = logger;
    }
    
    // ============================================================
    // GET: api/persona
    // ============================================================
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var personas = await _personaService.GetAllAsync();
            return Ok(personas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener personas");
            return StatusCode(500, new { message = "Error al obtener las personas", error = ex.Message });
        }
    }
    
    // ============================================================
    // GET: api/persona/{id}
    // ============================================================
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        try
        {
            var persona = await _personaService.GetByIdAsync(id);
            if (persona == null)
                return NotFound(new { message = "Persona no encontrada" });

            return Ok(persona);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener persona {PersonaId}", id);
            return StatusCode(500, new { message = "Error al obtener la persona", error = ex.Message });
        }
    }
    
    // ============================================================
    // POST: api/persona
    // ============================================================
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Persona persona)
    {
        try
        {
            if (persona == null)
                return BadRequest(new { message = "Datos inválidos" });

            var created = await _personaService.CreateAsync(persona);
            return CreatedAtAction(nameof(GetById), new { id = created.ID_Persona }, created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear persona");
            return StatusCode(500, new { message = "Error al crear la persona", error = ex.Message });
        }
    }
    
    // ============================================================
    // PUT: api/persona/{id}
    // ============================================================
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] Persona persona)
    {
        try
        {
            var exists = await _personaService.GetByIdAsync(id);
            if (exists == null)
                return NotFound(new { message = "Persona no encontrada" });

            persona.ID_Persona = id;
            var updated = await _personaService.UpdateAsync(persona);

            return Ok(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar persona {PersonaId}", id);
            return StatusCode(500, new { message = "Error al actualizar la persona", error = ex.Message });
        }
    }
    
    // ============================================================
    // DELETE: api/persona/{id}
    // ============================================================
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            var deleted = await _personaService.DeleteAsync(id);

            if (!deleted)
                return NotFound(new { message = "No existe la persona" });

            return Ok(new { message = "Persona eliminada correctamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar persona {PersonaId}", id);
            return StatusCode(500, new { message = "Error al eliminar la persona", error = ex.Message });
        }
    }
}