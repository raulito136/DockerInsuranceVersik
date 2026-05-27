using ReferenceData.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceData.Application.Interfaces
{
    /// <summary>
    /// Contract for the Region data access layer.
    /// </summary>
    public interface IRegionRepository
    {
        /// <summary>
        /// Retrieves regions based on the specified active status.
        /// </summary>
        Task<List<Region>> GetAllAsync(bool? isActive, CancellationToken ct);

        /// <summary>
        /// Retrieves a single region by ID.
        /// </summary>
        Task<Region?> GetByIdAsync(int id, CancellationToken ct);

        /// <summary>
        /// Retrieves a single region by its code.
        /// </summary>
        Task<Region?> GetByCodeAsync(string code, CancellationToken ct);

        /// <summary>
        /// Creates a new region record.
        /// </summary>
        Task<Region> CreateAsync(Region entity, CancellationToken ct);

        /// <summary>
        /// Updates an existing region record.
        /// </summary>
        Task<Region> UpdateAsync(Region entity, CancellationToken ct);

        /// <summary>
        /// Removes the region entity from the system.
        /// </summary>
        Task DeleteAsync(Region entity, CancellationToken ct);

        /// <summary>
        /// Checks for the existence of a region code.
        /// </summary>
        Task<bool> ExistsAsync(string code, CancellationToken ct);
    }
}