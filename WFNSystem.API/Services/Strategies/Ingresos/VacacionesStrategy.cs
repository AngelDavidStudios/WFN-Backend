using WFNSystem.API.Models;
using WFNSystem.API.Services.Strategies.Interfaces;

namespace WFNSystem.API.Services.Strategies.Ingresos;

public class VacacionesStrategy: ICalculoStrategy
{
    public decimal Calcular(
        Novedad novedad,
        Empleado empleado,
        decimal salarioBase,
        decimal totalIngresosGravados,
        IDictionary<string, decimal> parametrosSistema)
    {
        // Fórmula oficial:
        // TOTAL_INGRESOS_GRAVADOS_IESS / 24

        if (totalIngresosGravados <= 0)
            return 0;

        decimal resultado = totalIngresosGravados / 24m;

        return Math.Round(resultado, 2);
    }
}