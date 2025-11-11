using WFNSystem.API.Models;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Services.Strategies;

public class AporteIESSStrategy: ICalculoDecimoStrategy
{
    public decimal CalcularDecimo(Novedad novedad, Empleado empleado, Ingresos SubtotalGravado)
    {
        // Total gravado * 9.45%
        return SubtotalGravado.SubTotal_Gravado_IESS * 0.0945M;
    }
}