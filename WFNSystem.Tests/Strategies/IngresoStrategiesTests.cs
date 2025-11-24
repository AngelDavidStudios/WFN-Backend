using FluentAssertions;
using WFNSystem.API.Models;
using WFNSystem.API.Services.Strategies.Ingresos;
using Xunit;

namespace WFNSystem.Tests.Strategies;

/// <summary>
/// Tests unitarios para Strategies de Ingresos
/// Valida fórmulas exactas según Tabla referencia.txt
/// </summary>
public class IngresoStrategiesTests
{
    /// <summary>
    /// Fórmula: (((Salario Base/30/8) * 50%) + (Salario Base/30/8)) * NRO50
    /// Simplificado: (Salario Base/30/8) * 1.5 * NRO50
    /// </summary>
    [Theory]
    [InlineData(700, 5, 21.88)] // Empleado 1 Example.txt
    [InlineData(975, 2, 12.19)] // Empleado 2 Example.txt
    [InlineData(460, 10, 28.75)] // SBU con 10 horas
    public void HorasExtras50Strategy_DebeCalcularCorrectamente(
        decimal salarioBase, decimal horas, decimal esperado)
    {
        // Arrange
        var strategy = new HorasExtras50Strategy();
        var novedad = new Novedad { MontoAplicado = horas };
        var empleado = new Empleado { SalarioBase = salarioBase };

        // Act
        var resultado = strategy.Calcular(
            novedad,
            empleado,
            salarioBase,
            0,
            new Dictionary<string, decimal>()
        );

        // Assert
        resultado.Should().BeApproximately(esperado, 0.01m,
            $"HorasExtras50: ({salarioBase}/30/8)*1.5*{horas} debe ser {esperado}");
    }

    /// <summary>
    /// Fórmula: (((Salario Base/30/8) * 100%) + (Salario Base/30/8)) * NRO100
    /// Simplificado: (Salario Base/30/8) * 2.0 * NRO100
    /// </summary>
    [Theory]
    [InlineData(700, 6, 35.00)] // Empleado 1 Example.txt
    [InlineData(975, 10, 81.25)] // Empleado 2 Example.txt
    [InlineData(460, 8, 30.67)] // SBU con 8 horas
    public void HorasExtras100Strategy_DebeCalcularCorrectamente(
        decimal salarioBase, decimal horas, decimal esperado)
    {
        // Arrange
        var strategy = new HorasExtras100Strategy();
        var novedad = new Novedad { MontoAplicado = horas };
        var empleado = new Empleado { SalarioBase = salarioBase };

        // Act
        var resultado = strategy.Calcular(
            novedad,
            empleado,
            salarioBase,
            0,
            new Dictionary<string, decimal>()
        );

        // Assert
        resultado.Should().BeApproximately(esperado, 0.01m,
            $"HorasExtras100: ({salarioBase}/30/8)*2.0*{horas} debe ser {esperado}");
    }

    /// <summary>
    /// Fórmula: TOTAL INGRESOS GRAVADOS IESS / 12
    /// </summary>
    [Theory]
    [InlineData(847.98, 70.66)] // Empleado 1 Example.txt
    [InlineData(1068.44, 89.04)] // Empleado 2 Example.txt
    [InlineData(460, 38.33)] // SBU
    public void DecimoTerceroStrategy_DebeCalcularCorrectamente(
        decimal totalGravado, decimal esperado)
    {
        // Arrange
        var strategy = new DecimoTerceroStrategy();
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
            $"DecimoTercero: {totalGravado}/12 debe ser {esperado}");
    }

    /// <summary>
    /// Fórmula: Salario Canasta Básica (470) / 12
    /// </summary>
    [Fact]
    public void DecimoCuartoStrategy_DebeCalcularCorrectamente()
    {
        // Arrange
        var strategy = new DecimoCuartoStrategy();
        var novedad = new Novedad();
        var empleado = new Empleado();
        var parametros = new Dictionary<string, decimal>
        {
            ["DECIMO_CUARTO_BASE"] = 470m
        };

        decimal esperado = 39.17m; // 470 / 12 = 39.166...

        // Act
        var resultado = strategy.Calcular(
            novedad,
            empleado,
            0,
            0,
            parametros
        );

        // Assert
        resultado.Should().BeApproximately(esperado, 0.01m,
            "DecimoCuarto: 470/12 debe ser 39.17");
    }

    /// <summary>
    /// Fórmula: TOTAL INGRESOS GRAVADOS IESS * 8.33%
    /// </summary>
    [Theory]
    [InlineData(847.98, 70.64)] // Empleado 1 Example.txt (pero no aplica < 1 año)
    [InlineData(1068.44, 89.00)] // Empleado 2 Example.txt
    [InlineData(1500, 124.95)]
    public void FondosReservaStrategy_DebeCalcularCorrectamente(
        decimal totalGravado, decimal esperado)
    {
        // Arrange
        var strategy = new FondosReservaStrategy();
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
            $"FondosReserva: {totalGravado}*0.0833 debe ser {esperado}");
    }

    /// <summary>
    /// Fórmula: Retorna directamente el MontoAplicado
    /// </summary>
    [Theory]
    [InlineData(91.10)] // Variable Empleado 1
    [InlineData(100.50)]
    [InlineData(250.75)]
    public void SimpleStrategy_DebeRetornarMontoAplicado(decimal monto)
    {
        // Arrange
        var strategy = new SimpleStrategy();
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
            "SimpleStrategy debe retornar el MontoAplicado directamente");
    }

    [Fact]
    public void HorasExtras50_ConHorasCero_DebeRetornarCero()
    {
        // Arrange
        var strategy = new HorasExtras50Strategy();
        var novedad = new Novedad { MontoAplicado = 0 };
        var empleado = new Empleado { SalarioBase = 700 };

        // Act
        var resultado = strategy.Calcular(novedad, empleado, 700, 0, new Dictionary<string, decimal>());

        // Assert
        resultado.Should().Be(0m);
    }

    [Fact]
    public void HorasExtras100_ConSalarioBaseCero_DebeRetornarCero()
    {
        // Arrange
        var strategy = new HorasExtras100Strategy();
        var novedad = new Novedad { MontoAplicado = 10 };
        var empleado = new Empleado { SalarioBase = 0 };

        // Act
        var resultado = strategy.Calcular(novedad, empleado, 0, 0, new Dictionary<string, decimal>());

        // Assert
        resultado.Should().Be(0m);
    }
}

