namespace Claims.Application.DTOs.External;

/// <summary>
/// DTO para deserializar la respuesta del Policies Service (Oscar).
/// Cuando nuestro Claims Service necesita validar que una póliza existe y está activa,
/// llama a GET /api/v1/policies/by-number/{policyNumber} y recibe esto dentro del envelope.
/// </summary>
public class PolicyResponse
{
    public int Id { get; set; }
    public string PolicyNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal CoverageAmount { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}

/// <summary>
/// Envelope wrapper para la respuesta del Policies service.
/// El servicio devuelve: { "data": { ... } }
/// </summary>
public class PolicyApiResponse
{
    public PolicyResponse? Data { get; set; }
}
