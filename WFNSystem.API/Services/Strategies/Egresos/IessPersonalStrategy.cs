using WFNSystem.API.Models;
using WFNSystem.API.Services.Strategies.Interfaces;

namespace WFNSystem.API.Services.Strategies.Egresos;

public class IessPersonalStrategy: ICalculoStrategy
{
    public decimal Calcular(
        Novedad novedad,
        Empleado empleado,
        decimal salarioBase,
        decimal totalIngresosGravados,
        IDictionary<string, decimal> parametrosSistema)
    {
        // 9.45% de TOTAL INGRESOS GRAVADOS
        if (totalIngresosGravados <= 0)
            return 0;

        return Math.Round(totalIngresosGravados * 0.0945m, 2);
    }
}