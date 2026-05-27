using Claims.Application.Common;
using Claims.Application.DTOs;

namespace Claims.Application.Interfaces;

/// <summary>
/// Contrato del servicio de lógica de negocio para Comentarios de Claims.
/// </summary>
public interface IClaimCommentService
{
    /// <summary>
    /// Lista todos los comentarios de un claim.
    /// </summary>
    Task<ServiceResult<List<CommentResponse>>> GetByClaimIdAsync(int claimId);

    /// <summary>
    /// Agrega un comentario a un claim.
    /// </summary>
    Task<ServiceResult<CommentResponse>> CreateAsync(int claimId, CreateCommentRequest request);

    /// <summary>
    /// Elimina un comentario de un claim.
    /// </summary>
    Task<ServiceResult> DeleteAsync(int claimId, int commentId);
}
