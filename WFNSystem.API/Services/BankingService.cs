using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Services;

public class BankingService: IBankingService
{
    private readonly IBankingRepository _bankRepo;
    private readonly IEmpleadoRepository _empleadoRepo;

    public BankingService(
        IBankingRepository bankRepo,
        IEmpleadoRepository empleadoRepo)
    {
        _bankRepo = bankRepo;
        _empleadoRepo = empleadoRepo;
    }
    
    public async Task<IEnumerable<Banking>> GetByEmpleadoAsync(string empleadoId)
    {
        return await _bankRepo.GetBankingByEmpleadoIdAsync(empleadoId);
    }

    public async Task<Banking?> GetByIdAsync(string empleadoId, string bankingId)
    {
        return await _bankRepo.GetBankingByIdAsync(empleadoId, bankingId);
    }

    public async Task<Banking> CreateAsync(string empleadoId, Banking banking)
    {
        // 1. Validar existencia del empleado
        var empleado = await _empleadoRepo.GetByIdAsync(empleadoId);
        if (empleado == null)
            throw new ArgumentException("El empleado asociado no existe.");

        // 2. Generar el ID de la cuenta bancaria
        banking.ID_Banking = Guid.NewGuid().ToString();

        // 3. Asignar PK/SK correctos
        banking.PK = $"EMP#{empleadoId}";
        banking.SK = $"BANK#{banking.ID_Banking}";

        await _bankRepo.AddBankingAsync(banking);
        return banking;
    }

    public async Task<Banking> UpdateAsync(string empleadoId, Banking banking)
    {
        // 1. Validar existencia del empleado
        var empleado = await _empleadoRepo.GetByIdAsync(empleadoId);
        if (empleado == null)
            throw new ArgumentException("El empleado no existe.");

        // 2. Verificar que la cuenta exista realmente
        var existing = await _bankRepo.GetBankingByIdAsync(empleadoId, banking.ID_Banking);
        if (existing == null)
            throw new KeyNotFoundException("La cuenta bancaria no existe.");

        // 3. Reasignar PK/SK
        banking.PK = $"EMP#{empleadoId}";
        banking.SK = $"BANK#{banking.ID_Banking}";

        await _bankRepo.UpdateBankingAsync(banking);
        return banking;
    }

    public async Task<bool> DeleteAsync(string empleadoId, string bankingId)
    {
        var existing = await _bankRepo.GetBankingByIdAsync(empleadoId, bankingId);

        if (existing == null)
            return false;

        await _bankRepo.DeleteBankingAsync(empleadoId, bankingId);
        return true;
    }
}