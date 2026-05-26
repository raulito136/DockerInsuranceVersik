using Claims.Domain;

namespace Claims.Application.Interfaces;

/// <summary>
/// Contrato para acceder a los comentarios de claims en la base de datos.
/// </summary>
public interface IClaimCommentRepository
{
    /// <summary>
    /// Lista todos los comentarios de un claim específico.
    /// </summary>
    Task<List<ClaimComment>> GetByClaimIdAsync(int claimId);

    /// <summary>
    /// Obtiene un comentario por su ID.
    /// </summary>
    Task<ClaimComment?> GetByIdAsync(int id);

    /// <summary>
    /// Inserta un nuevo comentario.
    /// </summary>
    Task AddAsync(ClaimComment comment);

    /// <summary>
    /// Elimina un comentario.
    /// </summary>
    Task DeleteAsync(ClaimComment comment);
}
