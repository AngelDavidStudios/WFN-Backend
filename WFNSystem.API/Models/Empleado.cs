using Amazon.DynamoDBv2.DataModel;

namespace WFNSystem.API.Models;

[DynamoDBTable("WFNEmpleado")]
public class Empleado
{
    [DynamoDBHashKey("id")]
    public string ID_Empleado { get; set; }
    
    [DynamoDBProperty]
    public string ID_Persona { get; set; }
    
    [DynamoDBProperty]
    public List<Banking> BankingAccounts { get; set; }
    
    [DynamoDBProperty]
    public string ID_Departamento { get; set; }
    
    [DynamoDBProperty]
    public string FechaIngreso { get; set; }
    
    [DynamoDBProperty]
    public decimal SalarioBase { get; set; }
    
    [DynamoDBProperty]
    public bool Is_DecimoTercMensual { get; set; }
    
    [DynamoDBProperty]
    public bool Is_DecimoCuartoMensual { get; set; }
    
    [DynamoDBProperty]
    public bool Is_FondoReserva { get; set; }
    
    [DynamoDBProperty]
    public string DateCreated { get; set; }
    
    [DynamoDBProperty]
    public StatusLaboral StatusLaboral { get; set; }
}

public enum StatusLaboral
{
    Active,
    Inactive,
    OnLeave,
    Retired
}