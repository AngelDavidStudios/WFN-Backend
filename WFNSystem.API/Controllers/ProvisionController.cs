using Microsoft.AspNetCore.Mvc;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;

namespace WFNSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProvisionController: ControllerBase
{
    private readonly IRepository<Provision> _provisionRepository;
    
    public ProvisionController(IRepository<Provision> provisionRepository)
    {
        _provisionRepository = provisionRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var provisions = await _provisionRepository.GetAllAsync();
        return Ok(provisions);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var provision = await _provisionRepository.GetByIdAsync(id);
        return Ok(provision);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Provision provision)
    {
        if (provision.ID_Provision != null)
        {
            provision.ID_Provision = null;
        }

        await _provisionRepository.AddAsync(provision);
        return Ok("Provision created successfully");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] Provision provision)
    {
        await _provisionRepository.UpdateAsync(id, provision);
        return Ok("Provision updated successfully");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _provisionRepository.DeleteAsync(id);
        return Ok("Provision deleted successfully");
    }
}