using WFNSystem.API.Models;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Services.Strategies;

public class VariableStrategy: ICalculoNovedadStrategy
{
    public decimal Calcular(Novedad novedad, Empleado empleado)
    {
        return novedad.MontoAplicado;
    }
}