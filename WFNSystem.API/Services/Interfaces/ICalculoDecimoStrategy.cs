using WFNSystem.API.Models;

namespace WFNSystem.API.Services.Interfaces;

public interface ICalculoDecimoStrategy
{
    decimal CalcularDecimo(Novedad novedad, Empleado empleado, Ingresos SubtotalGravado);
}