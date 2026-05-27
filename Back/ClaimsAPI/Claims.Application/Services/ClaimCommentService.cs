using Claims.Application.Common;
using Claims.Application.DTOs;
using Claims.Application.Interfaces;
using Claims.Domain;

namespace Claims.Application.Services;

/// <summary>
/// Servicio de lógica de negocio para Comentarios de Claims.
/// Maneja listar, crear y eliminar comentarios.
/// Siempre verifica que el claim padre exista antes de operar.
/// </summary>
public class ClaimCommentService : IClaimCommentService
{
    private readonly IClaimCommentRepository _commentRepository;
    private readonly IClaimRepository _claimRepository;

    public ClaimCommentService(
        IClaimCommentRepository commentRepository,
        IClaimRepository claimRepository)
    {
        _commentRepository = commentRepository;
        _claimRepository = claimRepository;
    }

    public async Task<ServiceResult<List<CommentResponse>>> GetByClaimIdAsync(int claimId)
    {

        var claim = await _claimRepository.GetByIdAsync(claimId);
        if (claim == null)
            return ServiceResult<List<CommentResponse>>.Failure("ClaimId", "Claim not found", 404);

        var comments = await _commentRepository.GetByClaimIdAsync(claimId);

        var response = comments.Select(c => new CommentResponse
        {
            Id = c.Id,
            ClaimId = c.ClaimId,
            AuthorName = c.AuthorName,
            Comment = c.Comment,
            CreatedAt = c.CreatedAt
        }).ToList();

        return ServiceResult<List<CommentResponse>>.Success(response);
    }

    public async Task<ServiceResult<CommentResponse>> CreateAsync(int claimId, CreateCommentRequest request)
    {

        var claim = await _claimRepository.GetByIdAsync(claimId);
        if (claim == null)
            return ServiceResult<CommentResponse>.Failure("ClaimId", "Claim not found", 404);

        var errors = new List<ApiErrorItem>();

        if (string.IsNullOrWhiteSpace(request.AuthorName))
            errors.Add(new ApiErrorItem("AuthorName", "Author name is required"));

        if (string.IsNullOrWhiteSpace(request.Comment))
            errors.Add(new ApiErrorItem("Comment", "Comment is required"));

        if (errors.Count > 0)
            return ServiceResult<CommentResponse>.Failure(errors, 400);

        var comment = new ClaimComment
        {
            ClaimId = claimId,
            AuthorName = request.AuthorName,
            Comment = request.Comment,
            CreatedAt = DateTime.UtcNow
        };

        await _commentRepository.AddAsync(comment);

        var response = new CommentResponse
        {
            Id = comment.Id,
            ClaimId = comment.ClaimId,
            AuthorName = comment.AuthorName,
            Comment = comment.Comment,
            CreatedAt = comment.CreatedAt
        };

        return ServiceResult<CommentResponse>.Success(response, 201);
    }

    public async Task<ServiceResult> DeleteAsync(int claimId, int commentId)
    {

        var claim = await _claimRepository.GetByIdAsync(claimId);
        if (claim == null)
            return ServiceResult.Failure("ClaimId", "Claim not found", 404);

        var comment = await _commentRepository.GetByIdAsync(commentId);
        if (comment == null)
            return ServiceResult.Failure("CommentId", "Comment not found", 404);

        if (comment.ClaimId != claimId)
            return ServiceResult.Failure("CommentId", "Comment does not belong to this claim", 400);

        await _commentRepository.DeleteAsync(comment);

        return ServiceResult.Success(204);
    }
}
