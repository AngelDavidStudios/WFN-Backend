using WFNSystem.API.Services.Strategies.Ingresos;
using WFNSystem.API.Services.Strategies.Interfaces;

namespace WFNSystem.API.Services.Strategies;

public class IngresoStrategyFactory
{
    private readonly Dictionary<string, ICalculoStrategy> _strategies;

    public IngresoStrategyFactory()
    {
        _strategies = new Dictionary<string, ICalculoStrategy>(StringComparer.OrdinalIgnoreCase)
        {
            { "SALARIO_BASE", new SimpleStrategy() },
            { "VARIABLE", new SimpleStrategy() },
            { "COMISIONES", new SimpleStrategy() },
            { "TRANSPORTE", new SimpleStrategy() },

            { "NRO50", new HorasExtras50Strategy() },
            { "HORAS_EXTRAS_50", new HorasExtras50Strategy() },

            { "NRO100", new HorasExtras100Strategy() },
            { "HORAS_EXTRAS_100", new HorasExtras100Strategy() },

            { "DECIMO_TERCERO_MENSUAL", new DecimoTerceroStrategy() },
            { "DECIMO_CUARTO_MENSUAL", new DecimoCuartoStrategy() },
            { "FONDOS_RESERVA_MENSUAL", new FondosReservaStrategy() }
        };
    }
    
    public ICalculoStrategy GetStrategy(string parametro)
    {
        if (_strategies.TryGetValue(parametro, out var strategy))
        {
            return strategy;
        }
        
        throw new ArgumentException($"No existe estrategia para el parámetro: {parametro}");
    }
}