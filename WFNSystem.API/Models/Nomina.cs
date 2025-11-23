using Amazon.DynamoDBv2.DataModel;

namespace WFNSystem.API.Models;

[DynamoDBTable("WFNSystem")]
public class Nomina
{
    // PK = EMP#<id_empleado>
    [DynamoDBHashKey]
    public string PK { get; set; } = string.Empty;

    // SK = NOM#<id_nomina>
    [DynamoDBRangeKey]
    public string SK { get; set; } = string.Empty;

    // ID lógico de la nómina
    [DynamoDBProperty]
    public string ID_Nomina { get; set; } = string.Empty;

    // Relación con empleado
    [DynamoDBProperty]
    public string ID_Empleado { get; set; } = string.Empty;

    // Mes-Año: “2025-03”, “2025-11”
    [DynamoDBProperty]
    public string Periodo { get; set; } = string.Empty;

    // Lista de ingresos
    [DynamoDBProperty]
    public List<Ingresos> Ingresos { get; set; } = new();

    // Lista de egresos
    [DynamoDBProperty]
    public List<Egresos> Egresos { get; set; } = new();

    [DynamoDBProperty]
    public decimal TotalIngresos { get; set; }

    [DynamoDBProperty]
    public decimal TotalEgresos { get; set; }

    [DynamoDBProperty]
    public decimal NetoAPagar { get; set; }
}