using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReferenceData.Domain;

namespace ReferenceData.Application.Interfaces
{
    /// <summary>
    /// Repository interface for managing coverage type persistence.
    /// </summary>
    public interface ICoverageTypeRepository
    {
        /// <summary>
        /// Fetches all coverage types, with an optional filter for active/inactive status.
        /// </summary>
        Task<List<CoverageType>> GetAllAsync(bool? isActive, CancellationToken ct);

        /// <summary>
        /// Retrieves a coverage type using its unique ID.
        /// </summary>
        Task<CoverageType?> GetByIdAsync(int id, CancellationToken ct);

        /// <summary>
        /// Retrieves a coverage type using its unique string code.
        /// </summary>
        Task<CoverageType?> GetByCodeAsync(string code, CancellationToken ct);

        /// <summary>
        /// Adds a new coverage type to the database.
        /// </summary>
        Task<CoverageType> CreateAsync(CoverageType entity, CancellationToken ct);

        /// <summary>
        /// Synchronizes changes of a coverage type entity with the database.
        /// </summary>
        Task<CoverageType> UpdateAsync(CoverageType entity, CancellationToken ct);

        /// <summary>
        /// Deletes the specified coverage type record.
        /// </summary>
        Task DeleteAsync(CoverageType entity, CancellationToken ct);

        /// <summary>
        /// Verifies existence of a coverage type by its code.
        /// </summary>
        Task<bool> ExistsAsync(string code, CancellationToken ct);
    }
}