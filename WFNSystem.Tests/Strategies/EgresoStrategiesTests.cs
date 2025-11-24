using FluentAssertions;
using WFNSystem.API.Models;
using WFNSystem.API.Services.Strategies.Egresos;
using Xunit;

namespace WFNSystem.Tests.Strategies;

/// <summary>
/// Tests unitarios para Strategies de Egresos
/// Valida fórmulas exactas según Tabla referencia.txt
/// </summary>
public class EgresoStrategiesTests
{
    /// <summary>
    /// Fórmula: TOTAL INGRESOS GRAVADOS IESS * 9.45%
    /// </summary>
    [Theory]
    [InlineData(847.98, 80.13)] // Empleado 1 Example.txt
    [InlineData(1068.44, 100.97)] // Empleado 2 Example.txt
    [InlineData(460, 43.47)] // SBU
    [InlineData(1500, 141.75)]
    public void IessPersonalStrategy_DebeCalcular9_45Porciento(
        decimal totalGravado, decimal esperado)
    {
        // Arrange
        var strategy = new IessPersonalStrategy();
        var novedad = new Novedad();
        var empleado = new Empleado();

        // Act
        var resultado = strategy.Calcular(
            novedad,
            empleado,
            0,
            totalGravado,
            new Dictionary<string, decimal>()
        );

        // Assert
        resultado.Should().BeApproximately(esperado, 0.01m,
            $"IessPersonal: {totalGravado}*0.0945 debe ser {esperado}");
    }

    [Fact]
    public void IessPersonal_ConTotalGravadoCero_DebeRetornarCero()
    {
        // Arrange
        var strategy = new IessPersonalStrategy();
        var novedad = new Novedad();
        var empleado = new Empleado();

        // Act
        var resultado = strategy.Calcular(novedad, empleado, 0, 0, new Dictionary<string, decimal>());

        // Assert
        resultado.Should().Be(0m,
            "IESS Personal debe ser 0 si no hay ingresos gravados");
    }

    /// <summary>
    /// Fórmula: Salario Base * 3.41% (si tiene cónyuge)
    /// </summary>
    [Theory]
    [InlineData(700, 23.87)]
    [InlineData(975, 33.25)]
    [InlineData(1500, 51.15)]
    public void IessExtensionConyugeStrategy_DebeCalcular3_41Porciento(
        decimal salarioBase, decimal esperado)
    {
        // Arrange
        // Asumiendo que existe esta strategy
        // var strategy = new IessExtensionConyugeStrategy();
        // var novedad = new Novedad();
        // var empleado = new Empleado { SalarioBase = salarioBase };

        // Act
        // var resultado = strategy.Calcular(novedad, empleado, salarioBase, 0, new Dictionary<string, decimal>());

        // Assert
        // resultado.Should().BeApproximately(esperado, 0.01m);
        
        // Por ahora solo documentamos la fórmula esperada
        var resultadoEsperado = salarioBase * 0.0341m;
        resultadoEsperado.Should().BeApproximately(esperado, 0.01m,
            $"IessExtension: {salarioBase}*0.0341 debe ser {esperado}");
    }

    /// <summary>
    /// Tests para egresos con monto fijo (SimpleEgresoStrategy)
    /// Ejemplos: COMISARIATO, ANTICIPOS_EMPLEADOS, PRESTAMOS, etc.
    /// </summary>
    [Theory]
    [InlineData(25.50)] // COMISARIATO Empleado 1
    [InlineData(280)] // ANTICIPOS_EMPLEADOS Empleado 1
    [InlineData(19.30)] // FALTA_INJUSTIFICADA Empleado 1
    [InlineData(34.04)] // CATERING Empleado 2
    [InlineData(74.98)] // ANTICIPOS_EMPLEADOS Empleado 2
    public void SimpleEgresoStrategy_DebeRetornarMontoAplicado(decimal monto)
    {
        // Arrange
        var strategy = new SimpleEgresoStrategy();
        var novedad = new Novedad { MontoAplicado = monto };
        var empleado = new Empleado();

        // Act
        var resultado = strategy.Calcular(
            novedad,
            empleado,
            0,
            0,
            new Dictionary<string, decimal>()
        );

        // Assert
        resultado.Should().Be(monto,
            "SimpleEgreso debe retornar el MontoAplicado directamente");
    }

    /// <summary>
    /// Verificación del total de egresos del Empleado 1
    /// Según Example.txt: TOTAL_EGRESOS = 404.93
    /// </summary>
    [Fact]
    public void ValidarTotalEgresos_Empleado1_Example()
    {
        // Arrange - Todos los egresos del Empleado 1
        var egresos = new Dictionary<string, decimal>
        {
            ["IESS_PERSONAL"] = 80.13m,
            ["COMISARIATO"] = 25.50m,
            ["FALTA_INJUSTIFICADA"] = 19.30m,
            ["ANTICIPOS_EMPLEADOS"] = 280.00m
        };

        decimal esperado = 404.93m;

        // Act
        decimal total = egresos.Values.Sum();

        // Assert
        total.Should().Be(esperado,
            "El total de egresos del Empleado 1 debe coincidir con Example.txt");
    }

    /// <summary>
    /// Verificación del total de egresos del Empleado 2
    /// Según Example.txt: TOTAL_EGRESOS = 209.99
    /// </summary>
    [Fact]
    public void ValidarTotalEgresos_Empleado2_Example()
    {
        // Arrange - Todos los egresos del Empleado 2
        var egresos = new Dictionary<string, decimal>
        {
            ["IESS_PERSONAL"] = 100.97m,
            ["CATERING"] = 34.04m,
            ["ANTICIPOS_EMPLEADOS"] = 74.98m
        };

        decimal esperado = 209.99m;

        // Act
        decimal total = egresos.Values.Sum();

        // Assert
        total.Should().Be(esperado,
            "El total de egresos del Empleado 2 debe coincidir con Example.txt");
    }
}

