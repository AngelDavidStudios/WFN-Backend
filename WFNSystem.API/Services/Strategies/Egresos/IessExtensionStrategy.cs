using WFNSystem.API.Models;
using WFNSystem.API.Services.Strategies.Interfaces;

namespace WFNSystem.API.Services.Strategies.Egresos;

public class IessExtensionStrategy: ICalculoStrategy
{
    public decimal Calcular(
        Novedad novedad,
        Empleado empleado,
        decimal salarioBase,
        decimal totalIngresosGravados,
        IDictionary<string, decimal> parametrosSistema)
    {
        // 3.41% del salario base
        if (salarioBase <= 0)
            return 0;

        return Math.Round(salarioBase * 0.0341m, 2);
    }
}