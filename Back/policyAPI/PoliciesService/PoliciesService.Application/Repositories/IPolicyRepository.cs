using PoliciesService.Domain;

namespace PoliciesService.Application.Repositories
{
    public interface IPolicyRepository
    {
        Task<Policy?> GetByIdAsync(int id);
        Task<Policy?> GetByNumberAsync(string policyNumber);
        Task<(IEnumerable<Policy> Items, int TotalCount)> GetAllAsync(string? status, string? policyTypeCode, int page, int pageSize);
        Task<Policy> AddAsync(Policy policy);
        Task<Policy> UpdateAsync(Policy policy);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasAssociatedClaimsAsync(int policyId);
    }
}
