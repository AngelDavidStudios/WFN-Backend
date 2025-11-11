using Microsoft.AspNetCore.Mvc;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;

namespace WFNSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmpleadoController: ControllerBase
{
    private readonly IRepository<Empleado> _empleadoRepository;
    
    public EmpleadoController(IRepository<Empleado> empleadoRepository)
    {
        _empleadoRepository = empleadoRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var empleados = await _empleadoRepository.GetAllAsync();
        return Ok(empleados);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var empleado = await _empleadoRepository.GetByIdAsync(id);
        return Ok(empleado);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Empleado empleado)
    {
        if (empleado.ID_Empleado != null)
        {
            empleado.ID_Empleado = null;
        }

        await _empleadoRepository.AddAsync(empleado);
        return Ok("Empleado created successfully");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] Empleado empleado)
    {
        await _empleadoRepository.UpdateAsync(id, empleado);
        return Ok("Empleado updated successfully");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _empleadoRepository.DeleteAsync(id);
        return Ok("Empleado deleted successfully");
    }
}