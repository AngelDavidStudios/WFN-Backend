using WFNSystem.API.Models;

namespace WFNSystem.API.Repository.Interfaces;

public interface IEmpleadoRepository
{
    Task<IEnumerable<Empleado>> GetAllAsync();
    Task<Empleado?> GetByIdAsync(string empleadoId);
    Task AddAsync(Empleado empleado);
    Task UpdateAsync(Empleado empleado);
    Task DeleteAsync(string empleadoId);

    // Extra: obtener empleado + persona + departamento
    Task<object?> GetEmpleadoFullAsync(string empleadoId);
}