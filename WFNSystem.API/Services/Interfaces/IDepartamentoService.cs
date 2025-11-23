using WFNSystem.API.Models;

namespace WFNSystem.API.Services.Interfaces;

public interface IDepartamentoService
{
    Task<IEnumerable<Departamento>> GetAllAsync();
    Task<Departamento?> GetByIdAsync(string deptoId);

    Task<Departamento> CreateAsync(Departamento depto);
    Task<Departamento> UpdateAsync(Departamento depto);
    Task<bool> DeleteAsync(string deptoId);
}