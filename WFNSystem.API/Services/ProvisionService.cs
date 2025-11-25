using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Services;

public class ProvisionService: IProvisionService
{
    private readonly IProvisionRepository _repo;
    private readonly IEmpleadoRepository _empleadoRepo;

    public ProvisionService(IProvisionRepository repo, IEmpleadoRepository empleadoRepo)
    {
        _repo = repo;
        _empleadoRepo = empleadoRepo;
    }

    public async Task<IEnumerable<Provision>> GetByEmpleadoAsync(string empleadoId)
    {
        return await _repo.GetByEmpleadoAsync(empleadoId);
    }

    public async Task<IEnumerable<Provision>> GetByPeriodoAsync(string empleadoId, string periodo)
    {
        periodo = periodo.Trim().ToUpper();
        return await _repo.GetByPeriodoAsync(empleadoId, periodo);
    }

    public async Task<IEnumerable<Provision>> GetByTipoAsync(string empleadoId, string tipoProvision)
    {
        tipoProvision = tipoProvision.Trim().ToUpper().Replace(" ", "_");
        return await _repo.GetByTipoAsync(empleadoId, tipoProvision);
    }

    public async Task<Provision?> GetByIdAsync(string empleadoId, string tipoProvision, string periodo)
    {
        tipoProvision = tipoProvision.Trim().ToUpper().Replace(" ", "_");
        periodo = periodo.Trim().ToUpper();

        return await _repo.GetByIdAsync(empleadoId, tipoProvision, periodo);
    }

    public async Task<Provision> CreateAsync(string empleadoId, Provision provision)
    {
        // ID único
        provision.ID_Provision = Guid.NewGuid().ToString();

        // Normalizar
        provision.TipoProvision = provision.TipoProvision.Trim().ToUpper().Replace(" ", "_");
        provision.Periodo = provision.Periodo.Trim().ToUpper();

        // Construir claves
        provision.PK = $"EMP#{empleadoId}";
        provision.SK = $"PROV#{provision.TipoProvision}#{provision.Periodo}";

        // Timestamp
        provision.FechaCalculo = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

        await _repo.AddAsync(provision);
        return provision;
    }

    public async Task<Provision> UpdateAsync(string empleadoId, Provision provision)
    {
        if (string.IsNullOrWhiteSpace(provision.ID_Provision))
            throw new Exception("No se puede actualizar una provisión sin ID.");

        if (string.IsNullOrWhiteSpace(provision.TipoProvision))
            throw new Exception("La provisión debe incluir TipoProvision.");

        if (string.IsNullOrWhiteSpace(provision.Periodo))
            throw new Exception("La provisión debe incluir Periodo.");

        provision.TipoProvision = provision.TipoProvision.Trim().ToUpper().Replace(" ", "_");
        provision.Periodo = provision.Periodo.Trim().ToUpper();

        var exists = await _repo.GetByIdAsync(empleadoId, provision.TipoProvision, provision.Periodo);
        if (exists == null)
            throw new Exception("La provisión no existe.");

        // Reconstrucción de claves
        provision.PK = $"EMP#{empleadoId}";
        provision.SK = $"PROV#{provision.TipoProvision}#{provision.Periodo}";

        await _repo.UpdateAsync(provision);
        return provision;
    }

    public async Task<bool> DeleteAsync(string empleadoId, string tipoProvision, string periodo)
    {
        tipoProvision = tipoProvision.Trim().ToUpper().Replace(" ", "_");
        periodo = periodo.Trim().ToUpper();

        var exists = await _repo.GetByIdAsync(empleadoId, tipoProvision, periodo);
        if (exists == null)
            return false;

        await _repo.DeleteAsync(empleadoId, tipoProvision, periodo);
        return true;
    }

    // ============================================
    //   PROCESO AUTOMÁTICO DE PROVISIONES ANUALES
    // ============================================
    public async Task ProcesarProvisionesAsync(string empleadoId, string periodo)
    {
        periodo = periodo.Trim().ToUpper();

        var empleado = await _empleadoRepo.GetByIdAsync(empleadoId);
        if (empleado == null)
            throw new Exception("No existe el empleado para calcular provisiones.");

        decimal salario = empleado.SalarioBase;
        int diasMes = 30;

        // Cálculo mensual aproximado
        decimal decimoTercero = salario / 12;
        decimal decimoCuarto = salario / 12;     // Luego puedes ajustar por región
        decimal fondos = salario * 0.0833m;      // 8.33%

        // Lista de provisiones a procesar
        var provisionesAProcesar = new List<(string Tipo, decimal ValorMensual)>
        {
            ("DECIMO_TERCERO_ACUMULADO", decimoTercero),
            ("DECIMO_CUARTO_ACUMULADO", decimoCuarto),
            ("FONDOS_RESERVA_ACUMULADO", fondos)
        };

        foreach (var p in provisionesAProcesar)
        {
            var existing = await _repo.GetByIdAsync(empleadoId, p.Tipo, periodo);

            if (existing == null)
            {
                // Crear nuevo registro
                var nueva = new Provision
                {
                    ID_Provision = Guid.NewGuid().ToString(),
                    PK = $"EMP#{empleadoId}",
                    SK = $"PROV#{p.Tipo}#{periodo}",
                    TipoProvision = p.Tipo,
                    Periodo = periodo,
                    ValorMensual = p.ValorMensual,
                    Acumulado = p.ValorMensual,
                    Total = p.ValorMensual,
                    FechaCalculo = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    IsTransferred = false
                };
                await _repo.AddAsync(nueva);
            }
            else
            {
                // Actualizar acumulados
                existing.ValorMensual = p.ValorMensual;
                existing.Acumulado += p.ValorMensual;
                existing.Total = existing.Acumulado;

                await _repo.UpdateAsync(existing);
            }
        }
    }
}