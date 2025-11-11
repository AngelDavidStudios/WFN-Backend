using WFNSystem.API.Models;

namespace WFNSystem.API.Repository.Interfaces;

public interface INominaRepository: IRepository<Nomina>
{
    Task<IEnumerable<Nomina>> GetByPeriodoAsync(string periodo);
    Task<IEnumerable<Nomina>> GetByEmpleadoAsync(string empleadoId);
}