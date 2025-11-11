using WFNSystem.API.Models;

namespace WFNSystem.API.Services.Interfaces;

public interface ICalculoProvisionStrategy
{
    decimal Calcular(Provision provision, Empleado empleado, decimal totalGravado);
}