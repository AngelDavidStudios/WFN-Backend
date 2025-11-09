using Amazon.DynamoDBv2.DataModel;

namespace WFNSystem.API.Models;

[DynamoDBTable("WFNNomina")]
public class Nomina
{
    [DynamoDBHashKey("id")]
    public string ID_Nomina { get; set; }
    
    [DynamoDBProperty]
    public string ID_Empleado { get; set; }
    
    [DynamoDBProperty]
    public List<Ingresos> Ingresos { get; set; }
    
    [DynamoDBProperty]
    public List<Egresos> Egresos { get; set; }
    
    [DynamoDBProperty]
    public decimal TotalIngresos { get; set; }
    
    [DynamoDBProperty]
    public decimal TotalEgresos { get; set; }
    
    [DynamoDBProperty]
    public decimal NetoAPagar { get; set; }
}