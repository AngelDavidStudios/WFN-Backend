using WFNSystem.API.Models;
using WFNSystem.API.Services.Strategies.Interfaces;

namespace WFNSystem.API.Services.Strategies.Provisiones;

public class DevengamientoVacacionesStrategy: IProvisionStrategy
{
    public decimal Calcular(
        Empleado empleado,
        decimal salarioBase,
        decimal totalIngresosGravados,
        Provision provisionActual)
    {
        // Se descuenta del acumulado
        return Math.Round(provisionActual.Acumulado - provisionActual.ValorMensual, 2);
    }
}