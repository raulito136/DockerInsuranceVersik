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
    /// Entity Framework implementation of the <see cref="IClaimStatusRepository"/>.
    /// </summary>
    /// <param name="db">The database context.</param>
    public class ClaimStatusRepository(ReferenceDataDbContext db) : IClaimStatusRepository
    {
        /// <summary>
        /// Retrieves claim statuses from the database with an optional activity filter.
        /// </summary>
        public async Task<List<ClaimStatus>> GetAllAsync(bool? isActive, CancellationToken ct)
        {
            var query = isActive.HasValue
                ? db.ClaimStatuses.Where(x => x.IsActive == isActive.Value)
                : db.ClaimStatuses.IgnoreQueryFilters();

            return await query
                .OrderBy(x => x.Code)
                .ToListAsync(ct);
        }

        /// <summary>
        /// Finds a specific claim status by its primary key.
        /// </summary>
        public async Task<ClaimStatus?> GetByIdAsync(int id, CancellationToken ct) =>
            await db.ClaimStatuses.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id, ct);

        /// <summary>
        /// Retrieves a claim status by its unique business code.
        /// </summary>
        public async Task<ClaimStatus?> GetByCodeAsync(string code, CancellationToken ct) =>
            await db.ClaimStatuses.FirstOrDefaultAsync(x => x.Code == code, ct);

        /// <summary>
        /// Checks if any claim status exists with the given code.
        /// </summary>
        public async Task<bool> ExistsAsync(string code, CancellationToken ct) =>
            await db.ClaimStatuses.AnyAsync(x => x.Code == code, ct);

        /// <summary>
        /// Adds a new claim status to the context and saves changes.
        /// </summary>
        public async Task<ClaimStatus> CreateAsync(ClaimStatus entity, CancellationToken ct)
        {
            db.ClaimStatuses.Add(entity);
            await db.SaveChangesAsync(ct);
            return entity;
        }

        /// <summary>
        /// Marks the entity as modified and persists changes.
        /// </summary>
        public async Task<ClaimStatus> UpdateAsync(ClaimStatus entity, CancellationToken ct)
        {
            db.ClaimStatuses.Update(entity);
            await db.SaveChangesAsync(ct);
            return entity;
        }

        /// <summary>
        /// Removes the entity from the database.
        /// </summary>
        public async Task DeleteAsync(ClaimStatus entity, CancellationToken ct)
        {
            db.ClaimStatuses.Remove(entity);
            await db.SaveChangesAsync(ct);
        }
    }
}