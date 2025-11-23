using WFNSystem.API.Models;
using WFNSystem.API.Services.Strategies.Interfaces;

namespace WFNSystem.API.Services.Strategies.Ingresos;

public class DecimoCuartoStrategy: ICalculoStrategy
{
    public decimal Calcular(
        Novedad novedad,
        Empleado empleado,
        decimal salarioBase,
        decimal totalIngresosGravados,
        IDictionary<string, decimal> parametrosSistema)
    {
        if (!parametrosSistema.TryGetValue("DECIMO_CUARTO_BASE", out decimal baseCuarto))
            throw new Exception("Falta parámetro DECIMO_CUARTO_BASE en el sistema.");

        decimal resultado = baseCuarto / 12m;

        return Math.Round(resultado, 2);
    }
}