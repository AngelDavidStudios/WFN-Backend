using WFNSystem.API.Models;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Services.Strategies;

public class ProvisionVacacionStrategy: ICalculoProvisionStrategy
{
    public decimal Calcular(Provision provision, Empleado empleado, Ingresos totalGravado)
    {
        // total gravado / 24 (equivale al 4.16%)
        return totalGravado.SubTotal_Gravado_IESS / 24M;
    }
}