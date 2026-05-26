using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReferenceData.Domain;

namespace ReferenceData.Application.Interfaces
{
    /// <summary>
    /// Interface for data operations related to policy types.
    /// </summary>
    public interface IPolicyTypeRepository
    {
        /// <summary>
        /// Gets a collection of policy types.
        /// </summary>
        Task<List<PolicyType>> GetAllAsync(bool? isActive, CancellationToken ct);

        /// <summary>
        /// Gets a specific policy type by ID.
        /// </summary>
        Task<PolicyType?> GetByIdAsync(int id, CancellationToken ct);

        /// <summary>
        /// Gets a specific policy type by its code.
        /// </summary>
        Task<PolicyType?> GetByCodeAsync(string code, CancellationToken ct);

        /// <summary>
        /// Inserts a new policy type record.
        /// </summary>
        Task<PolicyType> CreateAsync(PolicyType entity, CancellationToken ct);

        /// <summary>
        /// Updates an existing policy type record.
        /// </summary>
        Task<PolicyType> UpdateAsync(PolicyType entity, CancellationToken ct);

        /// <summary>
        /// Hard or soft deletes a policy type record.
        /// </summary>
        Task DeleteAsync(PolicyType entity, CancellationToken ct);

        /// <summary>
        /// Determines if a policy type code is already in use.
        /// </summary>
        Task<bool> ExistsAsync(string code, CancellationToken ct);
    }
}
