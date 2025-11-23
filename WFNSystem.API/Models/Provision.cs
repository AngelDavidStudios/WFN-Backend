using Amazon.DynamoDBv2.DataModel;

namespace WFNSystem.API.Models;

[DynamoDBTable("WFNSystem")]
public class Provision
{
    // PK = EMP#<id_empleado>
    [DynamoDBHashKey]
    public string PK { get; set; } = string.Empty;

    // SK = PROV#<tipo>#<periodo>
    [DynamoDBRangeKey]
    public string SK { get; set; } = string.Empty;

    // Id lógico interno (por referencia)
    [DynamoDBProperty]
    public string ID_Provision { get; set; } = string.Empty;

    // Relación con empleado
    [DynamoDBProperty]
    public string ID_Empleado { get; set; } = string.Empty;

    // Tipo de provisión (DECIMO_TERCERO, DECIMO_CUARTO, FONDO_RESERVA, VACACIONES)
    [DynamoDBProperty]
    public string TipoProvision { get; set; } = string.Empty;

    // Periodo YYYY-MM
    [DynamoDBProperty]
    public string Periodo { get; set; } = string.Empty;

    // Valor mensual calculado según salario
    [DynamoDBProperty]
    public decimal ValorMensual { get; set; }

    // Suma total acumulada hasta este periodo
    [DynamoDBProperty]
    public decimal Acumulado { get; set; }

    // Valor total pagado cuando se transfiere
    [DynamoDBProperty]
    public decimal Total { get; set; }

    // TRUE si ya se pagó esta provisión en el periodo correspondiente
    [DynamoDBProperty]
    public bool IsTransferred { get; set; }

    // Fecha de cálculo
    [DynamoDBProperty]
    public string FechaCalculo { get; set; } = string.Empty;
}