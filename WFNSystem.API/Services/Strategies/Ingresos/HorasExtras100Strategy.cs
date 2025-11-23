using WFNSystem.API.Models;
using WFNSystem.API.Services.Strategies.Interfaces;

namespace WFNSystem.API.Services.Strategies.Ingresos;

public class HorasExtras100Strategy: ICalculoStrategy
{
    public decimal Calcular(
        Novedad novedad,
        Empleado empleado,
        decimal salarioBase,
        decimal totalIngresosGravados,
        IDictionary<string, decimal> parametrosSistema)
    {
        // cantidad de horas al 100%
        decimal horas = novedad.MontoAplicado;

        if (horas <= 0 || salarioBase <= 0)
            return 0;

        // valor de una hora = salario / 30 días / 8 horas
        decimal valorHora = salarioBase / 30m / 8m;

        // pago al 100% = valorHora * 2.0
        decimal resultado = valorHora * 2.0m * horas;

        return Math.Round(resultado, 2);
    }
}