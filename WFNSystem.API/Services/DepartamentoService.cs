using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Services;

public class DepartamentoService: IDepartamentoService
{
    private readonly IDepartamentoRepository _departamentoRepo;

    public DepartamentoService(IDepartamentoRepository departamentoRepo)
    {
        _departamentoRepo = departamentoRepo;
    }

    public async Task<IEnumerable<Departamento>> GetAllAsync()
    {
        return await _departamentoRepo.GetAllAsync();
    }

    public async Task<Departamento?> GetByIdAsync(string deptoId)
    {
        return await _departamentoRepo.GetByIdAsync(deptoId);
    }

    public async Task<Departamento> CreateAsync(Departamento depto)
    {
        // Validaciones mínimas (puedes agregar más después)
        if (string.IsNullOrWhiteSpace(depto.Nombre))
            throw new ArgumentException("El nombre del departamento es obligatorio.");

        if (string.IsNullOrWhiteSpace(depto.Email))
            throw new ArgumentException("El email del departamento es obligatorio.");

        // Generar ID desde backend
        depto.ID_Departamento = Guid.NewGuid().ToString();

        // Asignar PK/SK
        depto.PK = $"DEP#{depto.ID_Departamento}";
        depto.SK = "META#DEP";

        await _departamentoRepo.AddAsync(depto);
        return depto;
    }

    public async Task<Departamento> UpdateAsync(Departamento depto)
    {
        var existing = await _departamentoRepo.GetByIdAsync(depto.ID_Departamento);

        if (existing == null)
            throw new KeyNotFoundException("El departamento no existe.");

        depto.PK = $"DEP#{depto.ID_Departamento}";
        depto.SK = "META#DEP";

        await _departamentoRepo.UpdateAsync(depto);
        return depto;
    }

    public async Task<bool> DeleteAsync(string deptoId)
    {
        var existing = await _departamentoRepo.GetByIdAsync(deptoId);

        if (existing == null)
            return false;

        await _departamentoRepo.DeleteAsync(deptoId);
        return true;
    }
}