using WFNSystem.API.Models;

namespace WFNSystem.API.Services.Interfaces;

public interface INominaService
{
    Task<Nomina?> GetNominaAsync(string empleadoId, string periodo);
    Task<IEnumerable<Nomina>> GetNominasByEmpleadoAsync(string empleadoId);
    Task<IEnumerable<Nomina>> GetNominasByPeriodoAsync(string periodo);

    // Genera desde cero
    Task<Nomina> GenerarNominaAsync(string empleadoId, string periodo);

    // Vuelve a calcular si hay cambios
    Task<Nomina> RecalcularNominaAsync(string empleadoId, string periodo);

    // Eliminación estricta del período
    Task<bool> DeleteNominaAsync(string empleadoId, string periodo);
}