using Amazon.DynamoDBv2.DataModel;

namespace WFNSystem.API.Models;

[DynamoDBTable("WFNSystem")]
public class Parametro
{
    // PK fijo para agrupar todos los parámetros
    [DynamoDBHashKey]
    public string PK { get; set; } = "PARAMETRO";

    // SK = PARAM#<id_parametro>
    [DynamoDBRangeKey]
    public string SK { get; set; } = string.Empty;

    // Identificador único lógico
    [DynamoDBProperty]
    public string ID_Parametro { get; set; } = string.Empty;

    // Nombre normalizado en SNAKE_CASE
    [DynamoDBProperty]
    public string Nombre { get; set; } = string.Empty;

    // Tipo de parámetro: INGRESO / EGRESO / PROVISION
    [DynamoDBProperty]
    public string Tipo { get; set; } = string.Empty;

    // Tipo de cálculo: SIMPLE / PORCENTAJE / HORAS_EXTRAS / PRESTAMO...
    [DynamoDBProperty]
    public string TipoCalculo { get; set; } = string.Empty;

    // Descripción opcional para panel administrativo
    [DynamoDBProperty]
    public string Descripcion { get; set; } = string.Empty;

    // Auditoría
    [DynamoDBProperty]
    public string DateCreated { get; set; }
}