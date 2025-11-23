using WFNSystem.API.Models;
using WFNSystem.API.Services.Strategies.Interfaces;

namespace WFNSystem.API.Services.Strategies.Provisiones;

public class ProvisionVacacionesStrategy:IProvisionStrategy
{
    public decimal Calcular(
        Empleado empleado,
        decimal salarioBase,
        decimal totalIngresosGravados,
        Provision provisionActual)
    {
        if (totalIngresosGravados <= 0)
            return 0;

        decimal mensual = totalIngresosGravados / 24m;

        return Math.Round(mensual, 2);
    }
}