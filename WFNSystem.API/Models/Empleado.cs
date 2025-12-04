using Amazon.DynamoDBv2.DataModel;
using System.Text.Json.Serialization;

namespace WFNSystem.API.Models;

[DynamoDBTable("WFNSystem")]
public class Empleado
{
    // PK = EMPLEADO#<id_empleado>
    [DynamoDBHashKey]
    [JsonPropertyName("pk")]
    public string PK { get; set; } = string.Empty;

    // SK = META#EMP
    [DynamoDBRangeKey]
    [JsonPropertyName("sk")]
    public string SK { get; set; } = "META#EMP";

    // ID lógico del empleado
    [DynamoDBProperty]
    [JsonPropertyName("id_Empleado")]
    public string ID_Empleado { get; set; } = string.Empty;

    // Relación con persona
    [DynamoDBProperty]
    [JsonPropertyName("id_Persona")]
    public string ID_Persona { get; set; } = string.Empty;

    // Relación con departamento
    [DynamoDBProperty]
    [JsonPropertyName("id_Departamento")]
    public string ID_Departamento { get; set; } = string.Empty;

    // Fecha de ingreso (DateTime)
    [DynamoDBProperty]
    [JsonPropertyName("fechaIngreso")]
    public DateTime FechaIngreso { get; set; }

    // Salario base
    [DynamoDBProperty]
    [JsonPropertyName("salarioBase")]
    public decimal SalarioBase { get; set; }

    // Configuración laboral
    [DynamoDBProperty]
    [JsonPropertyName("is_DecimoTercMensual")]
    public bool Is_DecimoTercMensual { get; set; }

    [DynamoDBProperty]
    [JsonPropertyName("is_DecimoCuartoMensual")]
    public bool Is_DecimoCuartoMensual { get; set; }

    [DynamoDBProperty]
    [JsonPropertyName("is_FondoReserva")]
    public bool Is_FondoReserva { get; set; }

    // Fecha de creación del registro (DateTime)
    [DynamoDBProperty]
    [JsonPropertyName("dateCreated")]
    public string DateCreated { get; set; } = string.Empty;

    // Estado laboral normalizado
    [DynamoDBProperty]
    [JsonPropertyName("statusLaboral")]
    public StatusLaboral StatusLaboral { get; set; }
}

public enum StatusLaboral
{
    Active = 0,
    Inactive = 1,
    OnLeave = 2,
    Retired = 3
}