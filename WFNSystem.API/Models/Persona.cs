using System.ComponentModel.DataAnnotations;
using Amazon.DynamoDBv2.DataModel;

namespace WFNSystem.API.Models;

[DynamoDBTable("WFNPersona")]
public class Persona
{
    [DynamoDBHashKey("id")]
    public string ID_Persona { get; set; }
    
    [DynamoDBProperty]
    public string DNI { get; set; }
    
    [DynamoDBProperty]
    public string Gender { get; set; }
    
    [DynamoDBProperty]
    public string PrimerNombre { get; set; }
    
    [DynamoDBProperty]
    public string? SegundoNombre { get; set; }
    
    [DynamoDBProperty]
    public string ApellidoMaterno { get; set; }
    
    [DynamoDBProperty]
    public string? ApellidoPaterno { get; set; }
    
    [DynamoDBProperty]
    public string DateBirthday { get; set; }
    
    [DynamoDBProperty]
    public int Edad { get; set; }
    
    [DynamoDBProperty]
    public List<string>? Correo { get; set; }
    
    [DynamoDBProperty]
    public List<string>? Telefono { get; set; }
    
    [DynamoDBProperty]
    public List<Direccion>? Direcciones { get; set; }
    
    [DynamoDBProperty]
    public string DateCreated { get; set; }
    
}