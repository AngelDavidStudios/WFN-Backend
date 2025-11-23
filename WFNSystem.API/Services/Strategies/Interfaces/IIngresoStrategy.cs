using WFNSystem.API.Models;

namespace WFNSystem.API.Services.Strategies.Interfaces;

public interface IIngresoStrategy
{
    decimal Calcular(Novedad novedad, Empleado empleado);
}