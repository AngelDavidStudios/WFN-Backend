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
        periodo = ValidarYNormalizarPeriodo(periodo);
        return await _repo.GetByPeriodoAsync(periodo);
    }

    public async Task<WorkspaceNomina> CrearPeriodoAsync(string periodo)
    {
        // Validar y normalizar periodo
        periodo = ValidarYNormalizarPeriodo(periodo);

        // Verificar si ya existe
        var existing = await _repo.GetByPeriodoAsync(periodo);
        if (existing != null)
            throw new ArgumentException($"El período {periodo} ya existe.");

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
        periodo = ValidarYNormalizarPeriodo(periodo);

        var workspace = await _repo.GetByPeriodoAsync(periodo);
        if (workspace == null)
            throw new ArgumentException($"No existe el período {periodo}.");

        if (workspace.Estado == 1)
            throw new ArgumentException("El período ya está cerrado.");

        // Validar que existan nóminas en el período antes de cerrarlo
        var nominas = await _nominaRepo.GetByPeriodoGlobalAsync(periodo);
        if (!nominas.Any())
            throw new ArgumentException("No se puede cerrar un período sin nóminas generadas.");

        workspace.Estado = 1;
        workspace.FechaCierre = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

        await _repo.UpdateAsync(workspace);

        return workspace;
    }

    public async Task<WorkspaceNomina> UpdateAsync(WorkspaceNomina workspace)
    {
        // Validar periodo
        workspace.Periodo = ValidarYNormalizarPeriodo(workspace.Periodo);

        // Normalizar clave
        workspace.PK = "WORKSPACE#GLOBAL";
        workspace.SK = $"WORK#{workspace.Periodo}";

        await _repo.UpdateAsync(workspace);

        return workspace;
    }

    public async Task<bool> DeleteAsync(string periodo)
    {
        periodo = ValidarYNormalizarPeriodo(periodo);

        var existing = await _repo.GetByPeriodoAsync(periodo);
        if (existing == null)
            return false;

        // Seguridad: evitar borrar periodos cerrados
        if (existing.Estado == 1)
            throw new ArgumentException("No se puede eliminar un período ya cerrado.");

        // También deberíamos verificar que no existan nóminas
        var nominas = await _nominaRepo.GetByPeriodoGlobalAsync(periodo);
        if (nominas.Any())
            throw new ArgumentException("No se puede eliminar un período con nóminas generadas.");

        await _repo.DeleteAsync(periodo);
        return true;
    }

    public async Task<bool> VerificarPeriodoAbiertoAsync(string periodo)
    {
        periodo = ValidarYNormalizarPeriodo(periodo);

        var workspace = await _repo.GetByPeriodoAsync(periodo);
        if (workspace == null)
            return false;

        return workspace.Estado == 0;
    }

    // ============================================================
    // MÉTODOS PRIVADOS DE VALIDACIÓN
    // ============================================================

    private string ValidarYNormalizarPeriodo(string periodo)
    {
        if (string.IsNullOrWhiteSpace(periodo))
            throw new ArgumentException("El periodo es requerido.");

        // Normalizar
        periodo = periodo.Trim().ToUpper();

        // Validar formato YYYY-MM
        if (!System.Text.RegularExpressions.Regex.IsMatch(periodo, @"^\d{4}-\d{2}$"))
            throw new ArgumentException("El periodo debe tener el formato YYYY-MM (ejemplo: 2025-11).");

        // Validar que sea un periodo válido (mes entre 01 y 12)
        var partes = periodo.Split('-');
        if (partes.Length != 2)
            throw new ArgumentException("El periodo debe tener el formato YYYY-MM.");

        if (!int.TryParse(partes[1], out int mes) || mes < 1 || mes > 12)
            throw new ArgumentException("El mes debe estar entre 01 y 12.");

        return periodo;
    }
}