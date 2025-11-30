using Amazon.DynamoDBv2.DataModel;
using System.Text.Json.Serialization;

namespace WFNSystem.API.Models;

[DynamoDBTable("WFNSystem")]
public class Parametro
{
    // PK fijo para agrupar todos los parámetros
    [DynamoDBHashKey]
    [JsonPropertyName("pk")]
    public string PK { get; set; } = "PARAM#GLOBAL";

    // SK = PARAM#<id_parametro>
    [DynamoDBRangeKey]
    [JsonPropertyName("sk")]
    public string SK { get; set; } = string.Empty;

    // Identificador único lógico
    [DynamoDBProperty]
    [JsonPropertyName("id_Parametro")]
    public string ID_Parametro { get; set; } = string.Empty;

    // Nombre normalizado en SNAKE_CASE
    [DynamoDBProperty]
    [JsonPropertyName("nombre")]
    public string Nombre { get; set; } = string.Empty;

    // Tipo de parámetro: INGRESO / EGRESO / PROVISION
    [DynamoDBProperty]
    [JsonPropertyName("tipo")]
    public string Tipo { get; set; } = string.Empty;

    // Tipo de cálculo: SIMPLE / PORCENTAJE / HORAS_EXTRAS / PRESTAMO...
    [DynamoDBProperty]
    [JsonPropertyName("tipoCalculo")]
    public string TipoCalculo { get; set; } = string.Empty;

    // Descripción opcional para panel administrativo
    [DynamoDBProperty]
    [JsonPropertyName("descripcion")]
    public string Descripcion { get; set; } = string.Empty;

    // Auditoría
    [DynamoDBProperty]
    [JsonPropertyName("dateCreated")]
    public string DateCreated { get; set; } = string.Empty;
}