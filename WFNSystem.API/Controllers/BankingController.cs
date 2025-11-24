using Microsoft.AspNetCore.Mvc;
using WFNSystem.API.Models;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BankingController : ControllerBase
{
    private readonly IBankingService _bankingService;
    private readonly ILogger<BankingController> _logger;

    public BankingController(IBankingService bankingService, ILogger<BankingController> logger)
    {
        _bankingService = bankingService;
        _logger = logger;
    }
    
    // ============================================================
    // GET: api/banking/empleado/{empleadoId}
    // ============================================================
    [HttpGet("empleado/{empleadoId}")]
    public async Task<IActionResult> GetByEmpleado(string empleadoId)
    {
        try
        {
            var cuentas = await _bankingService.GetByEmpleadoAsync(empleadoId);

            if (cuentas == null || !cuentas.Any())
                return NotFound(new { message = "El empleado no tiene cuentas bancarias registradas" });

            return Ok(cuentas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener cuentas bancarias del empleado {EmpleadoId}", empleadoId);
            return StatusCode(500, new { message = "Error al obtener las cuentas bancarias", error = ex.Message });
        }
    }

    // ============================================================
    // POST: api/banking/empleado/{empleadoId}
    // ============================================================
    [HttpPost("empleado/{empleadoId}")]
    public async Task<IActionResult> Create(string empleadoId, [FromBody] Banking banking)
    {
        try
        {
            if (banking == null)
                return BadRequest(new { message = "Datos inválidos" });

            var created = await _bankingService.CreateAsync(empleadoId, banking);

            return CreatedAtAction(nameof(GetByEmpleado), 
                new { empleadoId = empleadoId }, created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear cuenta bancaria para empleado {EmpleadoId}", empleadoId);
            return StatusCode(500, new { message = "Error al crear la cuenta bancaria", error = ex.Message });
        }
    }

    // ============================================================
    // PUT: api/banking/{bankId}/empleado/{empleadoId}
    // ============================================================
    [HttpPut("{bankId}/empleado/{empleadoId}")]
    public async Task<IActionResult> Update(string bankId, string empleadoId, [FromBody] Banking banking)
    {
        try
        {
            var exists = await _bankingService.GetByIdAsync(empleadoId, bankId);
            if (exists == null)
                return NotFound(new { message = "La cuenta bancaria no existe para este empleado" });

            banking.ID_Banking = bankId;

            var updated = await _bankingService.UpdateAsync(empleadoId, banking);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar cuenta bancaria {BankId} del empleado {EmpleadoId}", bankId, empleadoId);
            return StatusCode(500, new { message = "Error al actualizar la cuenta bancaria", error = ex.Message });
        }
    }

    // ============================================================
    // DELETE: api/banking/{bankId}/empleado/{empleadoId}
    // ============================================================
    [HttpDelete("{bankId}/empleado/{empleadoId}")]
    public async Task<IActionResult> Delete(string bankId, string empleadoId)
    {
        try
        {
            var deleted = await _bankingService.DeleteAsync(empleadoId, bankId);

            if (!deleted)
                return NotFound(new { message = "No existe la cuenta bancaria para este empleado" });

            return Ok(new { message = "Cuenta bancaria eliminada correctamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar cuenta bancaria {BankId} del empleado {EmpleadoId}", bankId, empleadoId);
            return StatusCode(500, new { message = "Error al eliminar la cuenta bancaria", error = ex.Message });
        }
    }
}