using WFNSystem.API.Models;

namespace WFNSystem.API.Services.Interfaces;

public interface INominaService
{
    Task<IEnumerable<Nomina>> ObtenerNominasPorPeriodoAsync(string periodo);
    Task<Nomina?> ObtenerNominaPorEmpleadoAsync(string empleadoId, string periodo);
    Task CrearNominaAsync(Nomina nomina);
    Task ActualizarNominaAsync(Nomina nomina);
    Task CerrarNominaAsync(string empleadoId, string periodo);
}