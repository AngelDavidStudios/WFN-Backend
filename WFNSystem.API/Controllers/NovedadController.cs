using Microsoft.AspNetCore.Mvc;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;

namespace WFNSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NovedadController: ControllerBase
{
    private readonly IRepository<Novedad> _novedadRepository;
    
    public NovedadController(IRepository<Novedad> novedadRepository)
    {
        _novedadRepository = novedadRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var novedades = await _novedadRepository.GetAllAsync();
        return Ok(novedades);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var novedad = await _novedadRepository.GetByIdAsync(id);
        return Ok(novedad);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Novedad novedad)
    {
        if (novedad.ID_Novedad != null)
        {
            novedad.ID_Novedad = null;
        }

        await _novedadRepository.AddAsync(novedad);
        return Ok("Novedad created successfully");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] Novedad novedad)
    {
        await _novedadRepository.UpdateAsync(id, novedad);
        return Ok("Novedad updated successfully");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _novedadRepository.DeleteAsync(id);
        return Ok("Novedad deleted successfully");
    }
}