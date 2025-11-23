using WFNSystem.API.Models;

namespace WFNSystem.API.Services.Strategies.Interfaces;

public interface IProvisionStrategy
{
    decimal Calcular(
        Empleado empleado,
        decimal salarioBase,
        decimal totalIngresosGravados,
        Provision provisionActual
    );
}