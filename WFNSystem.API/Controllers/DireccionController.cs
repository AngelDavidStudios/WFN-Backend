using Microsoft.AspNetCore.Mvc;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;

namespace WFNSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DireccionController: ControllerBase
{
    private readonly IDireccionRepository _direccionRepository;
    
    public DireccionController(IDireccionRepository direccionRepository)
    {
        _direccionRepository = direccionRepository;
    }

    [HttpGet("persona/{personaId}")]
    public async Task<IActionResult> GetAllDirecciones(string personaId)
    {
        var direcciones = await _direccionRepository.GetDireccionesByPersonaIdAsync(personaId);
        return Ok(direcciones);
    }

    [HttpGet("persona/{personaId}/direccion/{direccionId}")]
    public async Task<IActionResult> GetDireccionById(string personaId, string direccionId)
    {
        var direccion = await _direccionRepository.GetDireccionByIdAsync(personaId, direccionId);
        return Ok(direccion);
    }

    [HttpPost("persona/{personaId}")]
    public async Task<IActionResult> CreateDireccion(string personaId, [FromBody] Direccion direccion)
    {
        await _direccionRepository.AddDireccionAsync(personaId, direccion);
        return Ok("Direccion created successfully");
    }

    [HttpPut("persona/{personaId}/direccion/{direccionId}")]
    public async Task<IActionResult> UpdateDireccion(string personaId, string direccionId,
        [FromBody] Direccion direccion)
    {
        await _direccionRepository.UpdateDireccionAsync(personaId, direccionId, direccion);
        return Ok("Direccion updated successfully");
    }

    [HttpDelete("persona/{personaId}/direccion/{direccionId}")]
    public async Task<IActionResult> DeleteDireccion(string personaId, string direccionId)
    {
        await _direccionRepository.DeleteDireccionAsync(personaId, direccionId);
        return Ok("Direccion deleted successfully");
    }
}