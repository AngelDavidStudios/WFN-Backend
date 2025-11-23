using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Services;

public class ProvisionService: IProvisionService
{
    private readonly IProvisionRepository _provisionRepo;
    private readonly IEmpleadoRepository _empleadoRepo;

    public ProvisionService(
        IProvisionRepository provisionRepo,
        IEmpleadoRepository empleadoRepo)
    {
        _provisionRepo = provisionRepo;
        _empleadoRepo = empleadoRepo;
    }

    public async Task<IEnumerable<Provision>> GetByEmpleadoAsync(string empleadoId)
    {
        return await _provisionRepo.GetProvisionesByEmpleadoAsync(empleadoId);
    }

    public async Task<IEnumerable<Provision>> GetByPeriodoAsync(string empleadoId, string periodo)
    {
        return await _provisionRepo.GetProvisionesByPeriodoAsync(empleadoId, periodo);
    }

    public async Task<Provision?> GetByIdAsync(string empleadoId, string provisionId)
    {
        return await _provisionRepo.GetByIdAsync(empleadoId, provisionId);
    }

    public async Task<Provision> CreateAsync(Provision provision)
    {
        // Validar empleado
        var empleado = await _empleadoRepo.GetByIdAsync(provision.ID_Empleado);
        if (empleado == null)
            throw new ArgumentException("El empleado asociado a la provisión no existe.");

        // Generar ID
        provision.ID_Provision = Guid.NewGuid().ToString();

        // PK/SK
        provision.PK = $"EMP#{provision.ID_Empleado}";
        provision.SK = $"PROV#{provision.ID_Provision}";

        await _provisionRepo.AddAsync(provision);
        return provision;
    }

    public async Task<Provision> UpdateAsync(Provision provision)
    {
        var existing = await _provisionRepo.GetByIdAsync(provision.ID_Empleado, provision.ID_Provision);
        if (existing == null)
            throw new KeyNotFoundException("La provisión no existe.");

        provision.PK = $"EMP#{provision.ID_Empleado}";
        provision.SK = $"PROV#{provision.ID_Provision}";

        await _provisionRepo.UpdateAsync(provision);
        return provision;
    }

    public async Task<bool> DeleteAsync(string empleadoId, string provisionId)
    {
        var existing = await _provisionRepo.GetByIdAsync(empleadoId, provisionId);
        if (existing == null)
            return false;

        await _provisionRepo.DeleteAsync(empleadoId, provisionId);
        return true;
    }

    public async Task ProcesarProvisionesAsync(string empleadoId, string periodo)
    {
        var empleado = await _empleadoRepo.GetByIdAsync(empleadoId);
        if (empleado == null)
            throw new ArgumentException("El empleado no existe.");

        // Obtener las provisiones previas del empleado
        var anteriores = await _provisionRepo.GetProvisionesByEmpleadoAsync(empleadoId);

        // Obtener salario base
        decimal salario = empleado.SalarioBase;

        // DECIMO TERCERO (8.33% mensual si es anualizado)
        if (empleado.Is_DecimoTercMensual == false)
        {
            await ProcesarProvision(
                tipo: "DECIMO_TERCERO",
                empleadoId: empleadoId,
                salario: salario,
                periodo: periodo,
                porcentajeAnual: 0.0833m,
                anteriores
            );
        }

        // DECIMO CUARTO (valor fijo si es anualizado)
        // Ecuador: valor referencial de RMU, pero esto lo ajustas tú después.
        if (empleado.Is_DecimoCuartoMensual == false)
        {
            await ProcesarProvision(
                tipo: "DECIMO_CUARTO",
                empleadoId: empleadoId,
                salario: salario,
                periodo: periodo,
                porcentajeAnual: 1m / 12m,
                anteriores
            );
        }

        // FONDO DE RESERVA (8.33% si es anualizado)
        if (empleado.Is_FondoReserva == false)
        {
            await ProcesarProvision(
                tipo: "FONDO_RESERVA",
                empleadoId: empleadoId,
                salario: salario,
                periodo: periodo,
                porcentajeAnual: 0.0833m,
                anteriores
            );
        }
    }

    private async Task ProcesarProvision(
        string tipo,
        string empleadoId,
        decimal salario,
        string periodo,
        decimal porcentajeAnual,
        IEnumerable<Provision> anteriores)
    {
        // Buscar provisión anterior del mismo tipo
        var previa = anteriores.Where(p => p.TipoProvision == tipo).OrderByDescending(p => p.Periodo).FirstOrDefault();

        decimal valorMensual = Math.Round(salario * porcentajeAnual, 2);
        decimal acumuladoPrevio = previa?.Total ?? 0;
        decimal nuevoTotal = acumuladoPrevio + valorMensual;

        var provision = new Provision
        {
            ID_Provision = Guid.NewGuid().ToString(),
            ID_Empleado = empleadoId,
            TipoProvision = tipo,
            Periodo = periodo,
            ValorMensual = valorMensual,
            Acumulado = acumuladoPrevio,
            Total = nuevoTotal,
            IsTransferred = false,
            DetalleCalculo = $"Salario {salario} * {porcentajeAnual:P} = {valorMensual}"
        };

        provision.PK = $"EMP#{empleadoId}";
        provision.SK = $"PROV#{provision.ID_Provision}";

        await _provisionRepo.AddAsync(provision);
    }
}