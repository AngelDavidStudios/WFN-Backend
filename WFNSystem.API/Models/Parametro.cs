using Amazon.DynamoDBv2.DataModel;

namespace WFNSystem.API.Models;

[DynamoDBTable("WFNSystem")]
public class Parametro
{
    // PK = PARAMETRO#GLOBAL
    [DynamoDBHashKey]
    public string PK { get; set; } = "PARAMETRO#GLOBAL";

    // SK = PARAM#<id_parametro>
    [DynamoDBRangeKey]
    public string SK { get; set; } = string.Empty;

    // ID lógico del parámetro
    [DynamoDBProperty]
    public string ID_Parametro { get; set; } = string.Empty;

    // Tipo de parámetro (Horas Extras, Gimnasio, Comisariato, Consumo Celular, etc.)
    [DynamoDBProperty]
    public string Tipo { get; set; } = string.Empty;

    // Descripción del parámetro
    [DynamoDBProperty]
    public string Descripcion { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string DateCreated { get; set; } = string.Empty;
}