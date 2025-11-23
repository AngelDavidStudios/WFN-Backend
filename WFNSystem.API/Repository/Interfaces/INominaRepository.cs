using WFNSystem.API.Models;

namespace WFNSystem.API.Repository.Interfaces;

public interface INominaRepository
{
    Task<Nomina?> GetNominaAsync(string empleadoId, string periodo);
    Task<IEnumerable<Nomina>> GetNominasByEmpleadoAsync(string empleadoId);
    Task<IEnumerable<Nomina>> GetNominasByPeriodoAsync(string periodo);

    Task AddAsync(Nomina nomina);
    Task UpdateAsync(Nomina nomina);
    Task DeleteAsync(string empleadoId, string periodo);
}