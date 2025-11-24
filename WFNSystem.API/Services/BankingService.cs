using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Services;

public class BankingService: IBankingService
{
    private readonly IBankingRepository _repo;
    private readonly IPersonaRepository _personaRepo;

    public BankingService(IBankingRepository repo, IPersonaRepository personaRepo)
    {
        _repo = repo;
        _personaRepo = personaRepo;
    }

    public async Task<IEnumerable<Banking>> GetByPersonaAsync(string personaId)
    {
        return await _repo.GetByPersonaAsync(personaId);
    }

    public async Task<Banking?> GetByIdAsync(string personaId, string bankingId)
    {
        return await _repo.GetByIdAsync(personaId, bankingId);
    }

    public async Task<Banking> CreateAsync(string personaId, Banking banking)
    {
        // Validar que la persona exista
        var persona = await _personaRepo.GetByIdAsync(personaId);
        if (persona == null)
            throw new Exception("No se puede agregar cuenta bancaria: la persona no existe.");

        // Validaciones de negocio
        ValidarCuentaBancaria(banking);

        // Crear nuevo ID
        banking.ID_Banking = Guid.NewGuid().ToString();

        // Construcción de claves
        banking.PK = $"PERSONA#{personaId}";
        banking.SK = $"BANK#{banking.ID_Banking}";

        // Fecha de creación
        banking.DateCreated = DateTime.UtcNow;

        await _repo.AddAsync(banking);
        return banking;
    }

    public async Task<Banking> UpdateAsync(string personaId, Banking banking)
    {
        // Validar persona
        var persona = await _personaRepo.GetByIdAsync(personaId);
        if (persona == null)
            throw new Exception("La persona no existe.");

        // Validar que la cuenta exista antes de actualizar
        var exists = await _repo.GetByIdAsync(personaId, banking.ID_Banking);
        if (exists == null)
            throw new Exception("La cuenta bancaria que intenta actualizar no existe.");

        // Validaciones de negocio
        ValidarCuentaBancaria(banking);

        // Mantener claves correctas
        banking.PK = $"PERSONA#{personaId}";
        banking.SK = $"BANK#{banking.ID_Banking}";

        // Preservar fecha de creación
        banking.DateCreated = exists.DateCreated;

        await _repo.UpdateAsync(banking);
        return banking;
    }

    public async Task<bool> DeleteAsync(string personaId, string bankingId)
    {
        var exists = await _repo.GetByIdAsync(personaId, bankingId);
        if (exists == null)
            return false;

        await _repo.DeleteAsync(personaId, bankingId);
        return true;
    }

    // ============================================================
    // MÉTODOS PRIVADOS DE VALIDACIÓN
    // ============================================================

    private void ValidarCuentaBancaria(Banking banking)
    {
        // Validar nombre del banco
        if (string.IsNullOrWhiteSpace(banking.BankName))
            throw new ArgumentException("El nombre del banco es requerido.");

        // Validar número de cuenta
        if (string.IsNullOrWhiteSpace(banking.AccountNumber))
            throw new ArgumentException("El número de cuenta es requerido.");

        if (banking.AccountNumber.Length < 5)
            throw new ArgumentException("El número de cuenta debe tener al menos 5 caracteres.");

        // Validar tipo de cuenta
        if (!string.IsNullOrWhiteSpace(banking.AccountType))
        {
            var tipoNormalizado = banking.AccountType.ToUpper();
            if (tipoNormalizado != "AHORRO" && tipoNormalizado != "CORRIENTE" && tipoNormalizado != "SAVING" && tipoNormalizado != "CHECKING")
                throw new ArgumentException("El tipo de cuenta debe ser 'AHORRO', 'CORRIENTE', 'SAVING' o 'CHECKING'.");
        }

        // Validar país
        if (string.IsNullOrWhiteSpace(banking.Pais))
            throw new ArgumentException("El país es requerido.");
    }
}