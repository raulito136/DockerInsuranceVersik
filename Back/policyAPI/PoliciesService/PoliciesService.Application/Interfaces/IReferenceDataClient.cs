using PoliciesService.Application.DTOs.External;
using Refit;

namespace PoliciesService.Application.Interfaces
{
    public interface IReferenceDataClient
    {
        // Validate PolicyTypeCode
        [Get("/api/v1/policy-types/by-code/{code}")]
        Task<IApiResponse<ReferenceDataResponse<PolicyTypeDTO>>> GetPolicyTypeAsync(string code);

        // Validate CoverageTypeCode
        [Get("/api/v1/coverage-types/by-code/{code}")]
        Task<IApiResponse<ReferenceDataResponse<CoverageTypeDTO>>> GetCoverageTypeAsync(string code);

        // Validate RegionCode
        // POSSIBLE ERROR: if the endpoint is literally "/api/v1/regions", it will return ALL the regions at once
        [Get("/api/v1/regions")]
        Task<IApiResponse<ReferenceDataPagedResponse<RegionDTO>>> GetAllRegionsAsync();
    }
}
