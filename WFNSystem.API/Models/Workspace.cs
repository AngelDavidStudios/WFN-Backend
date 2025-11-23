using Amazon.DynamoDBv2.DataModel;

namespace WFNSystem.API.Models;

[DynamoDBTable("WFNSystem")]
public class WorkspaceNomina
{
    // PK fijo para todos los periodos
    [DynamoDBHashKey]
    public string PK { get; set; } = "WORKSPACE";

    // SK = PERIOD#2025-02
    [DynamoDBRangeKey]
    public string SK { get; set; } = string.Empty;

    // Identificador del workspace
    [DynamoDBProperty]
    public string ID_Workspace { get; set; } = string.Empty;

    // Periodo YYYY-MM
    [DynamoDBProperty]
    public string Periodo { get; set; } = string.Empty;

    // Fecha en que se abrió el periodo
    [DynamoDBProperty]
    public string FechaCreacion { get; set; } = string.Empty;

    // Fecha en que se cerró el periodo
    [DynamoDBProperty]
    public string FechaCierre { get; set; } = string.Empty;

    // 0 = Abierto, 1 = Cerrado
    [DynamoDBProperty]
    public int Estado { get; set; }
}