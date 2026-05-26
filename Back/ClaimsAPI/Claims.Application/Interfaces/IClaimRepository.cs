using Claims.Domain;

namespace Claims.Application.Interfaces;

/// <summary>
/// Contrato para acceder a los datos de Claims en la base de datos.
/// La implementación real (con Entity Framework) irá en la capa Infrastructure.
/// Definir la interfaz aquí permite que la lógica de negocio no dependa de EF Core.
/// </summary>
public interface IClaimRepository
{
    /// <summary>
    /// Lista paginada y filtrable de claims.
    /// </summary>
    /// <param name="page">Número de página (empieza en 1)</param>
    /// <param name="pageSize">Cantidad de registros por página</param>
    /// <param name="statusCode">Filtro opcional por código de status</param>
    /// <param name="policyNumber">Filtro opcional por número de póliza</param>
    /// <returns>Tupla con la lista de claims y el total de registros</returns>
    Task<(List<Claim> Claims, int Total)> GetAllAsync(int page, int pageSize, string? statusCode, string? policyNumber);

    /// <summary>
    /// Obtiene un claim por su ID interno.
    /// </summary>
    Task<Claim?> GetByIdAsync(int id);

    /// <summary>
    /// Obtiene un claim por su número legible (ej: CLM-2026-00042).
    /// </summary>
    Task<Claim?> GetByClaimNumberAsync(string claimNumber);

    /// <summary>
    /// Inserta un nuevo claim en la base de datos.
    /// </summary>
    Task AddAsync(Claim claim);

    /// <summary>
    /// Actualiza un claim existente.
    /// </summary>
    Task UpdateAsync(Claim claim);

    /// <summary>
    /// Elimina un claim de la base de datos.
    /// </summary>
    Task DeleteAsync(Claim claim);

    /// <summary>
    /// Obtiene el siguiente número de secuencia para generar el ClaimNumber del año dado.
    /// Ejemplo: si el último claim del 2026 es CLM-2026-00042, devuelve 43.
    /// </summary>
    Task<int> GetNextSequenceNumberAsync(int year);
}
