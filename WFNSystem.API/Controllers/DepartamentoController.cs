using Microsoft.AspNetCore.Mvc;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DepartamentoController: ControllerBase
{
    private readonly IDepartamentoService _departamentoService;

    public DepartamentoController(IDepartamentoService departamentoService)
    {
        _departamentoService = departamentoService;
    }
    
    // ============================================================
    // GET: api/departamento
    // ============================================================
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _departamentoService.GetAllAsync();
        return Ok(result);
    }

    // ============================================================
    // GET: api/departamento/{id}
    // ============================================================
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var dep = await _departamentoService.GetByIdAsync(id);
        if (dep == null)
            return NotFound("Departamento no encontrado.");

        return Ok(dep);
    }

    // ============================================================
    // POST: api/departamento
    // ============================================================
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Departamento dep)
    {
        if (dep == null)
            return BadRequest("Datos inválidos.");

        var created = await _departamentoService.CreateAsync(dep);
        return CreatedAtAction(nameof(GetById), new { id = created.ID_Departamento }, created);
    }

    // ============================================================
    // PUT: api/departamento/{id}
    // ============================================================
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] Departamento dep)
    {
        var exists = await _departamentoService.GetByIdAsync(id);
        if (exists == null)
            return NotFound("Departamento no encontrado.");

        dep.ID_Departamento = id;
        var updated = await _departamentoService.UpdateAsync(dep);

        return Ok(updated);
    }

    // ============================================================
    // DELETE: api/departamento/{id}
    // ============================================================
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _departamentoService.DeleteAsync(id);
        if (!deleted)
            return NotFound("No existe el departamento.");

        return Ok(new { message = "Departamento eliminado correctamente." });
    }
}