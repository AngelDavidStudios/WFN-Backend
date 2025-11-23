using Amazon.DynamoDBv2.DataModel;

namespace WFNSystem.API.Models;

[DynamoDBTable("WFNSystem")]
public class Persona
{
    [DynamoDBHashKey]
    public string PK { get; set; } = string.Empty;

    [DynamoDBRangeKey]
    public string SK { get; set; } = "META#PERSONA";

    [DynamoDBProperty]
    public string ID_Persona { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string DNI { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string Gender { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string PrimerNombre { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string SegundoNombre { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string ApellidoMaterno { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string ApellidoPaterno { get; set; } = string.Empty;

    [DynamoDBProperty]
    public DateTime DateBirthday { get; set; }

    [DynamoDBProperty]
    public int Edad { get; set; }

    [DynamoDBProperty]
    public List<string> Correo { get; set; } = new();

    [DynamoDBProperty]
    public List<string> Telefono { get; set; } = new();

    [DynamoDBProperty]
    public Direccion Direccion { get; set; } = new();

    [DynamoDBProperty]
    public DateTime DateCreated { get; set; }
}