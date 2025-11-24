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
    // GET: api/banking/persona/{personaId}
    // ============================================================
    [HttpGet("persona/{personaId}")]
    public async Task<IActionResult> GetByPersona(string personaId)
    {
        try
        {
            var cuentas = await _bankingService.GetByPersonaAsync(personaId);

            if (cuentas == null || !cuentas.Any())
                return NotFound(new { message = "La persona no tiene cuentas bancarias registradas" });

            return Ok(cuentas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener cuentas bancarias de la persona {PersonaId}", personaId);
            return StatusCode(500, new { message = "Error al obtener las cuentas bancarias", error = ex.Message });
        }
    }

    // ============================================================
    // GET: api/banking/persona/{personaId}/{bankId}
    // ============================================================
    [HttpGet("persona/{personaId}/{bankId}")]
    public async Task<IActionResult> GetById(string personaId, string bankId)
    {
        try
        {
            var cuenta = await _bankingService.GetByIdAsync(personaId, bankId);

            if (cuenta == null)
                return NotFound(new { message = "Cuenta bancaria no encontrada" });

            return Ok(cuenta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener cuenta bancaria {BankId} de la persona {PersonaId}", bankId, personaId);
            return StatusCode(500, new { message = "Error al obtener la cuenta bancaria", error = ex.Message });
        }
    }

    // ============================================================
    // POST: api/banking/persona/{personaId}
    // ============================================================
    [HttpPost("persona/{personaId}")]
    public async Task<IActionResult> Create(string personaId, [FromBody] Banking banking)
    {
        try
        {
            if (banking == null)
                return BadRequest(new { message = "Datos inválidos" });

            var created = await _bankingService.CreateAsync(personaId, banking);

            return CreatedAtAction(nameof(GetById), 
                new { personaId, bankId = created.ID_Banking }, created);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validación fallida al crear cuenta bancaria para persona {PersonaId}", personaId);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear cuenta bancaria para persona {PersonaId}", personaId);
            return StatusCode(500, new { message = "Error al crear la cuenta bancaria", error = ex.Message });
        }
    }

    // ============================================================
    // PUT: api/banking/persona/{personaId}/{bankId}
    // ============================================================
    [HttpPut("persona/{personaId}/{bankId}")]
    public async Task<IActionResult> Update(string personaId, string bankId, [FromBody] Banking banking)
    {
        try
        {
            var exists = await _bankingService.GetByIdAsync(personaId, bankId);
            if (exists == null)
                return NotFound(new { message = "La cuenta bancaria no existe para esta persona" });

            banking.ID_Banking = bankId;

            var updated = await _bankingService.UpdateAsync(personaId, banking);
            return Ok(updated);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validación fallida al actualizar cuenta bancaria {BankId} de la persona {PersonaId}", bankId, personaId);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar cuenta bancaria {BankId} de la persona {PersonaId}", bankId, personaId);
            return StatusCode(500, new { message = "Error al actualizar la cuenta bancaria", error = ex.Message });
        }
    }

    // ============================================================
    // DELETE: api/banking/persona/{personaId}/{bankId}
    // ============================================================
    [HttpDelete("persona/{personaId}/{bankId}")]
    public async Task<IActionResult> Delete(string personaId, string bankId)
    {
        try
        {
            var deleted = await _bankingService.DeleteAsync(personaId, bankId);

            if (!deleted)
                return NotFound(new { message = "No existe la cuenta bancaria para esta persona" });

            return Ok(new { message = "Cuenta bancaria eliminada correctamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar cuenta bancaria {BankId} de la persona {PersonaId}", bankId, personaId);
            return StatusCode(500, new { message = "Error al eliminar la cuenta bancaria", error = ex.Message });
        }
    }
}