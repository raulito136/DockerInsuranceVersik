namespace Claims.Application.DTOs;

/// <summary>
/// Lo que la API devuelve cuando el cliente pide los comentarios de un claim.
/// </summary>
public class CommentResponse
{
    public int Id { get; set; }
    public int ClaimId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
