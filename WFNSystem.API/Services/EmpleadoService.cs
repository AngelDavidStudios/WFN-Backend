using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Services;

public class EmpleadoService: IEmpleadoService
{
    private readonly IEmpleadoRepository _repo;
    private readonly IPersonaRepository _personaRepo;
    private readonly IDepartamentoRepository _departamentoRepo;

    public EmpleadoService(
        IEmpleadoRepository repo,
        IPersonaRepository personaRepo,
        IDepartamentoRepository departamentoRepo)
    {
        _repo = repo;
        _personaRepo = personaRepo;
        _departamentoRepo = departamentoRepo;
    }

    public async Task<IEnumerable<Empleado>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<Empleado?> GetByIdAsync(string empleadoId)
    {
        return await _repo.GetByIdAsync(empleadoId);
    }

    public async Task<Empleado> CreateAsync(Empleado empleado)
    {
        // Validar que Persona exista
        var persona = await _personaRepo.GetByIdAsync(empleado.ID_Persona);
        if (persona == null)
            throw new Exception("La persona asociada al empleado no existe.");

        // Validar que Departamento exista
        var dep = await _departamentoRepo.GetByIdAsync(empleado.ID_Departamento);
        if (dep == null)
            throw new Exception("El departamento asociado al empleado no existe.");

        // Crear nuevo ID
        empleado.ID_Empleado = Guid.NewGuid().ToString();

        // Construcción de claves
        empleado.PK = $"EMP#{empleado.ID_Empleado}";
        empleado.SK = "META#EMP";

        // Fecha ISO
        empleado.DateCreated = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

        await _repo.AddAsync(empleado);
        return empleado;
    }

    public async Task<Empleado> UpdateAsync(Empleado empleado)
    {
        // Validar existencia
        var exists = await _repo.GetByIdAsync(empleado.ID_Empleado);
        if (exists == null)
            throw new Exception("El empleado que intenta actualizar no existe.");

        // Validar Persona
        var persona = await _personaRepo.GetByIdAsync(empleado.ID_Persona);
        if (persona == null)
            throw new Exception("La persona asociada al empleado no existe.");

        // Validar Departamento
        var dep = await _departamentoRepo.GetByIdAsync(empleado.ID_Departamento);
        if (dep == null)
            throw new Exception("El departamento asociado al empleado no existe.");

        // Mantener claves correctas
        empleado.PK = $"EMP#{empleado.ID_Empleado}";
        empleado.SK = "META#EMP";

        await _repo.UpdateAsync(empleado);
        return empleado;
    }

    public async Task<bool> DeleteAsync(string empleadoId)
    {
        var exists = await _repo.GetByIdAsync(empleadoId);
        if (exists == null)
            return false;

        await _repo.DeleteAsync(empleadoId);
        return true;
    }
}