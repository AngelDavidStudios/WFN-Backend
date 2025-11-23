using WFNSystem.API.Models;

namespace WFNSystem.API.Repository.Interfaces;

public interface IWorkspaceRepository
{
    Task<IEnumerable<WorkspaceNomina>> GetAllAsync();
    Task<WorkspaceNomina?> GetByPeriodoAsync(string periodo);

    Task AddAsync(WorkspaceNomina workspace);
    Task UpdateAsync(WorkspaceNomina workspace);
    Task DeleteAsync(string periodo);
}