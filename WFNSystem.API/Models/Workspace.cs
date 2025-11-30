using Amazon.DynamoDBv2.DataModel;
using System.Text.Json.Serialization;

namespace WFNSystem.API.Models;

[DynamoDBTable("WFNSystem")]
public class WorkspaceNomina
{
    // PK fijo para todos los periodos
    [DynamoDBHashKey]
    [JsonPropertyName("pk")]
    public string PK { get; set; } = "WORKSPACE#GLOBAL";

    // SK = WORK#{periodo} (ejemplo: WORK#2025-11)
    [DynamoDBRangeKey]
    [JsonPropertyName("sk")]
    public string SK { get; set; } = string.Empty;

    // Identificador del workspace
    [DynamoDBProperty]
    [JsonPropertyName("id_Workspace")]
    public string ID_Workspace { get; set; } = string.Empty;

    // Periodo YYYY-MM
    [DynamoDBProperty]
    [JsonPropertyName("periodo")]
    public string Periodo { get; set; } = string.Empty;

    // Fecha en que se abrió el periodo
    [DynamoDBProperty]
    [JsonPropertyName("fechaCreacion")]
    public string FechaCreacion { get; set; } = string.Empty;

    // Fecha en que se cerró el periodo
    [DynamoDBProperty]
    [JsonPropertyName("fechaCierre")]
    public string FechaCierre { get; set; } = string.Empty;

    // 0 = Abierto, 1 = Cerrado
    [DynamoDBProperty]
    [JsonPropertyName("estado")]
    public int Estado { get; set; }
}