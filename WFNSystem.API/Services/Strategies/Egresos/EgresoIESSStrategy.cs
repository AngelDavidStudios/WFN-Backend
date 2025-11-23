using WFNSystem.API.Models;
using WFNSystem.API.Services.Strategies.Interfaces;

namespace WFNSystem.API.Services.Strategies.Egresos;

public class EgresoIESSStrategy: IEgresoStrategy
{
    public decimal Calcular(Novedad novedad, Empleado empleado, decimal ingresosGravados)
    {
        return Math.Round(ingresosGravados * 0.0945m, 2);
    }
}