using Microsoft.AspNetCore.Mvc;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;

namespace WFNSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NominaController: ControllerBase
{
    private readonly IRepository<Nomina> _nominarepository;
    
    public NominaController(IRepository<Nomina> nominarepository)
    {
        _nominarepository = nominarepository;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var nominas = await _nominarepository.GetAllAsync();
        return Ok(nominas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var nomina = await _nominarepository.GetByIdAsync(id);
        return Ok(nomina);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Nomina nomina)
    {
        if (nomina.ID_Nomina != null)
        {
            nomina.ID_Nomina = null;
        }

        await _nominarepository.AddAsync(nomina);
        return Ok("Nomina created successfully");
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] Nomina nomina)
    {
        await _nominarepository.UpdateAsync(id, nomina);
        return Ok("Nomina updated successfully");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _nominarepository.DeleteAsync(id);
        return Ok("Nomina deleted successfully");
    }
}