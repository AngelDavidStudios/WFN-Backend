using FluentAssertions;
using Moq;
using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;
using WFNSystem.API.Services;
using WFNSystem.API.Services.Interfaces;
using WFNSystem.API.Services.Strategies;
using Microsoft.Extensions.Logging;
using Xunit;

namespace WFNSystem.Tests.Integration;

/// <summary>
/// Tests de integración completos basados en Example.txt
/// Valida el cálculo exacto de nómina para casos reales
/// </summary>
public class NominaIntegrationTests
{
    /// <summary>
    /// Test basado en Empleado 1 del Example.txt:
    /// LAVERDE VIQUE SOFIA CAROLINA
    /// - Décimos Mensualizados (Is_DecimoTercMensual = true, Is_DecimoCuartoMensual = true)
    /// - Sin Fondos de Reserva (antigüedad < 1 año)
    /// - Horas Extras: 5 al 50%, 6 al 100%
    /// </summary>
    [Fact]
    public async Task GenerarNomina_Empleado1_SofiaLaverde_DebeCalcularCorrectamente()
    {
        // ============================================================
        // ARRANGE - Configurar datos del Example.txt
        // ============================================================
        
        var empleado = new Empleado
        {
            ID_Empleado = "EMP001",
            ID_Persona = "PER001",
            ID_Departamento = "DEP001",
            PK = "EMP#EMP001",
            SK = "META#EMP",
            SalarioBase = 700m,
            FechaIngreso = new DateTime(2024, 11, 10), // Antigüedad < 1 año
            Is_DecimoTercMensual = true,  // Mensualizado
            Is_DecimoCuartoMensual = true, // Mensualizado
            Is_FondoReserva = false       // No aplica (< 1 año)
        };

        var periodo = "2025-11";

        // Novedades del empleado (horas extras + variable)
        var novedades = new List<Novedad>
        {
            new Novedad
            {
                ID_Novedad = "NOV001",
                ID_Parametro = "HORAS_EXTRAS_50",
                TipoNovedad = "INGRESO",
                MontoAplicado = 5m, // 5 horas
                Is_Gravable = true,
                Periodo = periodo
            },
            new Novedad
            {
                ID_Novedad = "NOV002",
                ID_Parametro = "HORAS_EXTRAS_100",
                TipoNovedad = "INGRESO",
                MontoAplicado = 6m, // 6 horas
                Is_Gravable = true,
                Periodo = periodo
            },
            new Novedad
            {
                ID_Novedad = "NOV003",
                ID_Parametro = "VARIABLE",
                TipoNovedad = "INGRESO",
                MontoAplicado = 91.10m,
                Is_Gravable = true,
                Periodo = periodo
            },
            new Novedad
            {
                ID_Novedad = "NOV004",
                ID_Parametro = "IESS_PERSONAL",
                TipoNovedad = "EGRESO",
                MontoAplicado = 0, // Se calcula automáticamente
                Is_Gravable = false,
                Periodo = periodo
            }
        };

        var parametros = new Dictionary<string, Parametro>
        {
            ["HORAS_EXTRAS_50"] = new Parametro 
            { 
                ID_Parametro = "HORAS_EXTRAS_50",
                Nombre = "HORAS_EXTRAS_50",
                Tipo = "INGRESO",
                TipoCalculo = "HORAS_EXTRAS_50"
            },
            ["HORAS_EXTRAS_100"] = new Parametro 
            { 
                ID_Parametro = "HORAS_EXTRAS_100",
                Nombre = "HORAS_EXTRAS_100",
                Tipo = "INGRESO",
                TipoCalculo = "HORAS_EXTRAS_100"
            },
            ["VARIABLE"] = new Parametro 
            { 
                ID_Parametro = "VARIABLE",
                Nombre = "VARIABLE",
                Tipo = "INGRESO",
                TipoCalculo = "VARIABLE"
            },
            ["IESS_PERSONAL"] = new Parametro 
            { 
                ID_Parametro = "IESS_PERSONAL",
                Nombre = "IESS_PERSONAL",
                Tipo = "IESS_PERSONAL",
                TipoCalculo = "IESS_PERSONAL"
            }
        };

        // ============================================================
        // ARRANGE - Configurar Mocks
        // ============================================================
        
        var mockNominaRepo = new Mock<INominaRepository>();
        var mockNovedadRepo = new Mock<INovedadRepository>();
        var mockEmpleadoRepo = new Mock<IEmpleadoRepository>();
        var mockParametroRepo = new Mock<IParametroRepository>();
        var mockProvisionRepo = new Mock<IProvisionRepository>();
        var mockProvisionService = new Mock<IProvisionService>();
        var mockWorkspaceService = new Mock<IWorkspaceService>();
        var mockLogger = new Mock<ILogger<NominaService>>();

        // Setup repositorios
        mockEmpleadoRepo.Setup(r => r.GetByIdAsync(empleado.ID_Empleado))
            .ReturnsAsync(empleado);

        mockNovedadRepo.Setup(r => r.GetByPeriodoAsync(empleado.ID_Empleado, periodo))
            .ReturnsAsync(novedades);

        mockParametroRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(parametros.Values.ToList());

        mockProvisionRepo.Setup(r => r.GetByTipoAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new List<Provision>());

        mockWorkspaceService.Setup(s => s.GetByPeriodoAsync(periodo))
            .ReturnsAsync(new WorkspaceNomina 
            { 
                Periodo = periodo, 
                Estado = 0 // Abierto
            });

        mockNominaRepo.Setup(r => r.AddAsync(It.IsAny<Nomina>()))
            .Returns(Task.CompletedTask);

        var strategyFactory = new StrategyFactory();

        var nominaService = new NominaService(
            mockNominaRepo.Object,
            mockNovedadRepo.Object,
            mockEmpleadoRepo.Object,
            mockParametroRepo.Object,
            mockProvisionRepo.Object,
            mockProvisionService.Object,
            strategyFactory,
            mockWorkspaceService.Object,
            mockLogger.Object
        );

        // ============================================================
        // ACT - Generar Nómina
        // ============================================================
        
        var nomina = await nominaService.GenerarNominaAsync(empleado.ID_Empleado, periodo);

        // ============================================================
        // ASSERT - Validar contra Example.txt
        // ============================================================
        
        // INGRESOS GRAVADOS IESS
        // Salario Base: 700
        // Horas Extras 50%: (700/30/8)*1.5*5 = 21.875 ≈ 21.88
        // Horas Extras 100%: (700/30/8)*2.0*6 = 35.00
        // Variable: 91.10
        // TOTAL GRAVADOS: 847.98
        nomina.TotalIngresosGravados.Should().Be(847.98m, 
            "debe coincidir con TOTAL_INGRESOS_GRAVADOS_IESS del Example.txt");

        // INGRESOS NO GRAVADOS
        // Décimo 3° Mensual: 847.98 / 12 = 70.665 ≈ 70.66
        // Décimo 4° Mensual: 470 / 12 = 39.166 ≈ 39.17
        // TOTAL NO GRAVADOS: 109.83
        nomina.TotalIngresosNoGravados.Should().BeApproximately(109.83m, 0.01m,
            "debe incluir décimos mensualizados como NO GRAVABLES");

        // TOTAL INGRESOS
        nomina.TotalIngresos.Should().BeApproximately(957.81m, 0.01m,
            "debe coincidir con TOTAL_INGRESOS del Example.txt");

        // EGRESOS
        // IESS Personal: 847.98 * 9.45% = 80.13
        nomina.IESS_AportePersonal.Should().BeApproximately(80.13m, 0.01m,
            "IESS Personal debe ser 9.45% del total gravados");

        // Validaciones adicionales
        nomina.ID_Empleado.Should().Be(empleado.ID_Empleado);
        nomina.Periodo.Should().Be(periodo);
        nomina.IsCerrada.Should().BeFalse();
    }

    /// <summary>
    /// Test basado en Empleado 2 del Example.txt:
    /// RODRIGUEZ ORELLANA CATALINA FERNANDA
    /// - Décimos Acumulados (Is_DecimoTercMensual = false, Is_DecimoCuartoMensual = false)
    /// - Fondos de Reserva Mensualizado (Is_FondoReserva = true)
    /// - Horas Extras: 2 al 50%, 10 al 100%
    /// </summary>
    [Fact]
    public async Task GenerarNomina_Empleado2_CatalinaRodriguez_DebeCalcularCorrectamente()
    {
        // ============================================================
        // ARRANGE
        // ============================================================
        
        var empleado = new Empleado
        {
            ID_Empleado = "EMP002",
            ID_Persona = "PER002",
            ID_Departamento = "DEP002",
            PK = "EMP#EMP002",
            SK = "META#EMP",
            SalarioBase = 975m,
            FechaIngreso = new DateTime(2021, 1, 1), // Antigüedad > 1 año
            Is_DecimoTercMensual = false,  // Acumulado
            Is_DecimoCuartoMensual = false, // Acumulado
            Is_FondoReserva = true          // Mensualizado
        };

        var periodo = "2025-11";

        var novedades = new List<Novedad>
        {
            new Novedad
            {
                ID_Parametro = "HORAS_EXTRAS_50",
                TipoNovedad = "INGRESO",
                MontoAplicado = 2m,
                Is_Gravable = true
            },
            new Novedad
            {
                ID_Parametro = "HORAS_EXTRAS_100",
                TipoNovedad = "INGRESO",
                MontoAplicado = 10m,
                Is_Gravable = true
            },
            new Novedad
            {
                ID_Parametro = "IESS_PERSONAL",
                TipoNovedad = "EGRESO",
                MontoAplicado = 0
            }
        };

        var parametros = new Dictionary<string, Parametro>
        {
            ["HORAS_EXTRAS_50"] = new Parametro 
            { 
                ID_Parametro = "HORAS_EXTRAS_50",
                TipoCalculo = "HORAS_EXTRAS_50",
                Tipo = "INGRESO"
            },
            ["HORAS_EXTRAS_100"] = new Parametro 
            { 
                ID_Parametro = "HORAS_EXTRAS_100",
                TipoCalculo = "HORAS_EXTRAS_100",
                Tipo = "INGRESO"
            },
            ["IESS_PERSONAL"] = new Parametro 
            { 
                ID_Parametro = "IESS_PERSONAL",
                TipoCalculo = "IESS_PERSONAL",
                Tipo = "IESS_PERSONAL"
            }
        };

        // Mocks
        var mockNominaRepo = new Mock<INominaRepository>();
        var mockNovedadRepo = new Mock<INovedadRepository>();
        var mockEmpleadoRepo = new Mock<IEmpleadoRepository>();
        var mockParametroRepo = new Mock<IParametroRepository>();
        var mockProvisionRepo = new Mock<IProvisionRepository>();
        var mockProvisionService = new Mock<IProvisionService>();
        var mockWorkspaceService = new Mock<IWorkspaceService>();
        var mockLogger = new Mock<ILogger<NominaService>>();

        mockEmpleadoRepo.Setup(r => r.GetByIdAsync(empleado.ID_Empleado))
            .ReturnsAsync(empleado);
        mockNovedadRepo.Setup(r => r.GetByPeriodoAsync(empleado.ID_Empleado, periodo))
            .ReturnsAsync(novedades);
        mockParametroRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(parametros.Values.ToList());
        mockProvisionRepo.Setup(r => r.GetByTipoAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new List<Provision>());
        mockWorkspaceService.Setup(s => s.GetByPeriodoAsync(periodo))
            .ReturnsAsync(new WorkspaceNomina { Periodo = periodo, Estado = 0 });
        mockNominaRepo.Setup(r => r.AddAsync(It.IsAny<Nomina>()))
            .Returns(Task.CompletedTask);

        var strategyFactory = new StrategyFactory();
        var nominaService = new NominaService(
            mockNominaRepo.Object,
            mockNovedadRepo.Object,
            mockEmpleadoRepo.Object,
            mockParametroRepo.Object,
            mockProvisionRepo.Object,
            mockProvisionService.Object,
            strategyFactory,
            mockWorkspaceService.Object,
            mockLogger.Object
        );

        // ============================================================
        // ACT
        // ============================================================
        
        var nomina = await nominaService.GenerarNominaAsync(empleado.ID_Empleado, periodo);

        // ============================================================
        // ASSERT - Validar contra Example.txt
        // ============================================================
        
        // INGRESOS GRAVADOS IESS
        // Salario Base: 975
        // Horas Extras 50%: (975/30/8)*1.5*2 = 12.1875 ≈ 12.19
        // Horas Extras 100%: (975/30/8)*2.0*10 = 81.25
        // TOTAL GRAVADOS: 1068.44
        nomina.TotalIngresosGravados.Should().BeApproximately(1068.44m, 0.01m,
            "debe coincidir con TOTAL_INGRESOS_GRAVADOS_IESS del Example.txt");

        // INGRESOS NO GRAVADOS
        // Fondos Reserva Mensual: 1068.44 * 8.33% = 88.99 ≈ 89.00
        // Décimo 3° y 4° NO se incluyen (se acumulan en provisiones)
        nomina.TotalIngresosNoGravados.Should().BeApproximately(89.00m, 0.01m,
            "debe incluir solo Fondos de Reserva mensualizado");

        // TOTAL INGRESOS
        nomina.TotalIngresos.Should().BeApproximately(1157.44m, 0.01m,
            "debe coincidir con TOTAL_INGRESOS del Example.txt");

        // EGRESOS
        // IESS Personal: 1068.44 * 9.45% = 100.97
        nomina.IESS_AportePersonal.Should().BeApproximately(100.97m, 0.01m,
            "IESS Personal debe ser 9.45% del total gravados");
    }
}

