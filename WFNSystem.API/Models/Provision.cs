using Amazon.DynamoDBv2.DataModel;
using System.Text.Json.Serialization;

namespace WFNSystem.API.Models;

[DynamoDBTable("WFNSystem")]
public class Provision
{
    // PK = EMP#<id_empleado>
    [DynamoDBHashKey]
    [JsonPropertyName("pk")]
    public string PK { get; set; } = string.Empty;

    // SK = PROV#<tipo>#<periodo>
    [DynamoDBRangeKey]
    [JsonPropertyName("sk")]
    public string SK { get; set; } = string.Empty;

    // Id lógico interno (por referencia)
    [DynamoDBProperty]
    [JsonPropertyName("id_Provision")]
    public string ID_Provision { get; set; } = string.Empty;

    // Relación con empleado
    [DynamoDBProperty]
    [JsonPropertyName("id_Empleado")]
    public string ID_Empleado { get; set; } = string.Empty;

    // Tipo de provisión (DECIMO_TERCERO, DECIMO_CUARTO, FONDO_RESERVA, VACACIONES)
    [DynamoDBProperty]
    [JsonPropertyName("tipoProvision")]
    public string TipoProvision { get; set; } = string.Empty;

    // Periodo YYYY-MM
    [DynamoDBProperty]
    [JsonPropertyName("periodo")]
    public string Periodo { get; set; } = string.Empty;

    // Valor mensual calculado según salario
    [DynamoDBProperty]
    [JsonPropertyName("valorMensual")]
    public decimal ValorMensual { get; set; }

    // Suma total acumulada hasta este periodo
    [DynamoDBProperty]
    [JsonPropertyName("acumulado")]
    public decimal Acumulado { get; set; }

    // Valor total pagado cuando se transfiere
    [DynamoDBProperty]
    [JsonPropertyName("total")]
    public decimal Total { get; set; }

    // TRUE si ya se pagó esta provisión en el periodo correspondiente
    [DynamoDBProperty]
    [JsonPropertyName("isTransferred")]
    public bool IsTransferred { get; set; }

    // Fecha de cálculo
    [DynamoDBProperty]
    [JsonPropertyName("fechaCalculo")]
    public string FechaCalculo { get; set; } = string.Empty;
}