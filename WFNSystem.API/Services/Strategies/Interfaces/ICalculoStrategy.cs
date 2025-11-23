using WFNSystem.API.Models;

namespace WFNSystem.API.Services.Strategies.Interfaces;

public interface ICalculoStrategy
{
    decimal Calcular(
        Novedad novedad,
        Empleado empleado,
        decimal salarioBase,
        decimal totalIngresosGravados,
        IDictionary<string, decimal> parametrosSistema
    );
}