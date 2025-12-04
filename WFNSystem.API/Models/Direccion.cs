using System.Text.Json.Serialization;

namespace WFNSystem.API.Models;

public class Direccion
{
    [JsonPropertyName("calle")]
    public string Calle { get; set; } = string.Empty;
    
    [JsonPropertyName("numero")]
    public string Numero { get; set; } = string.Empty;
    
    [JsonPropertyName("piso")]
    public string Piso { get; set; } = string.Empty;
}