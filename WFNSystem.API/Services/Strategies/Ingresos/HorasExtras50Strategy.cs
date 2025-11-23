using WFNSystem.API.Models;
using WFNSystem.API.Services.Strategies.Interfaces;

namespace WFNSystem.API.Services.Strategies.Ingresos;

public class HorasExtras50Strategy: ICalculoStrategy
{
    public decimal Calcular(
        Novedad novedad,
        Empleado empleado,
        decimal salarioBase,
        decimal totalIngresosGravados,
        IDictionary<string, decimal> parametrosSistema)
    {
        // cantidad de horas al 50%
        decimal horas = novedad.MontoAplicado;

        if (horas <= 0 || salarioBase <= 0)
            return 0;

        // valor de una hora = salario / 30 días / 8 horas
        decimal valorHora = salarioBase / 30m / 8m;

        // pago * 1.5 (50% adicional)
        decimal resultado = valorHora * 1.5m * horas;

        return Math.Round(resultado, 2);
    }
}