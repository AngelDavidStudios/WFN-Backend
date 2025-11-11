using Microsoft.AspNetCore.Mvc;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;

namespace WFNSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BankingController: ControllerBase
{
    private readonly IRepository<Banking> _bankingRepository;
    
    public BankingController(IRepository<Banking> bankingRepository)
    {
        _bankingRepository = bankingRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var bankings = await _bankingRepository.GetAllAsync();
        return Ok(bankings);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var banking = await _bankingRepository.GetByIdAsync(id);
        return Ok(banking);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Banking banking)
    {
        if (banking.ID_Banking != null)
        {
            banking.ID_Banking = null;
        }
        await _bankingRepository.AddAsync(banking);
        return Ok("Banking created successfully");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] Banking banking)
    {
        await _bankingRepository.UpdateAsync(id, banking);
        return Ok("Banking updated successfully");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _bankingRepository.DeleteAsync(id);
        return Ok("Banking deleted successfully");
    }
}