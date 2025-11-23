using WFNSystem.API.Models;

namespace WFNSystem.API.Repository.Interfaces;

public interface INominaRepository
{
    // Obtener nómina de un empleado en un período específico
    Task<Nomina?> GetAsync(string periodo, string empleadoId);

    // Obtener todas las nóminas de un período (PK = NOMINA#YYYY-MM)
    Task<IEnumerable<Nomina>> GetByPeriodoAsync(string periodo);

    // Obtener todas las nóminas de un empleado (requiere GSI sobre SK)
    Task<IEnumerable<Nomina>> GetByEmpleadoAsync(string empleadoId);

    // Crear nómina
    Task AddAsync(Nomina nomina);

    // Actualizar nómina (override dynamodb)
    Task UpdateAsync(Nomina nomina);

    // Eliminar nómina
    Task DeleteAsync(string periodo, string empleadoId);
}