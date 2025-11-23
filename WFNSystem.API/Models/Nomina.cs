using Amazon.DynamoDBv2.DataModel;

namespace WFNSystem.API.Models;

[DynamoDBTable("WFNSystem")]
public class Nomina
{
    // PK = EMP#<id_empleado>
    [DynamoDBHashKey]
    public string PK { get; set; } = string.Empty;

    // SK = NOM#<periodo>
    [DynamoDBRangeKey]
    public string SK { get; set; } = string.Empty;

    // Identificador lógico
    [DynamoDBProperty]
    public string ID_Nomina { get; set; } = string.Empty;

    // Relación con empleado
    [DynamoDBProperty]
    public string ID_Empleado { get; set; } = string.Empty;

    // Periodo YYYY-MM
    [DynamoDBProperty]
    public string Periodo { get; set; } = string.Empty;

    // Subtotales
    [DynamoDBProperty]
    public decimal TotalIngresosGravados { get; set; }

    [DynamoDBProperty]
    public decimal TotalIngresosNoGravados { get; set; }

    [DynamoDBProperty]
    public decimal TotalIngresos { get; set; }

    [DynamoDBProperty]
    public decimal TotalEgresos { get; set; }

    // Aportes
    [DynamoDBProperty]
    public decimal IESS_AportePersonal { get; set; }

    [DynamoDBProperty]
    public decimal IR_Retenido { get; set; }

    // Resultado final
    [DynamoDBProperty]
    public decimal NetoAPagar { get; set; }

    // Metadata
    [DynamoDBProperty]
    public string FechaCalculo { get; set; } = string.Empty;

    [DynamoDBProperty]
    public bool IsCerrada { get; set; }
}