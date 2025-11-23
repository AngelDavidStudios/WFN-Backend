using WFNSystem.API.Models;

namespace WFNSystem.API.Services.Interfaces;

public interface IDepartamentoService
{
    Task<IEnumerable<Departamento>> GetAllAsync();
    Task<Departamento?> GetByIdAsync(string departamentoId);

    Task<Departamento> CreateAsync(Departamento departamento);
    Task<Departamento> UpdateAsync(Departamento departamento);
    Task<bool> DeleteAsync(string departamentoId);
}