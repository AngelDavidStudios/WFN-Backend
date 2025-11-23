using WFNSystem.API.Models;
using WFNSystem.API.Services.Strategies.Interfaces;

namespace WFNSystem.API.Services.Strategies.Egresos;

public class PrestamoStrategy: ICalculoStrategy
{
    public decimal Calcular(
        Novedad novedad,
        Empleado empleado,
        decimal salarioBase,
        decimal totalIngresosGravados,
        IDictionary<string, decimal> parametrosSistema)
    {
        // El préstamo siempre es monto manual
        return Math.Round(novedad.MontoAplicado, 2);
    }
}