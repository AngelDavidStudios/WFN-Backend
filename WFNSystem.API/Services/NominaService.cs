using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;
using WFNSystem.API.Services.Interfaces;
using WFNSystem.API.Services.Strategies.Interfaces;

namespace WFNSystem.API.Services;

public class NominaService: INominaService
{
    private readonly INominaRepository _nominaRepo;
    private readonly INovedadRepository _novedadRepo;
    private readonly IEmpleadoRepository _empleadoRepo;
    private readonly IParametroRepository _parametroRepo;
    private readonly IProvisionRepository _provisionRepo;
    private readonly IWorkspaceRepository _workspaceRepo;
    
    private readonly IProvisionService _provisionService;
    private readonly IStrategyFactory _strategyFactory;
    
    private readonly ILogger<NominaService> _logger;
    
    public NominaService(
        INominaRepository nominaRepo,
        INovedadRepository novedadRepo,
        IEmpleadoRepository empleadoRepo,
        IParametroRepository parametroRepo,
        IProvisionRepository provisionRepo,
        IWorkspaceRepository workspaceRepo,
        IProvisionService provisionService,
        IStrategyFactory strategyFactory,
        ILogger<NominaService> logger)
    {
        _nominaRepo = nominaRepo;
        _novedadRepo = novedadRepo;
        _empleadoRepo = empleadoRepo;
        _parametroRepo = parametroRepo;
        _provisionRepo = provisionRepo;
        _workspaceRepo = workspaceRepo;

        _provisionService = provisionService;
        _strategyFactory = strategyFactory;

        _logger = logger;
    }
    
    public async Task<Nomina?> GetByPeriodoAsync(string empleadoId, string periodo)
    {
        if (string.IsNullOrWhiteSpace(empleadoId) || string.IsNullOrWhiteSpace(periodo))
            return null;

        return await _nominaRepo.GetByPeriodoAsync(empleadoId, periodo);
    }
    
    public async Task<IEnumerable<Nomina>> GetByEmpleadoAsync(string empleadoId)
    {
        if (string.IsNullOrWhiteSpace(empleadoId))
            return Enumerable.Empty<Nomina>();

        return await _nominaRepo.GetByEmpleadoAsync(empleadoId);
    }
    
    public async Task<IEnumerable<Nomina>> GetByPeriodoGlobalAsync(string periodo)
    {
        if (string.IsNullOrWhiteSpace(periodo))
            return Enumerable.Empty<Nomina>();

        return await _nominaRepo.GetByPeriodoGlobalAsync(periodo);
    }
    
    public async Task<bool> DeleteNominaAsync(string empleadoId, string periodo)
    {
        var exists = await _nominaRepo.GetByPeriodoAsync(empleadoId, periodo);
        if (exists == null)
            return false;

        await _nominaRepo.DeleteAsync(empleadoId, periodo);
        return true;
    }
    

}