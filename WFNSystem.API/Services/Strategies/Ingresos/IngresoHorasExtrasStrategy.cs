using WFNSystem.API.Models;
using WFNSystem.API.Services.Strategies.Interfaces;

namespace WFNSystem.API.Services.Strategies;

public class IngresoHorasExtrasStrategy: IIngresoStrategy
{
    public decimal Calcular(Novedad novedad, Empleado empleado)
    {
        decimal valorHora = empleado.SalarioBase / 240m; // 240 horas mensuales estándar
        decimal multiplicador = novedad.ID_Parametro.Contains("100") ? 2.0m : 1.5m;
        return Math.Round(valorHora * multiplicador * novedad.MontoAplicado, 2);
    }
}