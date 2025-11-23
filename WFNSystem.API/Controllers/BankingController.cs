using Microsoft.AspNetCore.Mvc;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BankingController: ControllerBase
{
    private readonly IBankingService _bankingService;

    public BankingController(IBankingService bankingService)
    {
        _bankingService = bankingService;
    }
    
    // ============================================================
    // GET: api/banking/empleado/{empleadoId}
    // ============================================================
    [HttpGet("empleado/{empleadoId}")]
    public async Task<IActionResult> GetByEmpleado(string empleadoId)
    {
        var cuentas = await _bankingService.GetByEmpleadoAsync(empleadoId);

        if (cuentas == null || !cuentas.Any())
            return NotFound("El empleado no tiene cuentas bancarias registradas.");

        return Ok(cuentas);
    }

    // ============================================================
    // POST: api/banking/empleado/{empleadoId}
    // ============================================================
    [HttpPost("empleado/{empleadoId}")]
    public async Task<IActionResult> Create(string empleadoId, [FromBody] Banking banking)
    {
        if (banking == null)
            return BadRequest("Datos inválidos.");

        var created = await _bankingService.CreateAsync(empleadoId, banking);

        return CreatedAtAction(nameof(GetByEmpleado), 
            new { empleadoId = empleadoId }, created);
    }

    // ============================================================
    // PUT: api/banking/{bankId}/empleado/{empleadoId}
    // ============================================================
    [HttpPut("{bankId}/empleado/{empleadoId}")]
    public async Task<IActionResult> Update(string bankId, string empleadoId, [FromBody] Banking banking)
    {
        var exists = await _bankingService.GetByIdAsync(empleadoId, bankId);
        if (exists == null)
            return NotFound("La cuenta bancaria no existe para este empleado.");

        banking.ID_Banking = bankId;

        var updated = await _bankingService.UpdateAsync(empleadoId, banking);
        return Ok(updated);
    }

    // ============================================================
    // DELETE: api/banking/{bankId}/empleado/{empleadoId}
    // ============================================================
    [HttpDelete("{bankId}/empleado/{empleadoId}")]
    public async Task<IActionResult> Delete(string bankId, string empleadoId)
    {
        var deleted = await _bankingService.DeleteAsync(empleadoId, bankId);

        if (!deleted)
            return NotFound("No existe la cuenta bancaria para este empleado.");

        return Ok(new { message = "Cuenta bancaria eliminada correctamente." });
    }
}