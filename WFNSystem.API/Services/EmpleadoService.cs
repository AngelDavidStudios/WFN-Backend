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
        // Validaciones de negocio
        ValidarEmpleado(empleado);

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

        // Validar y ajustar configuración de Fondo de Reserva
        AjustarFondoReserva(empleado);

        // Fecha ISO
        empleado.DateCreated = DateTime.UtcNow.ToString("yyyy-MM-dd");

        await _repo.AddAsync(empleado);
        return empleado;
    }

    public async Task<Empleado> UpdateAsync(Empleado empleado)
    {
        // Validar existencia
        var exists = await _repo.GetByIdAsync(empleado.ID_Empleado);
        if (exists == null)
            throw new Exception("El empleado que intenta actualizar no existe.");

        // Validaciones de negocio
        ValidarEmpleado(empleado);

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

        // Validar y ajustar configuración de Fondo de Reserva
        AjustarFondoReserva(empleado);

        // Preservar fecha de creación original
        empleado.DateCreated = exists.DateCreated;

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

    // ============================================================
    // MÉTODOS PRIVADOS DE VALIDACIÓN
    // ============================================================

    private void ValidarEmpleado(Empleado empleado)
    {
        // Validar salario base
        if (empleado.SalarioBase <= 0)
            throw new ArgumentException("El salario base debe ser mayor a cero.");

        if (empleado.SalarioBase < 460) // SBU Ecuador 2025
            throw new ArgumentException("El salario base no puede ser menor al SBU ($460).");

        // Validar fecha de ingreso
        if (empleado.FechaIngreso == default)
            throw new ArgumentException("La fecha de ingreso es requerida.");

        if (empleado.FechaIngreso > DateTime.UtcNow)
            throw new ArgumentException("La fecha de ingreso no puede ser futura.");

        // Validar referencias
        if (string.IsNullOrWhiteSpace(empleado.ID_Persona))
            throw new ArgumentException("El ID de la persona es requerido.");

        if (string.IsNullOrWhiteSpace(empleado.ID_Departamento))
            throw new ArgumentException("El ID del departamento es requerido.");
    }

    private void AjustarFondoReserva(Empleado empleado)
    {
        // Calcular antigüedad
        var antiguedad = DateTime.UtcNow - empleado.FechaIngreso;
        var antiguedadAnios = antiguedad.TotalDays / 365.25;

        // Fondos de Reserva solo aplica después de 1 año
        if (antiguedadAnios < 1)
        {
            empleado.Is_FondoReserva = false;
        }
        // Si tiene más de 1 año y no está configurado, mantener el valor del usuario
    }
}