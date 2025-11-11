using WFNSystem.API.Models;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Services.Strategies;

public class ProvisionDecimoCuartoAcumuladoStrategy: ICalculoProvisionStrategy
{
    private const decimal SalarioBasicoUnificado = 470;
    
    public decimal Calcular(Provision provision, Empleado empleado, Ingresos totalGravado)
    {
        // Salario Basico Unificado / 12 (si es acumulado)
        return SalarioBasicoUnificado / 12M;
    }
}