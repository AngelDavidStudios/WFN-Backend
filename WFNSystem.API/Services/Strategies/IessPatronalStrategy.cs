using WFNSystem.API.Models;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Services.Strategies;

public class IessPatronalStrategy: ICalculoProvisionStrategy
{
    public decimal Calcular(Provision provision, Empleado empleado, Ingresos totalGravado)
    {
        // 12.15% de IESS Patronal
        return totalGravado.SubTotal_Gravado_IESS * 0.1215M;
    }
}