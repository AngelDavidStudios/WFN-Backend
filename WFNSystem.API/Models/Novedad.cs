using Amazon.DynamoDBv2.DataModel;

namespace WFNSystem.API.Models;

[DynamoDBTable("WFNSystem")]
public class Novedad
{
    // PK = EMP#<id_empleado>
    [DynamoDBHashKey]
    public string PK { get; set; } = string.Empty;

    // SK = NOV#<id_novedad>
    [DynamoDBRangeKey]
    public string SK { get; set; } = string.Empty;

    // Identificador de la novedad
    [DynamoDBProperty]
    public string ID_Novedad { get; set; } = string.Empty;

    // Relación con empleado
    [DynamoDBProperty]
    public string ID_Empleado { get; set; } = string.Empty;

    // Relación con parámetro
    [DynamoDBProperty]
    public string ID_Parametro { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string FechaIngresada { get; set; } = string.Empty;

    // Tipo: Ingreso / Egreso
    [DynamoDBProperty]
    public string TipoNovedad { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string Descripcion { get; set; } = string.Empty;

    [DynamoDBProperty]
    public decimal MontoAplicado { get; set; }

    [DynamoDBProperty]
    public bool Is_Gravable { get; set; }
}