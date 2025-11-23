using Amazon.DynamoDBv2.DataModel;

namespace WFNSystem.API.Models;

[DynamoDBTable("WFNSystem")]
public class Nomina
{
    [DynamoDBHashKey] 
    public string PK { get; set; } = string.Empty; 
    // NOMINA#2025-11
    
    [DynamoDBRangeKey]
    public string SK { get; set; } = string.Empty;
    // EMPLEADO#<ID>
    
    public string ID_Nomina { get; set; } = string.Empty;

    public string ID_Empleado { get; set; } = string.Empty;
    public string Periodo { get; set; } = string.Empty;

    public List<Ingresos>? Ingresos { get; set; }
    public List<Egresos>? Egresos { get; set; }

    public decimal TotalIngresos { get; set; }
    public decimal TotalEgresos { get; set; }
    public decimal NetoAPagar { get; set; }

    public string DateCreated { get; set; } = string.Empty;
}