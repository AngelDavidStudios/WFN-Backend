using Amazon.DynamoDBv2.DataModel;

namespace WFNSystem.API.Models;

[DynamoDBTable("WFNovedades")]
public class Novedad
{
    [DynamoDBHashKey("id")]
    public string ID_Novedad { get; set; }
    
    [DynamoDBProperty]
    public string ID_Parametro { get; set; }
    
    [DynamoDBProperty]
    public string FechaIngresada { get; set; }
    
    [DynamoDBProperty]
    public string TipoNovedad { get; set; }
    
    [DynamoDBProperty]
    public string Descripcion { get; set; }
    
    [DynamoDBProperty]
    public string MontoAplicado { get; set; }
    
    [DynamoDBProperty]
    public bool Is_Gravable { get; set; }
}