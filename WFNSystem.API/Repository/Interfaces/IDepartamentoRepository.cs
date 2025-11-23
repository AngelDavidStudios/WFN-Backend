using WFNSystem.API.Models;

namespace WFNSystem.API.Repository.Interfaces;

public interface IDepartamentoRepository
{
    Task<IEnumerable<Departamento>> GetAllAsync();
    Task<Departamento?> GetByIdAsync(string deptoId);
    Task AddAsync(Departamento depto);
    Task UpdateAsync(Departamento depto);
    Task DeleteAsync(string deptoId);
}