namespace WFNSystem.API.Models;

public class Ingresos
{
    public string ID_Ingreso { get; set; } = string.Empty;
    public List<Novedad> Novedades { get; set; } = new();
    public decimal SubTotal_Gravado_IESS { get; set; }
    public decimal SubTotal_No_Gravado_IESS { get; set; }
    public decimal TotalIngresos { get; set; }
    public string DateCreated { get; set; } = string.Empty;
}