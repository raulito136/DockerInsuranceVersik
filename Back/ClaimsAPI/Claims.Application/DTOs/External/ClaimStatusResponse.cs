namespace Claims.Application.DTOs.External;

/// <summary>
/// DTO para deserializar la respuesta del Reference Data Service (Raúl).
/// Cuando nuestro Claims Service necesita validar que un código de status existe,
/// llama a GET /api/v1/claim-statuses/by-code/{code} y recibe esto dentro del envelope.
/// </summary>
public class ClaimStatusResponse
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

/// <summary>
/// Envelope wrapper para la respuesta del Reference Data service.
/// El servicio devuelve: { "data": { ... } }
/// </summary>
public class ClaimStatusApiResponse
{
    public ClaimStatusResponse? Data { get; set; }
}
