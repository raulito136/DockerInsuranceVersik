namespace Claims.Application.DTOs;

/// <summary>
/// Lo que envía el cliente para cambiar solo el status de un claim (PATCH).
/// Incluye StatusCode (el nuevo estado) y ChangedBy (quién hace el cambio — para auditoría).
/// </summary>
public class UpdateStatusRequest
{
    public string StatusCode { get; set; } = string.Empty;
    public string ChangedBy { get; set; } = string.Empty;
}
