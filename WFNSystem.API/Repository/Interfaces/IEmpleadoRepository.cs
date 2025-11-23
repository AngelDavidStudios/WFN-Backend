using WFNSystem.API.Models;

namespace WFNSystem.API.Repository.Interfaces;

public interface IEmpleadoRepository
{
    Task<Empleado?> GetByIdAsync(string empleadoId);
    Task<IEnumerable<Empleado>> GetAllAsync();
    Task<IEnumerable<Empleado>> GetByDepartamentoAsync(string departamentoId);

    Task AddAsync(Empleado empleado);
    Task UpdateAsync(Empleado empleado);
    Task DeleteAsync(string empleadoId);
}