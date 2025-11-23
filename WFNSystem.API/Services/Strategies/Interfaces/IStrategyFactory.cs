namespace WFNSystem.API.Services.Strategies.Interfaces;

public interface IStrategyFactory
{
    IIngresoStrategy GetIngresoStrategy(string parametro);
    IEgresoStrategy GetEgresoStrategy(string parametro);
}