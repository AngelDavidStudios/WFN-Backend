using Amazon.DynamoDBv2.DataModel;
using System.Text.Json.Serialization;

namespace WFNSystem.API.Models;

[DynamoDBTable("WFNSystem")]
public class Banking
{
    // PK = PERSONA#<ID_Persona>
    [DynamoDBHashKey]
    [JsonPropertyName("pk")]
    public string PK { get; set; } = string.Empty;    
    
    // SK = BANK#<ID_Banking>
    [DynamoDBRangeKey]
    [JsonPropertyName("sk")]
    public string SK { get; set; } = string.Empty;     

    // ID de la cuenta bancaria
    [DynamoDBProperty]
    [JsonPropertyName("id_Banking")]
    public string ID_Banking { get; set; } = string.Empty;

    [DynamoDBProperty]
    [JsonPropertyName("bankName")]
    public string BankName { get; set; } = string.Empty;
    
    [DynamoDBProperty]
    [JsonPropertyName("accountNumber")]
    public string AccountNumber { get; set; } = string.Empty;
    
    [DynamoDBProperty]
    [JsonPropertyName("accountType")]
    public string AccountType { get; set; } = string.Empty;
    
    [DynamoDBProperty]
    [JsonPropertyName("swiftCode")]
    public string SWIFTCode { get; set; } = string.Empty;
    
    [DynamoDBProperty]
    [JsonPropertyName("pais")]
    public string Pais { get; set; } = string.Empty;
    
    [DynamoDBProperty]
    [JsonPropertyName("sucursal")]
    public string Sucursal { get; set; } = string.Empty;
    
    [DynamoDBProperty]
    [JsonPropertyName("dateCreated")]
    public DateTime DateCreated { get; set; }
}