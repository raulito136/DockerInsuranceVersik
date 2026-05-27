using Claims.Application.DTOs.External;
using Refit;

namespace Claims.Application.Interfaces;

/// <summary>
/// Cliente HTTP tipado (Refit) para comunicarse con el Reference Data Service (Raúl, puerto 5003).
/// Se usa para validar que los códigos de status de claims existen y están activos.
/// </summary>
public interface IReferenceDataClient
{
    /// <summary>
    /// Valida que un código de status de claim existe y está activo.
    /// Se llama al crear/actualizar un claim o al cambiar su status.
    /// </summary>
    [Get("/api/v1/claim-statuses/by-code/{code}")]
    Task<ClaimStatusApiResponse> GetClaimStatusByCodeAsync(string code);
}
