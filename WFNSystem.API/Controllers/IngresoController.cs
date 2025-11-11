using Microsoft.AspNetCore.Mvc;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;

namespace WFNSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IngresoController: ControllerBase
{
    private readonly IRepository<Ingresos> _ingresoRepository;
    
    public IngresoController(IRepository<Ingresos> ingresoRepository)
    {
        _ingresoRepository = ingresoRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var ingresos = await _ingresoRepository.GetAllAsync();
        return Ok(ingresos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var ingreso = await _ingresoRepository.GetByIdAsync(id);
        return Ok(ingreso);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Ingresos ingreso)
    {
        if (ingreso.ID_Ingreso != null)
        {
            ingreso.ID_Ingreso = null;
        }

        await _ingresoRepository.AddAsync(ingreso);
        return Ok("Ingreso created successfully");
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] Ingresos ingreso)
    {
        await _ingresoRepository.UpdateAsync(id, ingreso);
        return Ok("Ingreso updated successfully");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _ingresoRepository.DeleteAsync(id);
        return Ok("Ingreso deleted successfully");
    }
}