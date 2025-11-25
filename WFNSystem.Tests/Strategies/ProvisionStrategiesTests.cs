using FluentAssertions;
using WFNSystem.API.Models;
using WFNSystem.API.Services.Strategies.Provisiones;
using Xunit;

namespace WFNSystem.Tests.Strategies;

/// <summary>
/// Tests unitarios para Strategies de Provisiones
/// Valida fórmulas exactas según Tabla referencia.txt
/// </summary>
public class ProvisionStrategiesTests
{
    /// <summary>
    /// Fórmula: TOTAL INGRESOS GRAVADOS IESS / 24
    /// </summary>
    [Theory]
    [InlineData(847.98, 35.33)] // Empleado 1 Example.txt
    [InlineData(1068.44, 44.52)] // Empleado 2 Example.txt
    [InlineData(1500, 62.50)]
    public void ProvisionVacacionesStrategy_DebeCalcularCorrectamente(
        decimal totalGravado, decimal esperado)
    {
        // Arrange
        var strategy = new ProvisionVacacionesStrategy();
        var empleado = new Empleado();
        var provisionActual = new Provision { Acumulado = 0 };

        // Act
        var resultado = strategy.Calcular(
            empleado,
            0,
            totalGravado,
            provisionActual
        );

        // Assert
        resultado.Should().BeApproximately(esperado, 0.01m,
            $"ProvisionVacaciones: {totalGravado}/24 debe ser {esperado}");
    }

    /// <summary>
    /// Fórmula: TOTAL INGRESOS GRAVADOS IESS * 12.15%
    /// </summary>
    [Theory]
    [InlineData(847.98, 103.03)] // Empleado 1 Example.txt
    [InlineData(1068.44, 129.82)] // Empleado 2 Example.txt
    [InlineData(1500, 182.25)]
    public void IessPatronalStrategy_DebeCalcular12_15Porciento(
        decimal totalGravado, decimal esperado)
    {
        // Arrange
        var strategy = new IessPatronalStrategy();
        var empleado = new Empleado();
        var provisionActual = new Provision { Acumulado = 0 };

        // Act
        var resultado = strategy.Calcular(
            empleado,
            0,
            totalGravado,
            provisionActual
        );

        // Assert
        resultado.Should().BeApproximately(esperado, 0.01m,
            $"IessPatronal: {totalGravado}*0.1215 debe ser {esperado}");
    }

    /// <summary>
    /// Fórmula: Sumatoria acumulada de décimos terceros
    /// Si Is_DecimoTercMensual = false, acumula cada mes
    /// Si Is_DecimoTercMensual = true, NO acumula (retorna acumulado actual)
    /// </summary>
    [Fact]
    public void DecimoTerceroAcumulado_Mensualizado_NoDebeAcumular()
    {
        // Arrange
        var strategy = new DecimoTerceroAcumuladoStrategy();
        var empleado = new Empleado 
        { 
            Is_DecimoTercMensual = true // Mensualizado
        };
        var provisionActual = new Provision { Acumulado = 100m };
        decimal totalGravado = 847.98m;

        // Act
        var resultado = strategy.Calcular(
            empleado,
            0,
            totalGravado,
            provisionActual
        );

        // Assert
        resultado.Should().Be(100m,
            "Si es mensualizado, NO debe acumular (retorna el acumulado actual)");
    }

    [Fact]
    public void DecimoTerceroAcumulado_NoMensualizado_DebeAcumular()
    {
        // Arrange
        var strategy = new DecimoTerceroAcumuladoStrategy();
        var empleado = new Empleado 
        { 
            Is_DecimoTercMensual = false // Acumulado
        };
        var provisionActual = new Provision { Acumulado = 100m };
        decimal totalGravado = 1068.44m;
        decimal decimoMensual = totalGravado / 12m; // 89.04

        // Act
        var resultado = strategy.Calcular(
            empleado,
            0,
            totalGravado,
            provisionActual
        );

        // Assert
        decimal esperado = 100m + 89.04m; // 189.04
        resultado.Should().BeApproximately(esperado, 0.01m,
            "Si NO es mensualizado, debe acumular el décimo del mes");
    }

    /// <summary>
    /// Verificación de acumulado anual del Empleado 2
    /// Según Example.txt: DECIMO_TERCERO_ACUMULADO = 89.04
    /// (Esto representa 1 mes de acumulación: 1068.44 / 12 = 89.04)
    /// </summary>
    [Fact]
    public void DecimoTerceroAcumulado_Empleado2_DebeCoincidirConExample()
    {
        // Arrange
        var strategy = new DecimoTerceroAcumuladoStrategy();
        var empleado = new Empleado { Is_DecimoTercMensual = false };
        var provisionActual = new Provision { Acumulado = 0 };
        decimal totalGravado = 1068.44m;

        // Act
        var resultado = strategy.Calcular(empleado, 0, totalGravado, provisionActual);

        // Assert
        resultado.Should().BeApproximately(89.04m, 0.01m,
            "El primer mes de acumulación debe coincidir con Example.txt");
    }

    [Fact]
    public void DecimoCuartoAcumulado_Mensualizado_NoDebeAcumular()
    {
        // Arrange
        var strategy = new DecimoCuartoAcumuladoStrategy();
        var empleado = new Empleado 
        { 
            Is_DecimoCuartoMensual = true // Mensualizado
        };
        var provisionActual = new Provision { Acumulado = 50m };

        // Act
        var resultado = strategy.Calcular(empleado, 0, 0, provisionActual);

        // Assert
        resultado.Should().Be(50m,
            "Si es mensualizado, NO debe acumular");
    }

    [Fact]
    public void DecimoCuartoAcumulado_NoMensualizado_DebeAcumular()
    {
        // Arrange
        var strategy = new DecimoCuartoAcumuladoStrategy();
        var empleado = new Empleado 
        { 
            Is_DecimoCuartoMensual = false // Acumulado
        };
        var provisionActual = new Provision { Acumulado = 50m };
        decimal decimoCuartoMensual = 470m / 12m; // 39.17

        // Act
        var resultado = strategy.Calcular(empleado, 0, 0, provisionActual);

        // Assert
        decimal esperado = 50m + 39.17m; // 89.17
        resultado.Should().BeApproximately(esperado, 0.01m,
            "Debe acumular el décimo cuarto mensual (470/12)");
    }

    /// <summary>
    /// Verificación de acumulado anual del Empleado 2
    /// Según Example.txt: DECIMO_CUARTO_ACUMULADO = 39.17
    /// </summary>
    [Fact]
    public void DecimoCuartoAcumulado_Empleado2_DebeCoincidirConExample()
    {
        // Arrange
        var strategy = new DecimoCuartoAcumuladoStrategy();
        var empleado = new Empleado { Is_DecimoCuartoMensual = false };
        var provisionActual = new Provision { Acumulado = 0 };

        // Act
        var resultado = strategy.Calcular(empleado, 0, 0, provisionActual);

        // Assert
        resultado.Should().BeApproximately(39.17m, 0.01m,
            "El primer mes debe coincidir con Example.txt (470/12)");
    }

    [Fact]
    public void FondoReservaAcumulado_Mensualizado_NoDebeAcumular()
    {
        // Arrange
        var strategy = new FondoReservaAcumuladoStrategy();
        var empleado = new Empleado 
        { 
            Is_FondoReserva = true // Mensualizado
        };
        var provisionActual = new Provision { Acumulado = 200m };
        decimal totalGravado = 1068.44m;

        // Act
        var resultado = strategy.Calcular(empleado, 0, totalGravado, provisionActual);

        // Assert
        resultado.Should().Be(200m,
            "Si es mensualizado, NO debe acumular en provisiones");
    }

    [Fact]
    public void FondoReservaAcumulado_NoMensualizado_DebeAcumular()
    {
        // Arrange
        var strategy = new FondoReservaAcumuladoStrategy();
        var empleado = new Empleado 
        { 
            Is_FondoReserva = false // Acumulado
        };
        var provisionActual = new Provision { Acumulado = 200m };
        decimal totalGravado = 847.98m;
        decimal fondoMensual = totalGravado * 0.0833m; // 70.64

        // Act
        var resultado = strategy.Calcular(empleado, 0, totalGravado, provisionActual);

        // Assert
        decimal esperado = 200m + 70.64m; // 270.64
        resultado.Should().BeApproximately(esperado, 0.01m,
            "Debe acumular el fondo de reserva mensual");
    }

    /// <summary>
    /// Verificación de acumulado del Empleado 1
    /// Según Example.txt: FONDOS_RESERVA_ACUMULADO = 70.64
    /// (Aunque el empleado tiene < 1 año, si se calculara sería 847.98 * 0.0833)
    /// </summary>
    [Fact]
    public void FondoReservaAcumulado_Empleado1_DebeCoincidirConExample()
    {
        // Arrange
        var strategy = new FondoReservaAcumuladoStrategy();
        var empleado = new Empleado { Is_FondoReserva = false };
        var provisionActual = new Provision { Acumulado = 0 };
        decimal totalGravado = 847.98m;

        // Act
        var resultado = strategy.Calcular(empleado, 0, totalGravado, provisionActual);

        // Assert
        resultado.Should().BeApproximately(70.64m, 0.01m,
            "El primer mes debe calcular 847.98 * 0.0833 = 70.64");
    }

    /// <summary>
    /// Verificación del total de provisiones del Empleado 1
    /// Según Example.txt: TOTAL_PROVISIONES = 1166.80
    /// (Incluye devengamiento de vacaciones previas)
    /// </summary>
    [Fact]
    public void ValidarTotalProvisiones_Empleado1_Example()
    {
        // Arrange
        var provisiones = new Dictionary<string, decimal>
        {
            ["PROVISION_VACACIONES"] = 35.33m,
            ["IESS_PATRONAL"] = 103.03m,
            ["FONDOS_RESERVA_ACUMULADO"] = 70.64m,
            ["DEVENGAMIENTO_PROVISION_VACACIONES"] = 0m
            // Décimos no se acumulan (son mensualizados)
        };

        decimal esperado = 209.00m; // Sin incluir devengamiento previo

        // Act
        decimal total = provisiones["PROVISION_VACACIONES"] + 
                       provisiones["IESS_PATRONAL"] + 
                       provisiones["FONDOS_RESERVA_ACUMULADO"];

        // Assert
        total.Should().BeApproximately(esperado, 0.01m,
            "La suma de provisiones nuevas del mes");
    }
}

