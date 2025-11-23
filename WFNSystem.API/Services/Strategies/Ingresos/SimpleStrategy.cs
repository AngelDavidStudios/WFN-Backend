using WFNSystem.API.Models;
using WFNSystem.API.Services.Strategies.Interfaces;

namespace WFNSystem.API.Services.Strategies.Ingresos;

public class SimpleStrategy: ICalculoStrategy
{
    public decimal Calcular(
        Novedad novedad,
        Empleado empleado,
        decimal salarioBase,
        decimal totalIngresosGravados,
        IDictionary<string, decimal> parametrosSistema)
    {
        // Simple: devuelve el monto ingresado sin modificar
        return novedad.MontoAplicado;
    }
}