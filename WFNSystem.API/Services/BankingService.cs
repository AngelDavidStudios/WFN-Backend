using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Services;

public class BankingService: IBankingService
{
    private readonly IBankingRepository _repo;
    private readonly IEmpleadoRepository _empleadoRepo;

    public BankingService(IBankingRepository repo, IEmpleadoRepository empleadoRepo)
    {
        _repo = repo;
        _empleadoRepo = empleadoRepo;
    }

    public async Task<IEnumerable<Banking>> GetByEmpleadoAsync(string empleadoId)
    {
        return await _repo.GetByEmpleadoAsync(empleadoId);
    }

    public async Task<Banking?> GetByIdAsync(string empleadoId, string bankingId)
    {
        return await _repo.GetByIdAsync(empleadoId, bankingId);
    }

    public async Task<Banking> CreateAsync(string empleadoId, Banking banking)
    {
        // Validar que el empleado exista
        var emp = await _empleadoRepo.GetByIdAsync(empleadoId);
        if (emp == null)
            throw new Exception("No se puede agregar cuenta bancaria: el empleado no existe.");

        // Crear nuevo ID
        banking.ID_Banking = Guid.NewGuid().ToString();

        // Construcción de claves
        banking.PK = $"EMP#{empleadoId}";
        banking.SK = $"BANK#{banking.ID_Banking}";

        await _repo.AddAsync(banking);
        return banking;
    }

    public async Task<Banking> UpdateAsync(string empleadoId, Banking banking)
    {
        // Validar empleado
        var emp = await _empleadoRepo.GetByIdAsync(empleadoId);
        if (emp == null)
            throw new Exception("El empleado no existe.");

        // Validar que la cuenta exista antes de actualizar
        var exists = await _repo.GetByIdAsync(empleadoId, banking.ID_Banking);
        if (exists == null)
            throw new Exception("La cuenta bancaria que intenta actualizar no existe.");

        // Mantener claves correctas
        banking.PK = $"EMP#{empleadoId}";
        banking.SK = $"BANK#{banking.ID_Banking}";

        await _repo.UpdateAsync(banking);
        return banking;
    }

    public async Task<bool> DeleteAsync(string empleadoId, string bankingId)
    {
        var exists = await _repo.GetByIdAsync(empleadoId, bankingId);
        if (exists == null)
            return false;

        await _repo.DeleteAsync(empleadoId, bankingId);
        return true;
    }
}