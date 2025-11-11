using WFNSystem.API.Models;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Services.Strategies;

public class FondosReservaStrategy: ICalculoDecimoStrategy
{
    public decimal CalcularDecimo(Novedad novedad, Empleado empleado, Ingresos SubtotalGravado)
    {
        // Total gravado * 8.33%
        return SubtotalGravado.SubTotal_Gravado_IESS * 0.0833M;
    }
}