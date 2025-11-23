using WFNSystem.API.Models;

namespace WFNSystem.API.Services.Interfaces;

public interface INovedadService
{
    Task<IEnumerable<Novedad>> GetByEmpleadoAsync(string empleadoId);
    Task<IEnumerable<Novedad>> GetByPeriodoAsync(string empleadoId, string periodo);

    Task<Novedad?> GetByIdAsync(string empleadoId, string periodo, string novedadId);

    Task<Novedad> CreateAsync(string empleadoId, string periodo, Novedad novedad);
    Task<Novedad> UpdateAsync(string empleadoId, string periodo, Novedad novedad);
    Task<bool> DeleteAsync(string empleadoId, string periodo, string novedadId);
}