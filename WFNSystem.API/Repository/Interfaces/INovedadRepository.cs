using WFNSystem.API.Models;

namespace WFNSystem.API.Repository.Interfaces;

public interface INovedadRepository: IRepository<Novedad>
{
    Task<IEnumerable<Novedad>> GetByEmpleadoAsync(string empleadoId);
    Task<IEnumerable<Novedad>> GetByTipoAsync(string tipo); // "Ingreso" o "Egreso"
}