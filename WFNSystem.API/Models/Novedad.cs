using Amazon.DynamoDBv2.DataModel;
using System.Text.Json.Serialization;

namespace WFNSystem.API.Models;

[DynamoDBTable("WFNSystem")]
public class Novedad
{
    // PK = EMP#<id_empleado>
    [DynamoDBHashKey]
    [JsonPropertyName("pk")]
    public string PK { get; set; } = string.Empty;

    // SK = NOV#<periodo>#<id_novedad>
    [DynamoDBRangeKey]
    [JsonPropertyName("sk")]
    public string SK { get; set; } = string.Empty;

    // Identificador lógico
    [DynamoDBProperty]
    [JsonPropertyName("id_Novedad")]
    public string ID_Novedad { get; set; } = string.Empty;

    // Parámetro que determina cómo se calcula esta novedad
    [DynamoDBProperty]
    [JsonPropertyName("id_Parametro")]
    public string ID_Parametro { get; set; } = string.Empty;

    // Periodo YYYY-MM
    [DynamoDBProperty]
    [JsonPropertyName("periodo")]
    public string Periodo { get; set; } = string.Empty;

    // INGRESSO / EGRESO / PROVISION
    [DynamoDBProperty]
    [JsonPropertyName("tipoNovedad")]
    public string TipoNovedad { get; set; } = string.Empty;

    // Fecha ingresada por usuario
    [DynamoDBProperty]
    [JsonPropertyName("fechaIngresada")]
    public string FechaIngresada { get; set; } = string.Empty;

    // Descripción opcional
    [DynamoDBProperty]
    [JsonPropertyName("descripcion")]
    public string Descripcion { get; set; } = string.Empty;

    // Valor ingresado manualmente (variables, préstamos, anticipos, etc.)
    [DynamoDBProperty]
    [JsonPropertyName("montoAplicado")]
    public decimal MontoAplicado { get; set; }

    // Define si afecta IESS/IR
    [DynamoDBProperty]
    [JsonPropertyName("is_Gravable")]
    public bool Is_Gravable { get; set; } = true;
}