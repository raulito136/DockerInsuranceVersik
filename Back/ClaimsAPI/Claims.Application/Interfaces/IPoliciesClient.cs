using Claims.Application.DTOs.External;
using Refit;

namespace Claims.Application.Interfaces;

/// <summary>
/// Cliente HTTP tipado (Refit) para comunicarse con el Policies Service (Oscar, puerto 5002).
/// Solo definimos el método que realmente necesitamos — no todos los endpoints del otro servicio.
/// 
/// Refit genera la implementación automáticamente: transforma esta interfaz en llamadas HTTP reales.
/// El atributo [Get(...)] le dice qué verbo HTTP y ruta usar.
/// </summary>
public interface IPoliciesClient
{
    /// <summary>
    /// Valida que una póliza existe y obtiene su información (status, CoverageAmount).
    /// Se llama cada vez que se crea o actualiza un claim para verificar que la póliza es ACTIVE
    /// y que el Amount del claim no excede el CoverageAmount.
    /// </summary>
    [Get("/api/v1/policies/by-number/{policyNumber}")]
    Task<PolicyApiResponse> GetPolicyByNumberAsync(string policyNumber);
}
