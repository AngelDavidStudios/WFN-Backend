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

    public async Task<Departamento> CreateAsync(Departamento departamento)
    {
        // Crear ID único
        departamento.ID_Departamento = Guid.NewGuid().ToString();

        // Construcción de claves
        departamento.PK = $"DEP#{departamento.ID_Departamento}";
        departamento.SK = "META#DEP";

        await _departamentoRepo.AddAsync(departamento);
        return departamento;
    }

    public async Task<Departamento> UpdateAsync(Departamento departamento)
    {
        // Verificar si existe
        var exists = await _departamentoRepo.GetByIdAsync(departamento.ID_Departamento);
        if (exists == null)
            throw new Exception("El departamento que intentas actualizar no existe.");

        // Mantener claves correctas
        departamento.PK = $"DEP#{departamento.ID_Departamento}";
        departamento.SK = "META#DEP";

        await _departamentoRepo.UpdateAsync(departamento);
        return departamento;
    }

    public async Task<bool> DeleteAsync(string departamentoId)
    {
        var exists = await _departamentoRepo.GetByIdAsync(departamentoId);
        if (exists == null)
            return false;

        await _departamentoRepo.DeleteAsync(departamentoId);
        return true;
    }
}