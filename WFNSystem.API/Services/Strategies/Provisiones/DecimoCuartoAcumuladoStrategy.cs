using WFNSystem.API.Models;
using WFNSystem.API.Services.Strategies.Interfaces;

namespace WFNSystem.API.Services.Strategies.Provisiones;

public class DecimoCuartoAcumuladoStrategy: IProvisionStrategy
{
    private const decimal SalarioCanastaBasica = 470m;

    public decimal Calcular(
        Empleado empleado,
        decimal salarioBase,
        decimal totalIngresosGravados,
        Provision provisionActual)
    {
        if (empleado.Is_DecimoCuartoMensual)
            return provisionActual.Acumulado;

        decimal mensual = SalarioCanastaBasica / 12m;

        return Math.Round(provisionActual.Acumulado + mensual, 2);
    }
}