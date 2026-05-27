using Claims.Application.Common;
using Claims.Application.DTOs;

namespace Claims.Application.Interfaces;

/// <summary>
/// Contrato del servicio de lógica de negocio para Claims.
/// Cada método valida, ejecuta la lógica, y devuelve un ServiceResult (nunca lanza excepciones de negocio).
/// </summary>
public interface IClaimService
{
    /// <summary>
    /// Lista paginada de claims con filtros opcionales.
    /// </summary>
    Task<ServiceResult<PaginatedResponse<ClaimResponse>>> GetAllAsync(int page, int pageSize, string? statusCode, string? policyNumber);

    /// <summary>
    /// Obtiene un claim por ID, incluyendo sus comentarios y auditoría.
    /// </summary>
    Task<ServiceResult<ClaimResponse>> GetByIdAsync(int id);

    /// <summary>
    /// Obtiene un claim por su número legible (ej: CLM-2026-00042).
    /// </summary>
    Task<ServiceResult<ClaimResponse>> GetByClaimNumberAsync(string claimNumber);

    /// <summary>
    /// Crea un nuevo claim. Valida póliza contra Policies service, genera ClaimNumber, y pone status SUBMITTED.
    /// </summary>
    Task<ServiceResult<ClaimResponse>> CreateAsync(CreateClaimRequest request);

    /// <summary>
    /// Actualiza un claim existente. Valida póliza y registra cambios en auditoría.
    /// </summary>
    Task<ServiceResult<ClaimResponse>> UpdateAsync(int id, UpdateClaimRequest request);

    /// <summary>
    /// Cambia solo el status de un claim. Valida transición según workflow y código contra Reference Data.
    /// </summary>
    Task<ServiceResult<ClaimResponse>> UpdateStatusAsync(int id, UpdateStatusRequest request);

    /// <summary>
    /// Elimina un claim. Solo permitido cuando el status es SUBMITTED.
    /// </summary>
    Task<ServiceResult> DeleteAsync(int id);
}
