using Microsoft.AspNetCore.Mvc;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;

namespace WFNSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PersonaController: ControllerBase
{
    private readonly IRepository<Persona> _personaRepository;
    
    public PersonaController(IRepository<Persona> personaRepository)
    {
        _personaRepository = personaRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var personas = await _personaRepository.GetAllAsync();
        return Ok(personas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var persona = await _personaRepository.GetByIdAsync(id);
        return Ok(persona);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Persona persona)
    {
        if (persona.ID_Persona != null)
        {
            persona.ID_Persona = null;
        }

        await _personaRepository.AddAsync(persona);
        return Ok("Persona created successfully");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] Persona persona)
    {
        await _personaRepository.UpdateAsync(id, persona);
        return Ok("Persona updated successfully");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _personaRepository.DeleteAsync(id);
        return Ok("Persona deleted successfully");
    }
}