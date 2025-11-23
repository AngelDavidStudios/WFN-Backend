using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Services;

public class WorkspaceService: IWorkspaceService
{
    private readonly IWorkspaceRepository _repo;
    private readonly INominaRepository _nominaRepo;

    public WorkspaceService(IWorkspaceRepository repo, INominaRepository nominaRepo)
    {
        _repo = repo;
        _nominaRepo = nominaRepo;
    }

    public async Task<IEnumerable<WorkspaceNomina>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<WorkspaceNomina?> GetByPeriodoAsync(string periodo)
    {
        periodo = periodo.Trim().ToUpper();
        return await _repo.GetByPeriodoAsync(periodo);
    }

    public async Task<WorkspaceNomina> CrearPeriodoAsync(string periodo)
    {
        periodo = periodo.Trim().ToUpper(); // "2025-11"

        // Verificar si ya existe
        var existing = await _repo.GetByPeriodoAsync(periodo);
        if (existing != null)
            throw new Exception($"El período {periodo} ya existe.");

        var workspace = new WorkspaceNomina
        {
            PK = "WORKSPACE#GLOBAL",
            SK = $"WORK#{periodo}",
            ID_Workspace = Guid.NewGuid().ToString(),
            Periodo = periodo,
            FechaCreacion = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            FechaCierre = string.Empty,
            Estado = 0 // 0 = Abierto
        };

        await _repo.AddAsync(workspace);

        return workspace;
    }

    public async Task<WorkspaceNomina> CerrarPeriodoAsync(string periodo)
    {
        periodo = periodo.Trim().ToUpper();

        var workspace = await _repo.GetByPeriodoAsync(periodo);
        if (workspace == null)
            throw new Exception($"No existe el período {periodo}.");

        if (workspace.Estado == 1)
            throw new Exception("El período ya está cerrado.");

        // Validar que existan nóminas en el período antes de cerrarlo
        var nominas = await _nominaRepo.GetByPeriodoGlobalAsync(periodo);
        if (!nominas.Any())
            throw new Exception("No se puede cerrar un período sin nóminas generadas.");

        workspace.Estado = 1;
        workspace.FechaCierre = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

        await _repo.UpdateAsync(workspace);

        return workspace;
    }

    public async Task<WorkspaceNomina> UpdateAsync(WorkspaceNomina workspace)
    {
        // Normalizar clave
        workspace.PK = "WORKSPACE#GLOBAL";
        workspace.SK = $"WORK#{workspace.Periodo.Trim().ToUpper()}";

        await _repo.UpdateAsync(workspace);

        return workspace;
    }

    public async Task<bool> DeleteAsync(string periodo)
    {
        periodo = periodo.Trim().ToUpper();

        var existing = await _repo.GetByPeriodoAsync(periodo);
        if (existing == null)
            return false;

        // Seguridad: evitar borrar periodos cerrados
        if (existing.Estado == 1)
            throw new Exception("No se puede eliminar un período ya cerrado.");

        // También deberíamos verificar que no existan nóminas
        var nominas = await _nominaRepo.GetByPeriodoGlobalAsync(periodo);
        if (nominas.Any())
            throw new Exception("No se puede eliminar un período con nóminas generadas.");

        await _repo.DeleteAsync(periodo);
        return true;
    }

    public async Task<bool> VerificarPeriodoAbiertoAsync(string periodo)
    {
        periodo = periodo.Trim().ToUpper();

        var workspace = await _repo.GetByPeriodoAsync(periodo);
        if (workspace == null)
            return false;

        return workspace.Estado == 0;
    }
}