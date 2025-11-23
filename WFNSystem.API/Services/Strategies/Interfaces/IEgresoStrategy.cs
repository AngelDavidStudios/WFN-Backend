using WFNSystem.API.Models;

namespace WFNSystem.API.Services.Strategies.Interfaces;

public interface IEgresoStrategy
{
    decimal Calcular(Novedad novedad, Empleado empleado, decimal ingresosGravados);
}