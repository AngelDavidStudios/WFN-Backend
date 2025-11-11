using WFNSystem.API.Models;

namespace WFNSystem.API.Services.Interfaces;

public interface ICalculoNominaService
{
    Task<Nomina> GenerarNominaAsync(string empleadoId, string periodo);
    Task<decimal> CalcularNovedadAsync(string tipo, Novedad novedad, Empleado empleado);
    Task<decimal> CalcularDecimoAsync(string tipo, Novedad novedad, Empleado empleado, Ingresos totalGravado);
    Task<decimal> CalcularProvisionAsync(string tipo, Provision provision, Empleado empleado, Ingresos totalGravado);
}