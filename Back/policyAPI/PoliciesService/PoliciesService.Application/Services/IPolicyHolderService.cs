using PoliciesService.Application.Common;
using PoliciesService.Application.DTOs.PolicyHolder;
namespace PoliciesService.Application.Services
{
    public interface IPolicyHolderService
    {
        Task<Result<PolicyHolderResponseDTO>> GetByIdAsync(int id);
        Task<Result<PolicyHolderDetailResponseDTO>> GetDetailByIdAsync(int id);
        Task<Result<PagedResult<PolicyHolderResponseDTO>>> GetAllAsync(int page, int pageSize);
        Task<Result<PolicyHolderResponseDTO>> CreatePolicyHolderAsync(PolicyHolderRequestDTO dto);
        Task<Result<PolicyHolderResponseDTO>> UpdatePolicyHolderAsync(int id, PolicyHolderRequestDTO dto);
        Task<Result<bool>> DeletePolicyHolderAsync(int id);
    }
}
