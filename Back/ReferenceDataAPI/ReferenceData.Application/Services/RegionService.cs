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
    /// Service for managing geographic regions and their availability.
    /// </summary>
    public class RegionService(IRegionRepository repo)
    {
        /// <summary>
        /// Gets all regions in a paginated format.
        /// </summary>
        public async Task<PagedResponse<RegionDto>> GetAllAsync(bool includeInactive, int page, int pageSize, CancellationToken ct)
        {
            bool? isActiveFilter = includeInactive ? null : true;

            var all = await repo.GetAllAsync(isActiveFilter, ct);
            var total = all.Count;

            var items = all.Skip((page - 1) * pageSize)
                           .Take(pageSize)
                           .Select(ToDto)
                           .ToList();

            return new PagedResponse<RegionDto>
            {
                Data = items,
                Page = page,
                PageSize = pageSize,
                Total = total
            };
        }

        /// <summary>
        /// Retrieves a region by ID.
        /// </summary>
        public async Task<ServiceResult<RegionDto>> GetByIdAsync(int id, CancellationToken ct)
        {
            var entity = await repo.GetByIdAsync(id, ct);
            return entity is null
                ? ServiceResult<RegionDto>.Fail("Id", $"Region with id {id} not found")
                : ServiceResult<RegionDto>.Ok(ToDto(entity));
        }

        /// <summary>
        /// Retrieves a region by its unique code.
        /// </summary>
        public async Task<ServiceResult<RegionDto>> GetByCodeAsync(string code, CancellationToken ct)
        {
            var entity = await repo.GetByCodeAsync(code.ToUpper(), ct);
            return entity is null
                ? ServiceResult<RegionDto>.Fail("Code", $"Region '{code}' not found")
                : ServiceResult<RegionDto>.Ok(ToDto(entity));
        }

        /// <summary>
        /// Creates a new region after checking that the code is not already registered.
        /// </summary>
        public async Task<ServiceResult<RegionDto>> CreateAsync(CreateRegionRequest request, CancellationToken ct)
        {
            var code = request.Code.Trim().ToUpper();
            if (await repo.ExistsAsync(code, ct))
                return ServiceResult<RegionDto>.Fail("Code", $"Code '{code}' already exists");

            var entity = new Region { Code = code, Name = request.Name.Trim(), IsActive = true };
            return ServiceResult<RegionDto>.Ok(ToDto(await repo.CreateAsync(entity, ct)));
        }

        /// <summary>
        /// Updates the region's information.
        /// </summary>
        public async Task<ServiceResult<RegionDto>> UpdateAsync(int id, UpdateRegionRequest request, CancellationToken ct)
        {
            var entity = await repo.GetByIdAsync(id, ct);
            if (entity is null)
                return ServiceResult<RegionDto>.Fail("Id", $"Region with id {id} not found");

            entity.Name = request.Name.Trim();
            entity.IsActive = request.IsActive;

            return ServiceResult<RegionDto>.Ok(ToDto(await repo.UpdateAsync(entity, ct)));
        }

        /// <summary>
        /// Marks a region as inactive (Soft Delete).
        /// </summary>
        public async Task<ServiceResult<bool>> DeleteAsync(int id, CancellationToken ct)
        {
            var entity = await repo.GetByIdAsync(id, ct);
            if (entity is null) return ServiceResult<bool>.Fail("Id", "No encontrado");

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
        private static RegionDto ToDto(Region e) => new RegionDto
        {
            Id = e.Id,
            Code = e.Code,
            Name = e.Name,
            IsActive = e.IsActive
        };
    }
}
