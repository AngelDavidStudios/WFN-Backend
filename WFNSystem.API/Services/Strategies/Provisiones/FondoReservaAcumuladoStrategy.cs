using WFNSystem.API.Models;
using WFNSystem.API.Services.Strategies.Interfaces;

namespace WFNSystem.API.Services.Strategies.Provisiones;

public class FondoReservaAcumuladoStrategy: IProvisionStrategy
{
    public decimal Calcular(
        Empleado empleado,
        decimal salarioBase,
        decimal totalIngresosGravados,
        Provision provisionActual)
    {
        // Si es mensualizado, no se acumula
        if (empleado.Is_FondoReserva)
            return provisionActual.Acumulado; 

        decimal mensual = totalIngresosGravados * 0.0833m;

        decimal nuevoAcumulado = provisionActual.Acumulado + mensual;

        return Math.Round(nuevoAcumulado, 2);
    }
}