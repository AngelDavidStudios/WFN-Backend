using WFNSystem.API.Models;

namespace WFNSystem.API.Repository.Interfaces;

public interface INovedadRepository
{
    Task<IEnumerable<Novedad>> GetNovedadesByEmpleadoAsync(string empleadoId);
    Task<IEnumerable<Novedad>> GetNovedadesByPeriodoAsync(string empleadoId, string periodo);

    Task<Novedad?> GetByIdAsync(string empleadoId, string novedadId);

    Task AddAsync(Novedad novedad);
    Task UpdateAsync(Novedad novedad);    
    Task DeleteAsync(string empleadoId, string novedadId);
}