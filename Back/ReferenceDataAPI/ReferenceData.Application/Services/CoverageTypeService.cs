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
    /// Service responsible for managing coverage type business operations.
    /// </summary>
    public class CoverageTypeService(ICoverageTypeRepository repo)
    {
        /// <summary>
        /// Retrieves paged records of coverage types.
        /// </summary>
        public async Task<PagedResponse<CoverageTypeDto>> GetAllAsync(bool includeInactive, int page, int pageSize, CancellationToken ct)
        {
            bool? isActiveFilter = includeInactive ? null : true;

            var all = await repo.GetAllAsync(isActiveFilter, ct);
            var total = all.Count;
            var items = all.Skip((page - 1) * pageSize)
                           .Take(pageSize)
                           .Select(ToDto)
                           .ToList();

            return new PagedResponse<CoverageTypeDto> { Data = items, Page = page, PageSize = pageSize, Total = total };
        }

        /// <summary>
        /// Retrieves a coverage type by its internal ID.
        /// </summary>
        public async Task<ServiceResult<CoverageTypeDto>> GetByIdAsync(int id, CancellationToken ct)
        {
            var entity = await repo.GetByIdAsync(id, ct);
            return entity is null
                ? ServiceResult<CoverageTypeDto>.Fail("Id", $"CoverageType with id {id} not found")
                : ServiceResult<CoverageTypeDto>.Ok(ToDto(entity));
        }

        /// <summary>
        /// Finds a coverage type by its business code.
        /// </summary>
        public async Task<ServiceResult<CoverageTypeDto>> GetByCodeAsync(string code, CancellationToken ct)
        {
            var entity = await repo.GetByCodeAsync(code.ToUpper(), ct);
            return entity is null
                ? ServiceResult<CoverageTypeDto>.Fail("Code", $"CoverageType '{code}' not found")
                : ServiceResult<CoverageTypeDto>.Ok(ToDto(entity));
        }

        /// <summary>
        /// Creates a new coverage type ensuring no duplicate codes exist.
        /// </summary>
        public async Task<ServiceResult<CoverageTypeDto>> CreateAsync(CreateCoverageTypeRequest request, CancellationToken ct)
        {
            var code = request.Code.Trim().ToUpper();
            if (await repo.ExistsAsync(code, ct))
                return ServiceResult<CoverageTypeDto>.Fail("Code", $"Code '{code}' already exists");

            var entity = new CoverageType { Code = code, Name = request.Name.Trim(), Description = request.Description ?? string.Empty, IsActive = true };
            var created = await repo.CreateAsync(entity, ct);
            return ServiceResult<CoverageTypeDto>.Ok(ToDto(created));
        }

        /// <summary>
        /// Updates the name, description, and status of a coverage type.
        /// </summary>
        public async Task<ServiceResult<CoverageTypeDto>> UpdateAsync(int id, UpdateCoverageTypeRequest request, CancellationToken ct)
        {
            var entity = await repo.GetByIdAsync(id, ct);
            if (entity is null)
                return ServiceResult<CoverageTypeDto>.Fail("Id", $"CoverageType with id {id} not found");

            entity.Name = request.Name.Trim();
            entity.Description = request.Description ?? string.Empty;
            entity.IsActive = request.IsActive;

            return ServiceResult<CoverageTypeDto>.Ok(ToDto(await repo.UpdateAsync(entity, ct)));
        }

        /// <summary>
        /// Deactivates a coverage type record (Soft Delete).
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
        private static CoverageTypeDto ToDto(CoverageType e) => new CoverageTypeDto
        {
            Id = e.Id,
            Code = e.Code,
            Name = e.Name,
            Description = e.Description,
            IsActive = e.IsActive
        };
    }
}