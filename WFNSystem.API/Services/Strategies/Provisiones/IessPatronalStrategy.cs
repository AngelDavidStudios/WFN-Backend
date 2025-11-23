using WFNSystem.API.Models;
using WFNSystem.API.Services.Strategies.Interfaces;

namespace WFNSystem.API.Services.Strategies.Provisiones;

public class IessPatronalStrategy: IProvisionStrategy
{
    public decimal Calcular(
        Empleado empleado,
        decimal salarioBase,
        decimal totalIngresosGravados,
        Provision provisionActual)
    {
        if (totalIngresosGravados <= 0)
            return 0;

        return Math.Round(totalIngresosGravados * 0.1215m, 2);
    }
}