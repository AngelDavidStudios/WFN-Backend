using WFNSystem.API.Models;
using WFNSystem.API.Services.Strategies.Interfaces;

namespace WFNSystem.API.Services.Strategies.Provisiones;

public class DecimoTerceroAcumuladoStrategy: IProvisionStrategy
{
    public decimal Calcular(
        Empleado empleado,
        decimal salarioBase,
        decimal totalIngresosGravados,
        Provision provisionActual)
    {
        if (empleado.Is_DecimoTercMensual)
            return provisionActual.Acumulado;

        decimal mensual = totalIngresosGravados / 12m;

        return Math.Round(provisionActual.Acumulado + mensual, 2);
    }
}