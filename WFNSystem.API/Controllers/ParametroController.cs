using Microsoft.AspNetCore.Mvc;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;

namespace WFNSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ParametroController: ControllerBase
{
    private readonly IRepository<Parametro> _parametroRepository;
    
    public ParametroController(IRepository<Parametro> parametroRepository)
    {
        _parametroRepository = parametroRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var parametros = await _parametroRepository.GetAllAsync();
        return Ok(parametros);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var parametro = await _parametroRepository.GetByIdAsync(id);
        return Ok(parametro);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Parametro parametro)
    {
        if (parametro.ID_Parametro != null)
        {
            parametro.ID_Parametro = null;
        }

        await _parametroRepository.AddAsync(parametro);
        return Ok("Parametro created successfully");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] Parametro parametro)
    {
        await _parametroRepository.UpdateAsync(id, parametro);
        return Ok("Parametro updated successfully");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _parametroRepository.DeleteAsync(id);
        return Ok("Parametro deleted successfully");
    }
}