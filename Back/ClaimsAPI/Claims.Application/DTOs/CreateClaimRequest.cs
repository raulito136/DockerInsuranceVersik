namespace Claims.Application.DTOs;

/// <summary>
/// Lo que envía el cliente para crear un nuevo claim.
/// No incluye ClaimNumber (se auto-genera), StatusCode (siempre empieza en SUBMITTED),
/// ni timestamps (los pone el servidor).
/// </summary>
public class CreateClaimRequest
{
    public string PolicyNumber { get; set; } = string.Empty;
    public DateOnly ClaimDate { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
}
