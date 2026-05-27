using PoliciesService.Application.Repositories;
using PoliciesService.Domain;
using Microsoft.EntityFrameworkCore;

namespace PoliciesService.Infrastructure.Repositories
{
    public class PolicyHolderRepository : IPolicyHolderRepository
    {
        private readonly AppDbContext _context;

        public PolicyHolderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PolicyHolder?> GetByIdAsync(int id)
        {
            return await _context.PolicyHolders
                .Include(ph => ph.Policies)
                .FirstOrDefaultAsync(ph => ph.Id == id);
        }

        public async Task<PolicyHolder?> GetDetailByIdAsync(int id)
        {
            return await _context.PolicyHolders
                .Include(ph => ph.Policies)
                .FirstOrDefaultAsync(ph => ph.Id == id);
        }

        public async Task<(IEnumerable<PolicyHolder> Items, int TotalCount)> GetAllAsync(int page, int pageSize)
        {
            var totalCount = await _context.PolicyHolders.CountAsync();

            var items = await _context.PolicyHolders
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.PolicyHolders.AnyAsync(x => x.Email == email);
        }

        public async Task<PolicyHolder> AddAsync(PolicyHolder policyHolder)
        {
            _context.PolicyHolders.Add(policyHolder);
            await _context.SaveChangesAsync();
            return policyHolder;
        }

        public async Task<PolicyHolder> UpdateAsync(PolicyHolder policyHolder)
        {
            _context.PolicyHolders.Update(policyHolder);
            await _context.SaveChangesAsync();
            return policyHolder;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var holder = await _context.PolicyHolders.FindAsync(id);
            if (holder == null) return false;

            _context.PolicyHolders.Remove(holder);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasActivePoliciesAsync(int policyHolderId)
        {
            //Console.WriteLine($"\n\n\nPolicy holder id: {policyHolderId}\n\n\n");
            return await _context.Policies
                .AnyAsync(p => p.PolicyHolderId == policyHolderId && p.Status == "ACTIVE");
        }
    }
}
