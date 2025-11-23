using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Services;

public class WorkspaceService: IWorkspaceService
{
    private readonly IWorkspaceRepository _workspaceRepo;

    public WorkspaceService(IWorkspaceRepository workspaceRepo)
    {
        _workspaceRepo = workspaceRepo;
    }

    public async Task<IEnumerable<WorkspaceNomina>> GetAllAsync()
    {
        return await _workspaceRepo.GetAllAsync();
    }

    public async Task<WorkspaceNomina?> GetByPeriodoAsync(string periodo)
    {
        return await _workspaceRepo.GetByPeriodoAsync(periodo);
    }

    public async Task<WorkspaceNomina> CrearPeriodoAsync(string periodo)
    {
        // 1. Validar período no vacío
        if (string.IsNullOrWhiteSpace(periodo))
            throw new ArgumentException("El período es obligatorio (ej: 2025-03).");

        // 2. Verificar que no exista ya
        var existing = await _workspaceRepo.GetByPeriodoAsync(periodo);
        if (existing != null)
            throw new InvalidOperationException("El período ya existe. No se puede duplicar.");

        // 3. Crear workspace
        var workspace = new WorkspaceNomina
        {
            Periodo = periodo,
            FechaCreacion = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            FechaCierre = string.Empty,
            Estado = 0, // 0 = Abierto
            PK = $"WS#{periodo}",
            SK = "META#WS"
        };

        await _workspaceRepo.AddAsync(workspace);
        return workspace;
    }

    public async Task<WorkspaceNomina> CerrarPeriodoAsync(string periodo)
    {
        // 1. Verificar existencia
        var workspace = await _workspaceRepo.GetByPeriodoAsync(periodo);
        if (workspace == null)
            throw new KeyNotFoundException("El período que intenta cerrar no existe.");

        // 2. Verificar si ya está cerrado
        if (workspace.Estado == 1)
            throw new InvalidOperationException("El período ya está cerrado.");

        // 3. Cerrar el período
        workspace.Estado = 1;
        workspace.FechaCierre = DateTime.UtcNow.ToString("yyyy-MM-dd");

        await _workspaceRepo.UpdateAsync(workspace);
        return workspace;
    }

    public async Task<WorkspaceNomina> UpdateAsync(WorkspaceNomina workspace)
    {
        workspace.PK = $"WS#{workspace.Periodo}";
        workspace.SK = "META#WS";

        await _workspaceRepo.UpdateAsync(workspace);
        return workspace;
    }

    public async Task<bool> DeleteAsync(string periodo)
    {
        var existing = await _workspaceRepo.GetByPeriodoAsync(periodo);
        if (existing == null)
            return false;

        await _workspaceRepo.DeleteAsync(periodo);
        return true;
    }

    public async Task<bool> VerificarPeriodoAbiertoAsync(string periodo)
    {
        var workspace = await _workspaceRepo.GetByPeriodoAsync(periodo);

        if (workspace == null)
            return false;

        return workspace.Estado == 0; // 0 = abierto
    }
}