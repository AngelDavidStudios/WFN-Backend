using WFNSystem.API.Models;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Services.Strategies;

public class ProvisionDecimoTerceroAcumuladoStrategy: ICalculoProvisionStrategy
{
    public decimal Calcular(Provision provision, Empleado empleado, Ingresos totalGravado)
    {
        // total gravado / 12 (si es acumulado)
        return totalGravado.SubTotal_Gravado_IESS / 12M;
    }
}