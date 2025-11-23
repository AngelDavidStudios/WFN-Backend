using Amazon.DynamoDBv2.DataModel;

namespace WFNSystem.API.Models;

[DynamoDBTable("WFNSystem")]
public class Banking
{
    // PK = EMPLEADO#<ID_Empleado>
    [DynamoDBHashKey]
    public string PK { get; set; } = string.Empty;    
    
    // SK = BANK#ID_Banking
    [DynamoDBRangeKey]
    public string SK { get; set; } = string.Empty;     

    // ID de la cuenta bancaria
    [DynamoDBProperty]
    public string ID_Banking { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string BankName { get; set; } = string.Empty;
    
    [DynamoDBProperty]
    public string AccountNumber { get; set; } = string.Empty;
    
    [DynamoDBProperty]
    public string AccountType { get; set; } = string.Empty;
    
    [DynamoDBProperty]
    public string SWIFTCode { get; set; } = string.Empty;
    
    [DynamoDBProperty]
    public string Pais { get; set; } = string.Empty;
    
    [DynamoDBProperty]
    public string Sucursal { get; set; } = string.Empty;
    
    [DynamoDBProperty]
    public DateTime DateCreated { get; set; }
}