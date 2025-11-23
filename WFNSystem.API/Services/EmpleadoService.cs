using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Services;

public class EmpleadoService: IEmpleadoService
{
    private readonly IEmpleadoRepository _empleadoRepo;
    private readonly IPersonaRepository _personaRepo;
    private readonly IDepartamentoRepository _deptoRepo;
    
    public EmpleadoService(
        IEmpleadoRepository empleadoRepo,
        IPersonaRepository personaRepo,
        IDepartamentoRepository deptoRepo)
    {
        _empleadoRepo = empleadoRepo;
        _personaRepo = personaRepo;
        _deptoRepo = deptoRepo;
    }

    public async Task<IEnumerable<Empleado>> GetAllAsync()
    {
        return await _empleadoRepo.GetAllAsync();
    }

    public async Task<Empleado?> GetByIdAsync(string empleadoId)
    {
        return await _empleadoRepo.GetByIdAsync(empleadoId);
    }

    public async Task<Empleado> CreateAsync(Empleado empleado)
    {
        // 1. Validar Persona
        var persona = await _personaRepo.GetByIdAsync(empleado.ID_Persona);
        if (persona == null)
            throw new ArgumentException("La persona asociada al empleado no existe.");

        // 2. Validar Departamento
        var depto = await _deptoRepo.GetByIdAsync(empleado.ID_Departamento);
        if (depto == null)
            throw new ArgumentException("El departamento asociado no existe.");

        // 3. Generar ID Empleado
        empleado.ID_Empleado = Guid.NewGuid().ToString();

        // 4. Asignar PK/SK
        empleado.PK = $"EMP#{empleado.ID_Empleado}";
        empleado.SK = "META#EMP";

        // 5. Fecha creación
        empleado.DateCreated = DateTime.UtcNow.ToString("yyyy-MM-dd");

        // 6. Validar defaults de estado laboral
        if (empleado.StatusLaboral == null)
            empleado.StatusLaboral = StatusLaboral.Active;

        await _empleadoRepo.AddAsync(empleado);
        return empleado;
    }

    public async Task<Empleado> UpdateAsync(Empleado empleado)
    {
        var existing = await _empleadoRepo.GetByIdAsync(empleado.ID_Empleado);
        if (existing == null)
            throw new KeyNotFoundException("El empleado no existe.");

        // Validar persona si se cambia
        var persona = await _personaRepo.GetByIdAsync(empleado.ID_Persona);
        if (persona == null)
            throw new ArgumentException("La persona asociada no existe.");

        // Validar departamento si se cambia
        var depto = await _deptoRepo.GetByIdAsync(empleado.ID_Departamento);
        if (depto == null)
            throw new ArgumentException("El departamento asociado no existe.");

        empleado.PK = $"EMP#{empleado.ID_Empleado}";
        empleado.SK = "META#EMP";

        await _empleadoRepo.UpdateAsync(empleado);
        return empleado;
    }

    public async Task<bool> DeleteAsync(string empleadoId)
    {
        var existing = await _empleadoRepo.GetByIdAsync(empleadoId);
        if (existing == null)
            return false;

        await _empleadoRepo.DeleteAsync(empleadoId);
        return true;
    }

    public async Task<object?> GetEmpleadoFullAsync(string empleadoId)
    {
        return await _empleadoRepo.GetEmpleadoFullAsync(empleadoId);
    }
}