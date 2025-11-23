using WFNSystem.API.Models;
using WFNSystem.API.Services.Strategies.Interfaces;

namespace WFNSystem.API.Services.Strategies;

public class IngresoPorcentajeStrategy: IIngresoStrategy
{
    public decimal Calcular(Novedad novedad, Empleado empleado)
    {
        return Math.Round(empleado.SalarioBase * (novedad.MontoAplicado / 100m), 2);
    }
}