using WFNSystem.API.Models;
using WFNSystem.API.Services.Strategies.Interfaces;

namespace WFNSystem.API.Services.Strategies;

public class IngresoProvisionMensualStrategy: IIngresoStrategy
{
    public decimal Calcular(Novedad novedad, Empleado empleado)
    {
        return novedad.MontoAplicado;
    }
}