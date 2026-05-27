using PoliciesService.Application.Common;
using PoliciesService.Application.DTOs.Policy;

namespace PoliciesService.Application.Services
{
    public interface IPolicyService
    {
        Task<Result<PolicyResponseDTO>> GetByIdAsync(int id);
        Task<Result<PolicyResponseDTO>> GetByNumberAsync(string policyNumber);
        Task<Result<PagedResult<PolicyResponseDTO>>> GetAllAsync(string? status, string? policyTypeCode, int page, int pageSize);
        Task<Result<PolicyResponseDTO>> CreatePolicyAsync(PolicyRequestDTO dto);
        Task<Result<PolicyResponseDTO>> UpdatePolicyAsync(int id, PolicyRequestDTO dto);
        Task<Result<bool>> DeletePolicyAsync(int id);
    }
}
