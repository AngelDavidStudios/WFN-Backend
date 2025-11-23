using Microsoft.AspNetCore.Mvc;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmpleadoController: ControllerBase
{
    private readonly IEmpleadoService _empleadoService;

    public EmpleadoController(IEmpleadoService empleadoService)
    {
        _empleadoService = empleadoService;
    }
    
    // ============================================================
    // GET: api/empleado
    // ============================================================
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var empleados = await _empleadoService.GetAllAsync();
        return Ok(empleados);
    }

    // ============================================================
    // GET: api/empleado/{id}
    // ============================================================
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var empleado = await _empleadoService.GetByIdAsync(id);
        if (empleado == null)
            return NotFound("Empleado no encontrado.");

        return Ok(empleado);
    }

    // ============================================================
    // POST: api/empleado
    // ============================================================
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Empleado empleado)
    {
        if (empleado == null)
            return BadRequest("Datos inválidos.");

        var created = await _empleadoService.CreateAsync(empleado);
        return CreatedAtAction(nameof(GetById),
            new { id = created.ID_Empleado },
            created);
    }

    // ============================================================
    // PUT: api/empleado/{id}
    // ============================================================
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] Empleado empleado)
    {
        var exists = await _empleadoService.GetByIdAsync(id);
        if (exists == null)
            return NotFound("Empleado no encontrado.");

        empleado.ID_Empleado = id;

        var updated = await _empleadoService.UpdateAsync(empleado);
        return Ok(updated);
    }

    // ============================================================
    // DELETE: api/empleado/{id}
    // ============================================================
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _empleadoService.DeleteAsync(id);

        if (!deleted) 
            return NotFound("No existe el empleado.");

        return Ok(new { message = "Empleado eliminado correctamente." });
    }
}