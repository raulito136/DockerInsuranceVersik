using ReferenceData.Application.Common;
using ReferenceData.Application.DTOs;
using ReferenceData.Application.Interfaces;
using ReferenceData.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceData.Application.Services
{
    /// <summary>
    /// Service for handling business logic related to claim statuses.
    /// </summary>
    public class ClaimStatusService(IClaimStatusRepository repo)
    {
        /// <summary>
        /// Retrieves a paged collection of claim statuses.
        /// </summary>
        /// <param name="includeInactive">If true, includes statuses marked as inactive.</param>
        /// <param name="page">Current page number.</param>
        /// <param name="pageSize">Number of records per page.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A paged response containing the list of <see cref="ClaimStatusDto"/>.</returns>
        public async Task<PagedResponse<ClaimStatusDto>> GetAllAsync(bool includeInactive, int page, int pageSize, CancellationToken ct)
        {
            // includeInactive=false → solo activos (isActive=true)
            // includeInactive=true  → todos (isActive=null, ignora el query filter global)
            bool? isActiveFilter = includeInactive ? null : true;

            var entities = await repo.GetAllAsync(isActiveFilter, ct);
            var dtos = entities.Select(ToDto).ToList();

            return new PagedResponse<ClaimStatusDto>
            {
                Data = dtos.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                Page = page,
                PageSize = pageSize,
                Total = dtos.Count
            };
        }


        /// <summary>
        /// Gets a specific claim status by its ID.
        /// </summary>
        public async Task<ServiceResult<ClaimStatusDto>> GetByIdAsync(int id, CancellationToken ct)
        {
            var entity = await repo.GetByIdAsync(id, ct);
            return entity is null
                ? ServiceResult<ClaimStatusDto>.Fail("Id", $"ClaimStatus with id {id} not found")
                : ServiceResult<ClaimStatusDto>.Ok(ToDto(entity));
        }

        /// <summary>
        /// Gets a specific claim status by its unique business code.
        /// </summary>
        public async Task<ServiceResult<ClaimStatusDto>> GetByCodeAsync(string code, CancellationToken ct)
        {
            var entity = await repo.GetByCodeAsync(code.ToUpper(), ct);
            return entity is null
                ? ServiceResult<ClaimStatusDto>.Fail("Code", $"ClaimStatus '{code}' not found")
                : ServiceResult<ClaimStatusDto>.Ok(ToDto(entity));
        }

        /// <summary>
        /// Creates a new claim status after validating that the code is unique.
        /// </summary>
        public async Task<ServiceResult<ClaimStatusDto>> CreateAsync(CreateClaimStatusRequest request, CancellationToken ct)
        {
            var code = request.Code.Trim().ToUpper();
            if (await repo.ExistsAsync(code, ct))
                return ServiceResult<ClaimStatusDto>.Fail("Code", $"Code '{code}' already exists");

            var entity = new ClaimStatus
            {
                Code = code,
                Name = request.Name.Trim(),
                Description = request.Description ?? string.Empty,
                IsActive = true
            };

            return ServiceResult<ClaimStatusDto>.Ok(ToDto(await repo.CreateAsync(entity, ct)));
        }

        /// <summary>
        /// Updates the properties of an existing claim status.
        /// </summary>
        public async Task<ServiceResult<ClaimStatusDto>> UpdateAsync(int id, UpdateClaimStatusRequest request, CancellationToken ct)
        {
            var entity = await repo.GetByIdAsync(id, ct);
            if (entity is null) return ServiceResult<ClaimStatusDto>.Fail("Id", "Not found");

            entity.Name = request.Name.Trim();
            entity.Description = request.Description ?? string.Empty;
            entity.IsActive = request.IsActive;

            return ServiceResult<ClaimStatusDto>.Ok(ToDto(await repo.UpdateAsync(entity, ct)));
        }

        /// <summary>
        /// Performs a soft delete by setting the status to inactive.
        /// </summary>
        public async Task<ServiceResult<bool>> DeleteAsync(int id, CancellationToken ct)
        {
            var entity = await repo.GetByIdAsync(id, ct);
            if (entity is null) return ServiceResult<bool>.Fail("Id", "Not found");

            entity.IsActive = false;
            await repo.UpdateAsync(entity, ct);
            return ServiceResult<bool>.Ok(true);
        }

        /// <summary>
        /// Activates the entity with the specified identifier asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to activate.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a ServiceResult indicating
        /// whether the activation was successful. Returns a failed result if the entity is not found.</returns>
        public async Task<ServiceResult<bool>> ActivateAsync(int id, CancellationToken ct)
        {
            var entity = await repo.GetByIdAsync(id, ct);
            if (entity is null) return ServiceResult<bool>.Fail("Id", "Not found");
            entity.IsActive = true;
            await repo.UpdateAsync(entity, ct);
            return ServiceResult<bool>.Ok(true);
        }

        /// <summary>
        /// Maps a domain entity to its corresponding Data Transfer Object.
        /// </summary>
        private static ClaimStatusDto ToDto(ClaimStatus e) => new ClaimStatusDto
        {
            Id = e.Id,
            Code = e.Code,
            Name = e.Name,
            Description = e.Description,
            IsActive = e.IsActive
        };
    }
}