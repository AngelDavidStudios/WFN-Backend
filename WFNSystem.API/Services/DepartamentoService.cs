using WFNSystem.API.Models;
using WFNSystem.API.Repository.Interfaces;
using WFNSystem.API.Services.Interfaces;

namespace WFNSystem.API.Services;

public class DepartamentoService: IDepartamentoService
{
    private readonly IDepartamentoRepository _departamentoRepo;

    public DepartamentoService(IDepartamentoRepository departamentoRepo)
    {
        _departamentoRepo = departamentoRepo;
    }

    public async Task<IEnumerable<Departamento>> GetAllAsync()
    {
        return await _departamentoRepo.GetAllAsync();
    }

    public async Task<Departamento?> GetByIdAsync(string deptoId)
    {
        return await _departamentoRepo.GetByIdAsync(deptoId);
    }

    public async Task<Departamento> CreateAsync(Departamento departamento)
    {
        // Validaciones
        ValidarDepartamento(departamento);

        // Crear ID único
        departamento.ID_Departamento = Guid.NewGuid().ToString();

        // Construcción de claves
        departamento.PK = $"DEP#{departamento.ID_Departamento}";
        departamento.SK = "META#DEP";

        // Fecha de creación
        departamento.DateCreated = DateTime.UtcNow.ToString("yyyy-MM-dd");

        await _departamentoRepo.AddAsync(departamento);
        return departamento;
    }

    public async Task<Departamento> UpdateAsync(Departamento departamento)
    {
        // Verificar si existe
        var exists = await _departamentoRepo.GetByIdAsync(departamento.ID_Departamento);
        if (exists == null)
            throw new Exception("El departamento que intentas actualizar no existe.");

        // Validaciones de negocio
        ValidarDepartamento(departamento);

        // Mantener claves correctas
        departamento.PK = $"DEP#{departamento.ID_Departamento}";
        departamento.SK = "META#DEP";

        // Mantener fecha de creación original
        departamento.DateCreated = exists.DateCreated;

        await _departamentoRepo.UpdateAsync(departamento);
        return departamento;
    }

    public async Task<bool> DeleteAsync(string departamentoId)
    {
        var exists = await _departamentoRepo.GetByIdAsync(departamentoId);
        if (exists == null)
            return false;

        await _departamentoRepo.DeleteAsync(departamentoId);
        return true;
    }

    // ============================================================
    // MÉTODOS PRIVADOS DE VALIDACIÓN
    // ============================================================

    private void ValidarDepartamento(Departamento departamento)
    {
        // Validar nombre
        if (string.IsNullOrWhiteSpace(departamento.Nombre))
            throw new ArgumentException("El nombre del departamento es requerido.");

        if (departamento.Nombre.Length < 3)
            throw new ArgumentException("El nombre del departamento debe tener al menos 3 caracteres.");

        // Validar email si se proporciona
        if (!string.IsNullOrWhiteSpace(departamento.Email))
        {
            if (!departamento.Email.Contains("@") || !departamento.Email.Contains("."))
                throw new ArgumentException("El email no tiene un formato válido.");
        }

        // Validar ubicación
        if (string.IsNullOrWhiteSpace(departamento.Ubicacion))
            throw new ArgumentException("La ubicación del departamento es requerida.");
    }
}