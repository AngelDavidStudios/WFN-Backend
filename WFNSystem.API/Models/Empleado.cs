using Amazon.DynamoDBv2.DataModel;

namespace WFNSystem.API.Models;

[DynamoDBTable("WFNSystem")]
public class Empleado
{
    // PK = EMPLEADO#<id_empleado>
    [DynamoDBHashKey]
    public string PK { get; set; } = string.Empty;

    // SK = META#EMP
    [DynamoDBRangeKey]
    public string SK { get; set; } = "META#EMP";

    // ID lógico del empleado
    [DynamoDBProperty]
    public string ID_Empleado { get; set; } = string.Empty;

    // Relación con persona
    [DynamoDBProperty]
    public string ID_Persona { get; set; } = string.Empty;

    // Relación con departamento
    [DynamoDBProperty]
    public string ID_Departamento { get; set; } = string.Empty;

    // Fecha de ingreso (DateTime)
    [DynamoDBProperty]
    public DateTime FechaIngreso { get; set; }

    // Salario base
    [DynamoDBProperty]
    public decimal SalarioBase { get; set; }

    // Configuración laboral
    [DynamoDBProperty]
    public bool Is_DecimoTercMensual { get; set; }

    [DynamoDBProperty]
    public bool Is_DecimoCuartoMensual { get; set; }

    [DynamoDBProperty]
    public bool Is_FondoReserva { get; set; }

    // Fecha de creación del registro (DateTime)
    [DynamoDBProperty]
    public string DateCreated { get; set; }

    // Estado laboral normalizado
    [DynamoDBProperty]
    public StatusLaboral StatusLaboral { get; set; }
}

public enum StatusLaboral
{
    Active = 0,
    Inactive = 1,
    OnLeave = 2,
    Retired = 3
}