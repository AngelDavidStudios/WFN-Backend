using WFNSystem.API.Models;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Services.Strategies;

public class DecimoTerceroStrategy: ICalculoDecimoStrategy
{
    public decimal CalcularDecimo(Novedad novedad, Empleado empleado, Ingresos SubtotalGravado)
    {
        return SubtotalGravado.SubTotal_Gravado_IESS / 12;
    }
}