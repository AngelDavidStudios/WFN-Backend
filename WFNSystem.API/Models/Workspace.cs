using System.ComponentModel.DataAnnotations;
using Amazon.DynamoDBv2.DataModel;

namespace WFNSystem.API.Models;

[DynamoDBTable("WFNSystem")]
public class WorkspaceNomina
{
    [DynamoDBHashKey]
    public string PK { get; set; } = "WORKSPACE#GLOBAL";

    [DynamoDBRangeKey]
    public string SK { get; set; } = string.Empty; // WORK#2025-11

    public string ID_Workspace { get; set; } = string.Empty;

    // "2025-11"
    public string Periodo { get; set; } = string.Empty;

    public string FechaCreacion { get; set; } = string.Empty;
    public string FechaCierre { get; set; } = string.Empty;

    // 0 = Abierto, 1 = Cerrado
    public int Estado { get; set; }
}
