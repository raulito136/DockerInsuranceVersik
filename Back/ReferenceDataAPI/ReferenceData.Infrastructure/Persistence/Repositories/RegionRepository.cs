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
    /// Data access implementation for Regions using Entity Framework Core.
    /// </summary>
    public class RegionRepository(ReferenceDataDbContext db) : IRegionRepository
    {
        /// <summary>
        /// Retrieves the list of regions, ordered by code.
        /// </summary>
        public async Task<List<Region>> GetAllAsync(bool? isActive, CancellationToken ct)
        {
            var query = isActive.HasValue
                ? db.Regions.Where(x => x.IsActive == isActive.Value)
                : db.Regions.IgnoreQueryFilters();

            return await query
                .OrderBy(x => x.Code)
                .ToListAsync(ct);
        }

        /// <summary>
        /// Finds a region by ID.
        /// </summary>
        public async Task<Region?> GetByIdAsync(int id, CancellationToken ct) =>
            await db.Regions.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id, ct);

        /// <summary>
        /// Finds a region by its unique code.
        /// </summary>
        public async Task<Region?> GetByCodeAsync(string code, CancellationToken ct) =>
            await db.Regions.FirstOrDefaultAsync(x => x.Code == code, ct);

        /// <summary>
        /// Checks if a region code exists in the database.
        /// </summary>
        public async Task<bool> ExistsAsync(string code, CancellationToken ct) =>
            await db.Regions.AnyAsync(x => x.Code == code, ct);

        /// <summary>
        /// Creates a new region record.
        /// </summary>
        public async Task<Region> CreateAsync(Region entity, CancellationToken ct)
        {
            db.Regions.Add(entity);
            await db.SaveChangesAsync(ct);
            return entity;
        }

        /// <summary>
        /// Updates a region's data.
        /// </summary>
        public async Task<Region> UpdateAsync(Region entity, CancellationToken ct)
        {
            db.Regions.Update(entity);
            await db.SaveChangesAsync(ct);
            return entity;
        }

        /// <summary>
        /// Deletes a region record.
        /// </summary>
        public async Task DeleteAsync(Region entity, CancellationToken ct)
        {
            db.Regions.Remove(entity);
            await db.SaveChangesAsync(ct);
        }
    }
}