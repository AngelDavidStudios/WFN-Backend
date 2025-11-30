using Amazon.DynamoDBv2.DataModel;
using System.Text.Json.Serialization;

namespace WFNSystem.API.Models;

[DynamoDBTable("WFNSystem")]
public class Persona
{
    [DynamoDBHashKey]
    [JsonPropertyName("pk")]
    public string PK { get; set; } = string.Empty;

    [DynamoDBRangeKey]
    [JsonPropertyName("sk")]
    public string SK { get; set; } = "META#PERSONA";

    [DynamoDBProperty]
    [JsonPropertyName("id_Persona")]
    public string ID_Persona { get; set; } = string.Empty;

    [DynamoDBProperty]
    [JsonPropertyName("dni")]
    public string DNI { get; set; } = string.Empty;

    [DynamoDBProperty]
    [JsonPropertyName("gender")]
    public string Gender { get; set; } = string.Empty;

    [DynamoDBProperty]
    [JsonPropertyName("primerNombre")]
    public string PrimerNombre { get; set; } = string.Empty;

    [DynamoDBProperty]
    [JsonPropertyName("segundoNombre")]
    public string SegundoNombre { get; set; } = string.Empty;

    [DynamoDBProperty]
    [JsonPropertyName("apellidoMaterno")]
    public string ApellidoMaterno { get; set; } = string.Empty;

    [DynamoDBProperty]
    [JsonPropertyName("apellidoPaterno")]
    public string ApellidoPaterno { get; set; } = string.Empty;

    [DynamoDBProperty]
    [JsonPropertyName("dateBirthday")]
    public DateTime DateBirthday { get; set; }

    [DynamoDBProperty]
    [JsonPropertyName("edad")]
    public int Edad { get; set; }

    [DynamoDBProperty]
    [JsonPropertyName("correo")]
    public List<string> Correo { get; set; } = new();

    [DynamoDBProperty]
    [JsonPropertyName("telefono")]
    public List<string> Telefono { get; set; } = new();

    [DynamoDBProperty]
    [JsonPropertyName("direccion")]
    public Direccion Direccion { get; set; } = new();

    [DynamoDBProperty]
    [JsonPropertyName("dateCreated")]
    public string DateCreated { get; set; } = string.Empty;
}