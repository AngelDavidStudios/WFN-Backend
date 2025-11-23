using Microsoft.AspNetCore.Mvc;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PersonaController: ControllerBase
{
    private readonly IPersonaService _personaService;

    public PersonaController(IPersonaService personaService)
    {
        _personaService = personaService;
    }
    
    // ============================================================
    // GET: api/persona
    // ============================================================
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var personas = await _personaService.GetAllAsync();
        return Ok(personas);
    }
    
    // ============================================================
    // GET: api/persona/{id}
    // ============================================================
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var persona = await _personaService.GetByIdAsync(id);
        if (persona == null) return NotFound("Persona no encontrada.");

        return Ok(persona);
    }
    
    // ============================================================
    // POST: api/persona
    // ============================================================
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Persona persona)
    {
        if (persona == null)
            return BadRequest("Datos inválidos.");

        var created = await _personaService.CreateAsync(persona);
        return CreatedAtAction(nameof(GetById), new { id = created.ID_Persona }, created);
    }
    
    // ============================================================
    // PUT: api/persona/{id}
    // ============================================================
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] Persona persona)
    {
        var exists = await _personaService.GetByIdAsync(id);
        if (exists == null) return NotFound("Persona no encontrada.");

        persona.ID_Persona = id;
        var updated = await _personaService.UpdateAsync(persona);

        return Ok(updated);
    }
    
    // ============================================================
    // DELETE: api/persona/{id}
    // ============================================================
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _personaService.DeleteAsync(id);

        if (!deleted) return NotFound("No existe la persona.");

        return Ok(new { message = "Persona eliminada correctamente." });
    }
}