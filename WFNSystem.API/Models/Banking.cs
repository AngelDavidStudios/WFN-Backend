using Amazon.DynamoDBv2.DataModel;

namespace WFNSystem.API.Models;

[DynamoDBTable("WFNBanking")]
public class Banking
{
    [DynamoDBHashKey("id")]
    public string ID_Banking { get; set; }
    
    [DynamoDBProperty]
    public string BankName { get; set; }
    
    [DynamoDBProperty]
    public string AccountNumber { get; set; }
    
    [DynamoDBProperty]
    public string AccountType { get; set; }
    
    [DynamoDBProperty]
    public string SWIFTCode { get; set; }
    
    [DynamoDBProperty]
    public string Pais { get; set; }
    
    [DynamoDBProperty]
    public string Sucursal { get; set; }
}