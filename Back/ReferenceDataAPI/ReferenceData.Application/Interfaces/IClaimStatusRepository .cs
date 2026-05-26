using ReferenceData.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceData.Application.Interfaces
{
    /// <summary>
    /// Defines the data access contract for Claim Status entities.
    /// </summary>
    public interface IClaimStatusRepository
    {
        /// <summary>
        /// Retrieves all claim statuses from the database, optionally filtered by status.
        /// </summary>
        /// <param name="isActive">If true, returns only active records; if false, only inactive; if null, returns all.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A list of claim status entities.</returns>
        Task<List<ClaimStatus>> GetAllAsync(bool? isActive, CancellationToken ct);

        /// <summary>
        /// Finds a claim status by its primary identifier.
        /// </summary>
        /// <param name="id">The unique ID of the entity.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The found entity or null if not found.</returns>
        Task<ClaimStatus?> GetByIdAsync(int id, CancellationToken ct);

        /// <summary>
        /// Finds a claim status by its unique business code.
        /// </summary>
        /// <param name="code">The alphanumeric code of the entity.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The found entity or null if not found.</returns>
        Task<ClaimStatus?> GetByCodeAsync(string code, CancellationToken ct);

        /// <summary>
        /// Persists a new claim status entity in the data store.
        /// </summary>
        /// <param name="entity">The entity to create.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The created entity with generated identifiers.</returns>
        Task<ClaimStatus> CreateAsync(ClaimStatus entity, CancellationToken ct);

        /// <summary>
        /// Updates an existing claim status entity.
        /// </summary>
        /// <param name="entity">The entity containing updated values.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The updated entity.</returns>
        Task<ClaimStatus> UpdateAsync(ClaimStatus entity, CancellationToken ct);

        /// <summary>
        /// Removes a claim status entity from the data store.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <param name="ct">The cancellation token.</param>
        Task DeleteAsync(ClaimStatus entity, CancellationToken ct);

        /// <summary>
        /// Checks if a claim status with the specified code already exists.
        /// </summary>
        /// <param name="code">The code to check.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>True if the code exists, otherwise false.</returns>
        Task<bool> ExistsAsync(string code, CancellationToken ct);
    }
}
