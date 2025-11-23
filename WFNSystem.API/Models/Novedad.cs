using Amazon.DynamoDBv2.DataModel;

namespace WFNSystem.API.Models;

[DynamoDBTable("WFNSystem")]
public class Novedad
{
    // PK = EMP#<id_empleado>
    [DynamoDBHashKey]
    public string PK { get; set; } = string.Empty;

    // SK = NOV#<periodo>#<id_novedad>
    [DynamoDBRangeKey]
    public string SK { get; set; } = string.Empty;

    // Identificador lógico
    [DynamoDBProperty]
    public string ID_Novedad { get; set; } = string.Empty;

    // Parámetro que determina cómo se calcula esta novedad
    [DynamoDBProperty]
    public string ID_Parametro { get; set; } = string.Empty;

    // Periodo YYYY-MM
    [DynamoDBProperty]
    public string Periodo { get; set; } = string.Empty;

    // INGRESSO / EGRESO / PROVISION
    [DynamoDBProperty]
    public string TipoNovedad { get; set; } = string.Empty;

    // Fecha ingresada por usuario
    [DynamoDBProperty]
    public string FechaIngresada { get; set; } = string.Empty;

    // Descripción opcional
    [DynamoDBProperty]
    public string Descripcion { get; set; } = string.Empty;

    // Valor ingresado manualmente (variables, préstamos, anticipos, etc.)
    [DynamoDBProperty]
    public decimal MontoAplicado { get; set; }

    // Define si afecta IESS/IR
    [DynamoDBProperty]
    public bool Is_Gravable { get; set; } = true;
}