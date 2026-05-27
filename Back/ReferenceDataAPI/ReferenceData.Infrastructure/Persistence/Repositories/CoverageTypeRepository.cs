using Microsoft.EntityFrameworkCore;
using ReferenceData.Application.Interfaces;
using ReferenceData.Domain;

namespace ReferenceData.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Repository implementation for Coverage Types using Entity Framework Core.
    /// </summary>
    public class CoverageTypeRepository(ReferenceDataDbContext db) : ICoverageTypeRepository
    {
        /// <summary>
        /// Fetches coverage types from the DB, ordered by code.
        /// </summary>
        public async Task<List<CoverageType>> GetAllAsync(bool? isActive, CancellationToken ct)
        {
            var query = isActive.HasValue
                ? db.CoverageTypes.Where(x => x.IsActive == isActive.Value)
                : db.CoverageTypes.IgnoreQueryFilters();

            return await query
                .OrderBy(x => x.Code)
                .ToListAsync(ct);
        }

        /// <summary>
        /// Finds a coverage type by ID.
        /// </summary>
        public async Task<CoverageType?> GetByIdAsync(int id, CancellationToken ct) =>
            await db.CoverageTypes.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id, ct);

        /// <summary>
        /// Finds a coverage type by its unique code.
        /// </summary>
        public async Task<CoverageType?> GetByCodeAsync(string code, CancellationToken ct) =>
            await db.CoverageTypes.FirstOrDefaultAsync(x => x.Code == code, ct);

        /// <summary>
        /// Verifies if a code is already registered in the CoverageTypes table.
        /// </summary>
        public async Task<bool> ExistsAsync(string code, CancellationToken ct) =>
            await db.CoverageTypes.AnyAsync(x => x.Code == code, ct);

        /// <summary>
        /// Inserts a new coverage type into the database.
        /// </summary>
        public async Task<CoverageType> CreateAsync(CoverageType entity, CancellationToken ct)
        {
            db.CoverageTypes.Add(entity);
            await db.SaveChangesAsync(ct);
            return entity;
        }

        /// <summary>
        /// Updates an existing coverage type record.
        /// </summary>
        public async Task<CoverageType> UpdateAsync(CoverageType entity, CancellationToken ct)
        {
            db.CoverageTypes.Update(entity);
            await db.SaveChangesAsync(ct);
            return entity;
        }

        /// <summary>
        /// Deletes a coverage type record permanently.
        /// </summary>
        public async Task DeleteAsync(CoverageType entity, CancellationToken ct)
        {
            db.CoverageTypes.Remove(entity);
            await db.SaveChangesAsync(ct);
        }
    }
}