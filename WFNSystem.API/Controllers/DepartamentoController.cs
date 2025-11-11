using Microsoft.AspNetCore.Mvc;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;

namespace WFNSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DepartamentoController: ControllerBase
{
    private readonly IRepository<Departamento> _departamentoRepository;
    
    public DepartamentoController(IRepository<Departamento> departamentoRepository)
    {
        _departamentoRepository = departamentoRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var departamentos = await _departamentoRepository.GetAllAsync();
        return Ok(departamentos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var departamento = await _departamentoRepository.GetByIdAsync(id);
        return Ok(departamento);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Departamento departamento)
    {
        if (departamento.ID_Departamento != null)
        {
            departamento.ID_Departamento = null;
        }

        await _departamentoRepository.AddAsync(departamento);
        return Ok("Departamento created successfully");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] Departamento departamento)
    {
        await _departamentoRepository.UpdateAsync(id, departamento);
        return Ok("Departamento updated successfully");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _departamentoRepository.DeleteAsync(id);
        return Ok("Departamento deleted successfully");
    }
}