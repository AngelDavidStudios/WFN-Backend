using Amazon.DynamoDBv2.DataModel;
using System.Text.Json.Serialization;

namespace WFNSystem.API.Models;

[DynamoDBTable("WFNSystem")]
public class Nomina
{
    // PK = EMP#<id_empleado>
    [DynamoDBHashKey]
    [JsonPropertyName("pk")]
    public string PK { get; set; } = string.Empty;

    // SK = NOM#<periodo>
    [DynamoDBRangeKey]
    [JsonPropertyName("sk")]
    public string SK { get; set; } = string.Empty;

    // Identificador lógico
    [DynamoDBProperty]
    [JsonPropertyName("id_Nomina")]
    public string ID_Nomina { get; set; } = string.Empty;

    // Relación con empleado
    [DynamoDBProperty]
    [JsonPropertyName("id_Empleado")]
    public string ID_Empleado { get; set; } = string.Empty;

    // Periodo YYYY-MM
    [DynamoDBProperty]
    [JsonPropertyName("periodo")]
    public string Periodo { get; set; } = string.Empty;

    // Subtotales
    [DynamoDBProperty]
    [JsonPropertyName("totalIngresosGravados")]
    public decimal TotalIngresosGravados { get; set; }

    [DynamoDBProperty]
    [JsonPropertyName("totalIngresosNoGravados")]
    public decimal TotalIngresosNoGravados { get; set; }

    [DynamoDBProperty]
    [JsonPropertyName("totalIngresos")]
    public decimal TotalIngresos { get; set; }

    [DynamoDBProperty]
    [JsonPropertyName("totalEgresos")]
    public decimal TotalEgresos { get; set; }

    // Aportes
    [DynamoDBProperty]
    [JsonPropertyName("iess_AportePersonal")]
    public decimal IESS_AportePersonal { get; set; }

    [DynamoDBProperty]
    [JsonPropertyName("ir_Retenido")]
    public decimal IR_Retenido { get; set; }

    // Resultado final
    [DynamoDBProperty]
    [JsonPropertyName("netoAPagar")]
    public decimal NetoAPagar { get; set; }

    // Metadata
    [DynamoDBProperty]
    [JsonPropertyName("fechaCalculo")]
    public string FechaCalculo { get; set; } = string.Empty;

    [DynamoDBProperty]
    [JsonPropertyName("isCerrada")]
    public bool IsCerrada { get; set; }
}