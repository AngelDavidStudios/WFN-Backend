using Amazon.DynamoDBv2.DataModel;

namespace WFNSystem.API.Models;

[DynamoDBTable("WFNDireccion")]
public class Direccion
{
    [DynamoDBHashKey("id")]
    public string ID_Direccion { get; set; }
    
    [DynamoDBProperty]
    public string Calle { get; set; }
    
    [DynamoDBProperty]
    public string Numero { get; set; }
    
    [DynamoDBProperty]
    public string? Piso { get; set; }
}