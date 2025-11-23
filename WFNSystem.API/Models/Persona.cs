using Amazon.DynamoDBv2.DataModel;

namespace WFNSystem.API.Models;

[DynamoDBTable("WFNSystem")]
public class Persona
{
    // PK = PERSONA#<id_persona>
    [DynamoDBHashKey]
    public string PK { get; set; } = string.Empty;

    // SK = META#PERSONA
    [DynamoDBRangeKey]
    public string SK { get; set; } = "META#PERSONA";

    // Identificador real de persona
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
    public string DateBirthday { get; set; } = string.Empty;

    [DynamoDBProperty]
    public int Edad { get; set; }

    [DynamoDBProperty]
    public List<string> Correo { get; set; } = new();

    [DynamoDBProperty]
    public List<string> Telefono { get; set; } = new();
    
    [DynamoDBProperty]
    public Direccion Direccion { get; set; } = new();

    [DynamoDBProperty]
    public string DateCreated { get; set; } = string.Empty;
}