using Claims.Domain;

namespace Claims.Application.Interfaces;

/// <summary>
/// Contrato para acceder a los registros de auditoría de claims.
/// Solo lectura e inserción — los registros de auditoría nunca se modifican ni eliminan.
/// </summary>
public interface IClaimAuditRepository
{
    /// <summary>
    /// Lista todos los registros de auditoría de un claim.
    /// </summary>
    Task<List<ClaimAudit>> GetByClaimIdAsync(int claimId);

    /// <summary>
    /// Inserta un nuevo registro de auditoría.
    /// </summary>
    Task AddAsync(ClaimAudit audit);
}
