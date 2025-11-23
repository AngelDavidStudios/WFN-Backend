using WFNSystem.API.Models;
using WFNSystem.API.Services.Strategies.Interfaces;

namespace WFNSystem.API.Services.Strategies.Egresos;

public class EgresoPorcentajeStrategy: IEgresoStrategy
{
    public decimal Calcular(Novedad novedad, Empleado empleado, decimal ingresosGravados)
    {
        return Math.Round(ingresosGravados * (novedad.MontoAplicado / 100m), 2);
    }
}