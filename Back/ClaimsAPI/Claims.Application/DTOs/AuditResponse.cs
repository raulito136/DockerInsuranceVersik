namespace Claims.Application.DTOs;

/// <summary>
/// Lo que la API devuelve cuando el cliente pide el historial de auditoría de un claim.
/// Cada registro muestra qué campo cambió, el valor anterior y el nuevo.
/// </summary>
public class AuditResponse
{
    public int Id { get; set; }
    public int ClaimId { get; set; }
    public string ChangedBy { get; set; } = string.Empty;
    public string FieldChanged { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime ChangedAt { get; set; }
}
