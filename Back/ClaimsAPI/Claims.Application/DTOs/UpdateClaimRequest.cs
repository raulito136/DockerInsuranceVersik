namespace Claims.Application.DTOs;

/// <summary>
/// Lo que envía el cliente para actualizar un claim existente (PUT).
/// Mismos campos que Create, pero se usa en contexto de actualización.
/// </summary>
public class UpdateClaimRequest
{
    public string PolicyNumber { get; set; } = string.Empty;
    public DateOnly ClaimDate { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
}
