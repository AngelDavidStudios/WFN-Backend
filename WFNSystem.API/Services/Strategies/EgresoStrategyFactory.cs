using WFNSystem.API.Services.Strategies.Egresos;
using WFNSystem.API.Services.Strategies.Interfaces;

namespace WFNSystem.API.Services.Strategies;

public class EgresoStrategyFactory
{
    private readonly Dictionary<string, ICalculoStrategy> _strategies;

    public EgresoStrategyFactory()
    {
        _strategies = new Dictionary<string, ICalculoStrategy>(StringComparer.OrdinalIgnoreCase)
        {
            { "IESS_PERSONAL", new IessPersonalStrategy() },
            { "IESS_EXTENSION_CONYUGE", new IessExtensionStrategy() },
            { "IESS_EXTENSION_CONVIVIENTE", new IessExtensionStrategy() },

            { "IMPUESTO_RENTA", new ImpuestoStrategy() },

            { "PRESTAMO_QUIROGRAFARIO", new PrestamoStrategy() },
            { "PRESTAMO_HIPOTECARIO", new PrestamoStrategy() },
            { "PRESTAMOS_EMPLEADOS", new PrestamoStrategy() },

            { "ANTICIPOS_EMPLEADOS", new SimpleEgresoStrategy() },
            { "COMPRA_ALMACEN", new SimpleEgresoStrategy() },
            { "GIMNASIO", new SimpleEgresoStrategy() },
            { "COMISARIATO", new SimpleEgresoStrategy() },
            { "CATERING", new SimpleEgresoStrategy() },
            { "CONSUMO_CELULAR", new SimpleEgresoStrategy() },
            { "COMPRA_CELULAR", new SimpleEgresoStrategy() },
            { "FALTA_INJUSTIFICADA", new SimpleEgresoStrategy() },
            { "SEGURO_VIDA", new SimpleEgresoStrategy() },
            { "PENSION_ALIMENTICIA", new SimpleEgresoStrategy() },
            { "SUBSIDIO_IESS_ENFERMEDAD", new SimpleEgresoStrategy() },
            { "NOTA_CREDITO_IESS_MATERNIDAD", new SimpleEgresoStrategy() }
        };
    }
    
    public ICalculoStrategy GetStrategy(string egresoType)
    {
        if (_strategies.TryGetValue(egresoType, out var strategy))
        {
            return strategy;
        }

        throw new ArgumentException($"Egreso strategy not found for type: {egresoType}");
    }
}