using WFNSystem.API.Models;

namespace WFNSystem.API.Repository.Interfaces;

public interface IDepartamentoRepository
{
    Task<Departamento?> GetByIdAsync(string departamentoId);
    Task<IEnumerable<Departamento>> GetAllAsync();

    Task AddAsync(Departamento departamento);
    Task UpdateAsync(Departamento departamento);
    Task DeleteAsync(string departamentoId);
}