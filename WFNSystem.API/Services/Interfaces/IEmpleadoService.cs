using WFNSystem.API.Models;

namespace WFNSystem.API.Services.Interfaces;

public interface IEmpleadoService
{
    Task<IEnumerable<Empleado>> GetAllAsync();
    Task<Empleado?> GetByIdAsync(string empleadoId);

    Task<Empleado> CreateAsync(Empleado empleado);
    Task<Empleado> UpdateAsync(Empleado empleado);
    Task<bool> DeleteAsync(string empleadoId);

    // Empleado + Persona + Departamento + BankingAccounts
    Task<object?> GetEmpleadoFullAsync(string empleadoId);
}