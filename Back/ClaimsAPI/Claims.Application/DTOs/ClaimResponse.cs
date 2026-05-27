namespace Claims.Application.DTOs;

/// <summary>
/// Lo que la API devuelve cuando el cliente pide un claim.
/// Incluye todos los campos del claim más, opcionalmente, sus comentarios y auditoría.
/// </summary>
public class ClaimResponse
{
    public int Id { get; set; }
    public string ClaimNumber { get; set; } = string.Empty;
    public string PolicyNumber { get; set; } = string.Empty;
    public DateOnly ClaimDate { get; set; }
    public string StatusCode { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Solo se incluyen cuando se pide un claim individual (GET by ID).
    /// En listados paginados son null para no sobrecargar la respuesta.
    /// </summary>
    public List<CommentResponse>? Comments { get; set; }
    public List<AuditResponse>? Audits { get; set; }
}
