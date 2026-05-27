using PoliciesService.Application.DTOs.Policy;

namespace PoliciesService.Application.DTOs.PolicyHolder
{
    public class PolicyHolderDetailResponseDTO : PolicyHolderResponseDTO
    {
        public List<PolicyResponseDTO> Policies { get; set; } = [];
    }
}
