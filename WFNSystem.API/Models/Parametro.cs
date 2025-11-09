using Amazon.DynamoDBv2.DataModel;

namespace WFNSystem.API.Models;

[DynamoDBTable("WFNParametro")]
public class Parametro
{
    [DynamoDBHashKey("id")]
    public string ID_Parametro { get; set; }
    
    [DynamoDBProperty]
    public string Tipo { get; set; }
    
    [DynamoDBProperty]
    public string Descripcion { get; set; }
    
    [DynamoDBProperty]
    public string DateCreated { get; set; }
}