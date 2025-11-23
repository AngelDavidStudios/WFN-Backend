using Amazon.DynamoDBv2.DataModel;

namespace WFNSystem.API.Models;

[DynamoDBTable("WFNSystem")]
public class Provision
{
    // PK = EMP#<id_empleado>
    [DynamoDBHashKey]
    public string PK { get; set; } = string.Empty;

    // SK = PROV#<id_provision>
    [DynamoDBRangeKey]
    public string SK { get; set; } = string.Empty;

    // Identificador lógico de la provisión
    [DynamoDBProperty]
    public string ID_Provision { get; set; } = string.Empty;

    // Relación con empleado
    [DynamoDBProperty]
    public string ID_Empleado { get; set; } = string.Empty;

    // Tipo de provisión (ej: "DECIMO_TERCERO", "VACACIONES", "IESS_PATRONAL")
    [DynamoDBProperty]
    public string TipoProvision { get; set; } = string.Empty;

    // Ej. "2025-11"
    [DynamoDBProperty]
    public string Periodo { get; set; } = string.Empty;

    // Valor mensual generado para esta provisión
    [DynamoDBProperty]
    public decimal ValorMensual { get; set; }

    // Acumulado histórico en el año
    [DynamoDBProperty]
    public decimal Acumulado { get; set; }

    // Total actual (ej: ValorMensual + Acumulado)
    [DynamoDBProperty]
    public decimal Total { get; set; }

    // Indica si ya fue transferido a una nómina
    [DynamoDBProperty]
    public bool IsTransferred { get; set; }
    
    [DynamoDBProperty]
    public string DetalleCalculo { get; set; } = string.Empty;
}