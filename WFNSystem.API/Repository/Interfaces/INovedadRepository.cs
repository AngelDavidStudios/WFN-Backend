using WFNSystem.API.Models;

namespace WFNSystem.API.Repository.Interfaces;

public interface INovedadRepository
{
    Task<IEnumerable<Novedad>> GetByEmpleadoAsync(string empleadoId);
    Task<IEnumerable<Novedad>> GetByPeriodoAsync(string empleadoId, string periodo);
    Task<Novedad?> GetByIdAsync(string empleadoId, string novedadId, string periodo);

    Task AddAsync(Novedad novedad);
    Task UpdateAsync(Novedad novedad);
    Task DeleteAsync(string empleadoId, string periodo, string novedadId);
}