using Microsoft.EntityFrameworkCore;
using ReferenceData.Application.Interfaces;
using ReferenceData.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceData.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Repository implementation for Policy Types.
    /// </summary>
    public class PolicyTypeRepository(ReferenceDataDbContext db) : IPolicyTypeRepository
    {
        /// <summary>
        /// Gets all policy types from the database.
        /// </summary>
        public async Task<List<PolicyType>> GetAllAsync(bool? isActive, CancellationToken ct)
        {
            var query = isActive.HasValue
                ? db.PolicyTypes.Where(x => x.IsActive == isActive.Value)
                : db.PolicyTypes.IgnoreQueryFilters();

            return await query
                .OrderBy(x => x.Code)
                .ToListAsync(ct);
        }

        /// <summary>
        /// Locates a policy type by its internal ID.
        /// </summary>
        public async Task<PolicyType?> GetByIdAsync(int id, CancellationToken ct) =>
            await db.PolicyTypes.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id, ct);

        /// <summary>
        /// Locates a policy type by its business code.
        /// </summary>
        public async Task<PolicyType?> GetByCodeAsync(string code, CancellationToken ct) =>
            await db.PolicyTypes.FirstOrDefaultAsync(x => x.Code == code, ct);

        /// <summary>
        /// Checks for code duplicates in the PolicyTypes table.
        /// </summary>
        public async Task<bool> ExistsAsync(string code, CancellationToken ct) =>
            await db.PolicyTypes.AnyAsync(x => x.Code == code, ct);

        /// <summary>
        /// Saves a new policy type record.
        /// </summary>
        public async Task<PolicyType> CreateAsync(PolicyType entity, CancellationToken ct)
        {
            db.PolicyTypes.Add(entity);
            await db.SaveChangesAsync(ct);
            return entity;
        }

        /// <summary>
        /// Updates an existing policy type record.
        /// </summary>
        public async Task<PolicyType> UpdateAsync(PolicyType entity, CancellationToken ct)
        {
            db.PolicyTypes.Update(entity);
            await db.SaveChangesAsync(ct);
            return entity;
        }

        /// <summary>
        /// Removes the record from the data store.
        /// </summary>
        public async Task DeleteAsync(PolicyType entity, CancellationToken ct)
        {
            db.PolicyTypes.Remove(entity);
            await db.SaveChangesAsync(ct);
        }
    }
}