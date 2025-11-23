using WFNSystem.API.Services.Strategies.Interfaces;

namespace WFNSystem.API.Services.Strategies;

public class IngresoStrategyFactory
{
    public IIngresoStrategy GetStrategy(string parametro)
    {
        return parametro switch
        {
            "HORAS_EXTRAS_50"        => new IngresoHorasExtrasStrategy(),
            "HORAS_EXTRAS_100"       => new IngresoHorasExtrasStrategy(),
            "COMISIONES"             => new IngresoPorcentajeStrategy(),
            "VARIABLE"               => new IngresoSimpleStrategy(),
            "TRANSPORTE"             => new IngresoSimpleStrategy(),
            
            // Provisiones mensualizadas (se convierten en ingreso)
            "DECIMO_TERCERO_MENSUAL" => new IngresoProvisionMensualStrategy(),
            "DECIMO_CUARTO_MENSUAL"  => new IngresoProvisionMensualStrategy(),
            "FONDO_RESERVA_MENSUAL"  => new IngresoProvisionMensualStrategy(),

            _ => new IngresoSimpleStrategy()
        };
    }
}