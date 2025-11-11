using WFNSystem.API.Models;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Services.Strategies;

public class DecimoCuartoStrategy : ICalculoDecimoStrategy
{
    private const decimal SalarioBasicoUnificado = 470;
    
    public decimal CalcularDecimo(Novedad novedad, Empleado empleado, Ingresos SubtotalGravado)
    {
        return SalarioBasicoUnificado / 12;
    }

}