using Microsoft.AspNetCore.Mvc;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProvisionController: ControllerBase
{
    private readonly IProvisionService _provisionService;

    public ProvisionController(IProvisionService provisionService)
    {
        _provisionService = provisionService;
    }

    // ============================================================
    // GET: api/provision/empleado/{empleadoId}
    // ============================================================
    [HttpGet("empleado/{empleadoId}")]
    public async Task<IActionResult> GetByEmpleado(string empleadoId)
    {
        var provisiones = await _provisionService.GetByEmpleadoAsync(empleadoId);
        return Ok(provisiones);
    }

    // ============================================================
    // GET: api/provision/empleado/{empleadoId}/periodo/{periodo}
    // ============================================================
    [HttpGet("empleado/{empleadoId}/periodo/{periodo}")]
    public async Task<IActionResult> GetByPeriodo(string empleadoId, string periodo)
    {
        var provisiones = await _provisionService.GetByPeriodoAsync(empleadoId, periodo);

        if (provisiones == null || !provisiones.Any())
            return NotFound("No existen provisiones para este período.");

        return Ok(provisiones);
    }

    // ============================================================
    // GET: api/provision/{provisionId}/empleado/{empleadoId}
    // ============================================================
    [HttpGet("{provisionId}/empleado/{empleadoId}")]
    public async Task<IActionResult> GetById(string empleadoId, string provisionId)
    {
        var prov = await _provisionService.GetByIdAsync(empleadoId, provisionId);

        if (prov == null)
            return NotFound("Provisión no encontrada.");

        return Ok(prov);
    }

    // ============================================================
    // POST: api/provision
    // BODY debe incluir: ID_Empleado + TipoProvision + Periodo + Valores
    // ============================================================
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Provision provision)
    {
        if (provision == null)
            return BadRequest("Datos inválidos.");

        if (string.IsNullOrWhiteSpace(provision.ID_Empleado))
            return BadRequest("ID_Empleado es obligatorio.");

        var created = await _provisionService.CreateAsync(provision);

        return CreatedAtAction(nameof(GetById),
            new { empleadoId = created.ID_Empleado, provisionId = created.ID_Provision },
            created);
    }

    // ============================================================
    // PUT: api/provision/{provisionId}
    // ============================================================
    [HttpPut("{provisionId}")]
    public async Task<IActionResult> Update(string provisionId, [FromBody] Provision provision)
    {
        if (string.IsNullOrWhiteSpace(provision.ID_Empleado))
            return BadRequest("ID_Empleado es obligatorio para actualizar.");

        provision.ID_Provision = provisionId;

        var updated = await _provisionService.UpdateAsync(provision);

        return Ok(updated);
    }

    // ============================================================
    // DELETE: api/provision/{provisionId}/empleado/{empleadoId}
    // ============================================================
    [HttpDelete("{provisionId}/empleado/{empleadoId}")]
    public async Task<IActionResult> Delete(string provisionId, string empleadoId)
    {
        var deleted = await _provisionService.DeleteAsync(empleadoId, provisionId);

        if (!deleted)
            return NotFound("La provisión no existe.");

        return Ok(new { message = "Provisión eliminada correctamente." });
    }

    // ============================================================
    // POST: api/provision/procesar/empleado/{empleadoId}/periodo/{periodo}
    // ============================================================
    [HttpPost("procesar/empleado/{empleadoId}/periodo/{periodo}")]
    public async Task<IActionResult> Procesar(string empleadoId, string periodo)
    {
        await _provisionService.ProcesarProvisionesAsync(empleadoId, periodo);
        return Ok(new { message = "Provisiones procesadas correctamente." });
    }
}