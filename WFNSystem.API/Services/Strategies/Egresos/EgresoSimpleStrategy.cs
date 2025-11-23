using WFNSystem.API.Models;
using WFNSystem.API.Services.Strategies.Interfaces;

namespace WFNSystem.API.Services.Strategies.Egresos;

public class EgresoSimpleStrategy: IEgresoStrategy
{
    public decimal Calcular(Novedad novedad, Empleado empleado, decimal ingresosGravados)
    {
        return novedad.MontoAplicado;
    }
}