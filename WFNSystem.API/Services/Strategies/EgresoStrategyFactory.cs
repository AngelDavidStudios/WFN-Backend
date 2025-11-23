using WFNSystem.API.Services.Strategies.Egresos;
using WFNSystem.API.Services.Strategies.Interfaces;

namespace WFNSystem.API.Services.Strategies;

public class EgresoStrategyFactory
{
    public IEgresoStrategy GetStrategy(string parametro)
    {
        return parametro switch
        {
            "IESS_PERSONAL"                 => new EgresoIESSStrategy(),
            "IESS_EXTENSION_CONYUGUE"       => new EgresoPorcentajeStrategy(),

            "PRESTAMO_QUIROGRAFARIO"        => new EgresoPrestamoStrategy(),
            "PRESTAMO_HIPOTECARIO"          => new EgresoPrestamoStrategy(),
            "PRESTAMO_EMPLEADOS"            => new EgresoPrestamoStrategy(),

            "ANTICIPO_EMPLEADOS"            => new EgresoAnticipoStrategy(),

            // Egresos simples
            "IMPUESTO_RENTA"                => new EgresoSimpleStrategy(),
            "COMPRA_ALMACEN"                => new EgresoSimpleStrategy(),
            "GIMNASIO"                      => new EgresoSimpleStrategy(),
            "COMISARIATO"                   => new EgresoSimpleStrategy(),
            "CATERING"                      => new EgresoSimpleStrategy(),
            "CONSUMO_CELULAR"               => new EgresoSimpleStrategy(),
            "COMPRA_CELULAR"                => new EgresoSimpleStrategy(),
            "FALTA_INJUSTIFICADA"           => new EgresoSimpleStrategy(),
            "SEGURO_VIDA"                   => new EgresoSimpleStrategy(),
            "PENSION_ALIMENTICIA"           => new EgresoSimpleStrategy(),
            "SUBSIDIO_IESS_ENFERMEDAD"      => new EgresoSimpleStrategy(),
            "NOTA_CREDITO_IESS_MATERNIDAD"  => new EgresoSimpleStrategy(),

            _ => new EgresoSimpleStrategy()
        };
    }
}