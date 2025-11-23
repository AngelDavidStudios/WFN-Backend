using Amazon.DynamoDBv2.DataModel;

namespace WFNSystem.API.Models;

[DynamoDBTable("WFNSystem")]
public class Departamento
{
    // PK = DEPARTAMENTO#<ID_Departamento>
    [DynamoDBHashKey]
    public string PK { get; set; } = string.Empty;

    // SK fijo porque es entidad principal
    [DynamoDBRangeKey]
    public string SK { get; set; } = "META#DEPARTAMENTO";

    [DynamoDBProperty]
    public string ID_Departamento { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string Nombre { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string Ubicacion { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string Email { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string Telefono { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string Cargo { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string CentroCosto { get; set; } = string.Empty;

    [DynamoDBProperty]
    public DateTime DateCreated { get; set; }
}