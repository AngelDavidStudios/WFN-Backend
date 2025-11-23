using WFNSystem.API.Services.Strategies.Interfaces;
using WFNSystem.API.Services.Strategies.Provisiones;

namespace WFNSystem.API.Services.Strategies;

public class ProvisionStrategyFactory
{
    private readonly Dictionary<string, IProvisionStrategy> _strategies;

    public ProvisionStrategyFactory()
    {
        _strategies = new Dictionary<string, IProvisionStrategy>(StringComparer.OrdinalIgnoreCase)
        {
            { "PROVISION_VACACIONES", new ProvisionVacacionesStrategy() },
            { "IESS_PATRONAL", new IessPatronalStrategy() },
            { "FONDOS_RESERVA_ACUMULADO", new FondoReservaAcumuladoStrategy() },
            { "DECIMO_TERCERO_ACUMULADO", new DecimoTerceroAcumuladoStrategy() },
            { "DECIMO_CUARTO_ACUMULADO", new DecimoCuartoAcumuladoStrategy() },
            { "DEVENGAMIENTO_VACACIONES", new DevengamientoVacacionesStrategy() }
        };
    }
    
    public IProvisionStrategy GetStrategy(string provisionType)
    {
        if (_strategies.TryGetValue(provisionType, out var strategy))
        {
            return strategy;
        }

        throw new ArgumentException($"No strategy found for provision type: {provisionType}");
    }
}