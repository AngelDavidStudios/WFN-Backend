using WFNSystem.API.Models;

namespace WFNSystem.API.Repository.Interfaces;

public interface INominaRepository
{
    Task<Nomina?> GetByPeriodoAsync(string empleadoId, string periodo);
    Task<IEnumerable<Nomina>> GetByEmpleadoAsync(string empleadoId);
    Task<IEnumerable<Nomina>> GetByPeriodoGlobalAsync(string periodo);

    Task AddAsync(Nomina nomina);
    Task UpdateAsync(Nomina nomina);
    Task DeleteAsync(string empleadoId, string periodo);
}