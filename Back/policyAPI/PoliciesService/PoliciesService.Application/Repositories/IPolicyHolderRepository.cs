using PoliciesService.Domain;

namespace PoliciesService.Application.Repositories
{
    public interface IPolicyHolderRepository
    {
        Task<PolicyHolder?> GetByIdAsync(int id);
        Task<PolicyHolder?> GetDetailByIdAsync(int id);
        Task<(IEnumerable<PolicyHolder> Items, int TotalCount)> GetAllAsync(int page, int pageSize);
        Task<bool> EmailExistsAsync(string email);
        Task<PolicyHolder> AddAsync(PolicyHolder policyHolder);
        Task<PolicyHolder> UpdateAsync(PolicyHolder policyHolder);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasActivePoliciesAsync(int policyHolderId);
    }
}
