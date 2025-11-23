using WFNSystem.API.Models;

namespace WFNSystem.API.Services.Interfaces;

public interface INominaService
{
    Task<Nomina?> GetByPeriodoAsync(string empleadoId, string periodo);
    Task<IEnumerable<Nomina>> GetByEmpleadoAsync(string empleadoId);
    Task<IEnumerable<Nomina>> GetByPeriodoGlobalAsync(string periodo);

    Task<Nomina> GenerarNominaAsync(string empleadoId, string periodo);
    Task<Nomina> RecalcularNominaAsync(string empleadoId, string periodo);
    Task<bool> DeleteNominaAsync(string empleadoId, string periodo);
}