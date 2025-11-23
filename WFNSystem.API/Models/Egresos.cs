namespace WFNSystem.API.Models;

public class Egresos
{
    public string ID_Egreso { get; set; } = string.Empty;
    public List<Novedad> Novedades { get; set; } = new();
    public decimal TotalEgresos { get; set; }
    public string DateCreated { get; set; } = string.Empty;
}