using Amazon.DynamoDBv2.DataModel;

namespace WFNSystem.API.Models;

[DynamoDBTable("WFNWorkspace")]
public class Workspace
{
    [DynamoDBHashKey("id")]
    public string ID_Workspace { get; set; }
    
    [DynamoDBProperty]
    public List<Nomina> Nominas { get; set; }
    
    [DynamoDBProperty]
    public string FechaCreacion { get; set; }
    
    [DynamoDBProperty]
    public string FechaCierre { get; set; }
    
    [DynamoDBProperty]
    public Estado Estado { get; set; } = Estado.Abierto;
}

public enum Estado
{
    Abierto,
    Cerrado,
    Pendiente
}