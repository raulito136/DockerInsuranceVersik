namespace Claims.Application.DTOs;

/// <summary>
/// Lo que envía el cliente para agregar un comentario a un claim.
/// </summary>
public class CreateCommentRequest
{
    public string AuthorName { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
}
