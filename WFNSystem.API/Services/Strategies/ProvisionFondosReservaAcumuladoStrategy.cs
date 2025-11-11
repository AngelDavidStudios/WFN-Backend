using WFNSystem.API.Models;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Services.Strategies;

public class ProvisionFondosReservaAcumuladoStrategy: ICalculoProvisionStrategy
{
    public decimal Calcular(Provision provision, Empleado empleado, Ingresos totalGravado)
    {
        // 8.33% de Fondos de Reserva Acumulado
        return totalGravado.SubTotal_Gravado_IESS * 0.0833M;
    }
}