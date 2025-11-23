using WFNSystem.API.Models;

namespace WFNSystem.API.Services.Interfaces;

public interface IWorkspaceService
{
    Task<IEnumerable<WorkspaceNomina>> GetAllAsync();
    Task<WorkspaceNomina?> GetByPeriodoAsync(string periodo);

    Task<WorkspaceNomina> CrearPeriodoAsync(string periodo);
    Task<WorkspaceNomina> CerrarPeriodoAsync(string periodo);

    Task<WorkspaceNomina> UpdateAsync(WorkspaceNomina workspace);
    Task<bool> DeleteAsync(string periodo);

    Task<bool> VerificarPeriodoAbiertoAsync(string periodo);
}