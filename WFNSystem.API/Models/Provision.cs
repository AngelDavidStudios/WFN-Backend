using Amazon.DynamoDBv2.DataModel;

namespace WFNSystem.API.Models;

[DynamoDBTable("WFNProvision")]
public class Provision
{
    [DynamoDBHashKey("id")]
    public string ID_Provision { get; set; }
    
    [DynamoDBProperty]
    public string ID_Empleado { get; set; }
    
    [DynamoDBProperty]
    public string TipoProvision { get; set; }
    
    [DynamoDBProperty]
    public string Periodo { get; set; }
    
    [DynamoDBProperty]
    public decimal ValorMensual { get; set; }
    
    [DynamoDBProperty]
    public decimal Acumulado { get; set; }
    
    [DynamoDBProperty]
    public decimal Total { get; set; }
    
    [DynamoDBProperty]
    public bool IsTrasnferred { get; set; }
}