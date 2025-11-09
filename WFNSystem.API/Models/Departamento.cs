using Amazon.DynamoDBv2.DataModel;

namespace WFNSystem.API.Models;

[DynamoDBTable("WFNDepartamento")]
public class Departamento
{
    [DynamoDBHashKey("id")]
    public string ID_Departamento { get; set; }
    
    [DynamoDBProperty]
    public string Nombre { get; set; }
    
    [DynamoDBProperty]
    public string Ubicacion { get; set; }
    
    [DynamoDBProperty]
    public string Email { get; set; }
    
    [DynamoDBProperty]
    public string Telefono { get; set; }
    
    [DynamoDBProperty]
    public string Cargo { get; set; }
    
    [DynamoDBProperty]
    public string CentroCosto { get; set; }
}