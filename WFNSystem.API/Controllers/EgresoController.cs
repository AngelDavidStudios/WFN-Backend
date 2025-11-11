using Microsoft.AspNetCore.Mvc;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;

namespace WFNSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EgresoController: ControllerBase
{
    private readonly IRepository<Egresos> _egresoRepository;
    
    public EgresoController(IRepository<Egresos> egresoRepository)
    {
        _egresoRepository = egresoRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var egresos = await _egresoRepository.GetAllAsync();
        return Ok(egresos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var egreso = await _egresoRepository.GetByIdAsync(id);
        return Ok(egreso);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Egresos egreso)
    {
        if (egreso.ID_Egreso != null)
        {
            egreso.ID_Egreso = null;
        }

        await _egresoRepository.AddAsync(egreso);
        return Ok("Egreso created successfully");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] Egresos egresos)
    {
        await _egresoRepository.UpdateAsync(id, egresos);
        return Ok("Egreso updated successfully");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _egresoRepository.DeleteAsync(id);
        return Ok("Egreso deleted successfully");
    }
}