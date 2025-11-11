using WFNSystem.API.Models;

namespace WFNSystem.API.Services.Interfaces;

public interface ICalculoNovedadStrategy
{
    decimal Calcular(Novedad novedad, Empleado empleado);
}