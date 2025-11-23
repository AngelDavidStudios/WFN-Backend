using WFNSystem.API.Models;
using WFNSystem.API.Services.Strategies.Interfaces;

namespace WFNSystem.API.Services.Strategies.Ingresos;

public class DecimoTerceroStrategy: ICalculoStrategy
{
    public decimal Calcular(
        Novedad novedad,
        Empleado empleado,
        decimal salarioBase,
        decimal totalIngresosGravados,
        IDictionary<string, decimal> parametrosSistema)
    {
        if (totalIngresosGravados <= 0)
            return 0;

        // Fórmula oficial
        decimal resultado = totalIngresosGravados / 12m;

        return Math.Round(resultado, 2);
    }
}