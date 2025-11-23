using WFNSystem.API.Services.Strategies.Egresos;
using WFNSystem.API.Services.Strategies.Interfaces;

namespace WFNSystem.API.Services.Strategies;

public class StrategyFactory: IStrategyFactory
{
    private readonly IngresoStrategyFactory _ingresoFactory;
    private readonly EgresoStrategyFactory _egresoFactory;
    
    
    public StrategyFactory()
    {
        _ingresoFactory = new IngresoStrategyFactory();
        _egresoFactory  = new EgresoStrategyFactory();
    }
    
    public IIngresoStrategy GetIngresoStrategy(string parametroDescripcion)
    {
        return _ingresoFactory.GetStrategy(parametroDescripcion);
    }

    public IEgresoStrategy GetEgresoStrategy(string parametroDescripcion)
    {
        return _egresoFactory.GetStrategy(parametroDescripcion);
    }
}