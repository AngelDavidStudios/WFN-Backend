using Microsoft.AspNetCore.Mvc;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ParametroController: ControllerBase
{
    private readonly IParametroService _parametroService;

    public ParametroController(IParametroService parametroService)
    {
        _parametroService = parametroService;
    }
    
    // ============================================================
    // GET: api/parametro
    // ============================================================
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var parametros = await _parametroService.GetAllAsync();
        return Ok(parametros);
    }

    // ============================================================
    // GET: api/parametro/{id}
    // ============================================================
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var parametro = await _parametroService.GetByIdAsync(id);
        if (parametro == null)
            return NotFound("Parámetro no encontrado.");

        return Ok(parametro);
    }

    // ============================================================
    // POST: api/parametro
    // ============================================================
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Parametro parametro)
    {
        if (parametro == null)
            return BadRequest("Datos inválidos.");

        var created = await _parametroService.CreateAsync(parametro);

        return CreatedAtAction(nameof(GetById),
            new { id = created.ID_Parametro }, created);
    }

    // ============================================================
    // PUT: api/parametro/{id}
    // ============================================================
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] Parametro parametro)
    {
        var exists = await _parametroService.GetByIdAsync(id);

        if (exists == null)
            return NotFound("Parámetro no encontrado.");

        parametro.ID_Parametro = id;

        var updated = await _parametroService.UpdateAsync(parametro);
        return Ok(updated);
    }

    // ============================================================
    // DELETE: api/parametro/{id}
    // ============================================================
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _parametroService.DeleteAsync(id);

        if (!deleted)
            return NotFound("No existe el parámetro.");

        return Ok(new { message = "Parámetro eliminado correctamente." });
    }
}