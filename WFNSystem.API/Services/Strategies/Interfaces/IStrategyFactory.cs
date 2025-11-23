namespace WFNSystem.API.Services.Strategies.Interfaces;

public interface IStrategyFactory
{
    ICalculoStrategy GetIngresoStrategy(string tipoParametro);
    ICalculoStrategy GetEgresoStrategy(string tipoParametro);
    IProvisionStrategy GetProvisionStrategy(string tipoProvision);
}