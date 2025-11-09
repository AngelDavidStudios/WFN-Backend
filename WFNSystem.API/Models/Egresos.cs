using Amazon.DynamoDBv2.DataModel;

namespace WFNSystem.API.Models;

[DynamoDBTable("WFNEgresos")]
public class Egresos
{
    [DynamoDBHashKey("id")]
    public string ID_Egreso { get; set; }
    
    [DynamoDBProperty]
    public List<Novedad> Novedades { get; set; }
    
    [DynamoDBProperty]
    public decimal TotalEgresos { get; set; }
    
    [DynamoDBProperty]
    public string DateCreated { get; set; }
}