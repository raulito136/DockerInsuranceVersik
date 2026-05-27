using Claims.Application.Common;
using Claims.Application.DTOs;
using Claims.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Api.Controllers;

/// <summary>
/// Controller principal del Claims Service.
/// 
/// ¿Qué hace un controller?
/// Es la "puerta de entrada" de la API. Recibe las peticiones HTTP,
/// las traduce a llamadas al servicio de aplicación, y convierte
/// el resultado en una respuesta HTTP con el status code correcto.
/// 
/// [Route("api/v1/claims")] — Todas las rutas empiezan con /api/v1/claims
/// [ApiController] — Habilita validación automática del model binding
/// </summary>
[Route("api/v1/claims")]
[ApiController]
public class ClaimsController : ControllerBase
{
    private readonly IClaimService _claimService;
    private readonly IClaimCommentService _commentService;
    private readonly IClaimAuditRepository _auditRepository;
    private readonly IClaimCommentRepository _commentRepository;

    public ClaimsController(
        IClaimService claimService,
        IClaimCommentService commentService,
        IClaimAuditRepository auditRepository,
        IClaimCommentRepository commentRepository)
    {
        _claimService = claimService;
        _commentService = commentService;
        _auditRepository = auditRepository;
        _commentRepository = commentRepository;
    }

    /// <summary>
    /// GET /api/v1/claims?page=1&pageSize=20&statusCode=SUBMITTED&policyNumber=POL-001
    /// Lista paginada de claims con filtros opcionales.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? statusCode = null,
        [FromQuery] string? policyNumber = null)
    {
        var result = await _claimService.GetAllAsync(page, pageSize, statusCode, policyNumber);
        return StatusCode(result.StatusCode, result.Data);
    }

    /// <summary>
    /// GET /api/v1/claims/{id}
    /// Obtiene un claim por ID, incluyendo sus comentarios y auditoría.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _claimService.GetByIdAsync(id);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, ApiResponse<object>.Error(result.Errors));

        var comments = await _commentRepository.GetByClaimIdAsync(id);
        var audits = await _auditRepository.GetByClaimIdAsync(id);

        result.Data!.Comments = comments.Select(c => new CommentResponse
        {
            Id = c.Id,
            ClaimId = c.ClaimId,
            AuthorName = c.AuthorName,
            Comment = c.Comment,
            CreatedAt = c.CreatedAt
        }).ToList();

        result.Data.Audits = audits.Select(a => new AuditResponse
        {
            Id = a.Id,
            ClaimId = a.ClaimId,
            ChangedBy = a.ChangedBy,
            FieldChanged = a.FieldChanged,
            OldValue = a.OldValue,
            NewValue = a.NewValue,
            ChangedAt = a.ChangedAt
        }).ToList();

        return Ok(ApiResponse<ClaimResponse>.Success(result.Data));
    }

    /// <summary>
    /// GET /api/v1/claims/by-number/{claimNumber}
    /// Obtiene un claim por su número legible (ej: CLM-2026-00001).
    /// </summary>
    [HttpGet("by-number/{claimNumber}")]
    public async Task<IActionResult> GetByClaimNumber(string claimNumber)
    {
        var result = await _claimService.GetByClaimNumberAsync(claimNumber);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, ApiResponse<object>.Error(result.Errors));

        return Ok(ApiResponse<ClaimResponse>.Success(result.Data!));
    }

    /// <summary>
    /// POST /api/v1/claims
    /// Crea un nuevo claim. El body debe contener PolicyNumber, ClaimDate, Amount y Description.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClaimRequest request)
    {
        var result = await _claimService.CreateAsync(request);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, ApiResponse<object>.Error(result.Errors));

        return StatusCode(201, ApiResponse<ClaimResponse>.Success(result.Data!));
    }

    /// <summary>
    /// PUT /api/v1/claims/{id}
    /// Actualiza un claim existente. Registra cambios en auditoría automáticamente.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateClaimRequest request)
    {
        var result = await _claimService.UpdateAsync(id, request);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, ApiResponse<object>.Error(result.Errors));

        return Ok(ApiResponse<ClaimResponse>.Success(result.Data!));
    }

    /// <summary>
    /// PATCH /api/v1/claims/{id}/status
    /// Cambia solo el status de un claim. Valida la transición según el workflow.
    /// Body: { "statusCode": "UNDER_REVIEW", "changedBy": "admin" }
    /// </summary>
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
    {
        var result = await _claimService.UpdateStatusAsync(id, request);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, ApiResponse<object>.Error(result.Errors));

        return Ok(ApiResponse<ClaimResponse>.Success(result.Data!));
    }

    /// <summary>
    /// DELETE /api/v1/claims/{id}
    /// Elimina un claim. Solo permitido cuando el status es SUBMITTED.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _claimService.DeleteAsync(id);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, ApiResponse<object>.Error(result.Errors));

        return NoContent();
    }

    /// <summary>
    /// GET /api/v1/claims/{id}/comments
    /// Lista todos los comentarios de un claim.
    /// </summary>
    [HttpGet("{id}/comments")]
    public async Task<IActionResult> GetComments(int id)
    {
        var result = await _commentService.GetByClaimIdAsync(id);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, ApiResponse<object>.Error(result.Errors));

        return Ok(ApiResponse<List<CommentResponse>>.Success(result.Data!));
    }

    /// <summary>
    /// POST /api/v1/claims/{id}/comments
    /// Agrega un comentario a un claim.
    /// Body: { "authorName": "David", "comment": "Revisión iniciada" }
    /// </summary>
    [HttpPost("{id}/comments")]
    public async Task<IActionResult> AddComment(int id, [FromBody] CreateCommentRequest request)
    {
        var result = await _commentService.CreateAsync(id, request);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, ApiResponse<object>.Error(result.Errors));

        return StatusCode(201, ApiResponse<CommentResponse>.Success(result.Data!));
    }

    /// <summary>
    /// DELETE /api/v1/claims/{id}/comments/{commentId}
    /// Elimina un comentario de un claim.
    /// </summary>
    [HttpDelete("{id}/comments/{commentId}")]
    public async Task<IActionResult> DeleteComment(int id, int commentId)
    {
        var result = await _commentService.DeleteAsync(id, commentId);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, ApiResponse<object>.Error(result.Errors));

        return NoContent();
    }

    /// <summary>
    /// GET /api/v1/claims/{id}/audit
    /// Obtiene el historial completo de auditoría de un claim.
    /// Muestra todos los cambios que se han hecho: quién, qué campo, valor anterior y nuevo.
    /// </summary>
    [HttpGet("{id}/audit")]
    public async Task<IActionResult> GetAudit(int id)
    {
        var claimResult = await _claimService.GetByIdAsync(id);
        if (!claimResult.IsSuccess)
            return StatusCode(claimResult.StatusCode, ApiResponse<object>.Error(claimResult.Errors));

        var audits = await _auditRepository.GetByClaimIdAsync(id);

        var response = audits.Select(a => new AuditResponse
        {
            Id = a.Id,
            ClaimId = a.ClaimId,
            ChangedBy = a.ChangedBy,
            FieldChanged = a.FieldChanged,
            OldValue = a.OldValue,
            NewValue = a.NewValue,
            ChangedAt = a.ChangedAt
        }).ToList();

        return Ok(ApiResponse<List<AuditResponse>>.Success(response));
    }
}
