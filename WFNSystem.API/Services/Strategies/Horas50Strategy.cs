using WFNSystem.API.Models;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Services.Strategies;

public class Horas50Strategy: ICalculoNovedadStrategy
{
    public decimal Calcular(Novedad novedad, Empleado empleado)
    {
        decimal valorA = empleado.SalarioBase / 30 / 8;
        decimal valorB = (valorA) * 50 / 100;
        return valorA + valorB * novedad.MontoAplicado;
    }
}