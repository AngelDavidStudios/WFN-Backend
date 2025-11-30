using Amazon.DynamoDBv2.DataModel;
using System.Text.Json.Serialization;

namespace WFNSystem.API.Models;

[DynamoDBTable("WFNSystem")]
public class Departamento
{
    // PK = DEP#<ID_Departamento>
    [DynamoDBHashKey]
    [JsonPropertyName("pk")]
    public string PK { get; set; } = string.Empty;

    // SK fijo porque es entidad principal
    [DynamoDBRangeKey]
    [JsonPropertyName("sk")]
    public string SK { get; set; } = "META#DEP";

    [DynamoDBProperty]
    [JsonPropertyName("id_Departamento")]
    public string ID_Departamento { get; set; } = string.Empty;

    [DynamoDBProperty]
    [JsonPropertyName("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [DynamoDBProperty]
    [JsonPropertyName("ubicacion")]
    public string Ubicacion { get; set; } = string.Empty;

    [DynamoDBProperty]
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [DynamoDBProperty]
    [JsonPropertyName("telefono")]
    public string Telefono { get; set; } = string.Empty;

    [DynamoDBProperty]
    [JsonPropertyName("cargo")]
    public string Cargo { get; set; } = string.Empty;

    [DynamoDBProperty]
    [JsonPropertyName("centroCosto")]
    public string CentroCosto { get; set; } = string.Empty;

    [DynamoDBProperty]
    [JsonPropertyName("dateCreated")]
    public string DateCreated { get; set; } = string.Empty;
}