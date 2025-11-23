using WFNSystem.API.Services.Strategies.Egresos;
using WFNSystem.API.Services.Strategies.Interfaces;

namespace WFNSystem.API.Services.Strategies;

public class StrategyFactory: IStrategyFactory
{
    private readonly IngresoStrategyFactory _ingresoFactory;
    private readonly EgresoStrategyFactory _egresoFactory;
    private readonly ProvisionStrategyFactory _provisionFactory;
    
    
    public StrategyFactory()
    {
        _ingresoFactory = new IngresoStrategyFactory();
        _egresoFactory  = new EgresoStrategyFactory();
        _provisionFactory = new ProvisionStrategyFactory();
    }
    
    public ICalculoStrategy GetIngresoStrategy(string tipoParametro) => 
        _ingresoFactory.GetStrategy(tipoParametro);
    
    public ICalculoStrategy GetEgresoStrategy(string tipoParametro) => 
        _egresoFactory.GetStrategy(tipoParametro);
    
    public IProvisionStrategy GetProvisionStrategy(string tipoProvision) => 
        _provisionFactory.GetStrategy(tipoProvision);
}