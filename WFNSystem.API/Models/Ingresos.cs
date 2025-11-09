using Amazon.DynamoDBv2.DataModel;

namespace WFNSystem.API.Models;

[DynamoDBTable("WFNIngresos")]
public class Ingresos
{
    [DynamoDBHashKey("id")]
    public string ID_Ingreso { get; set; }
    
    [DynamoDBProperty]
    public List<Novedad> Novedades { get; set; }
    
    [DynamoDBProperty]
    public decimal SubTotal_Gravado_IESS { get; set; }
    
    [DynamoDBProperty]
    public decimal SubTotal_No_Gravado_IESS { get; set; }
    
    [DynamoDBProperty]
    public decimal TotalIngresos { get; set; }
    
    [DynamoDBProperty]
    public string DateCreated { get; set; }
}