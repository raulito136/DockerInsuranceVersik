using ReferenceData.Application.Common;
using ReferenceData.Application.DTOs;
using ReferenceData.Application.Interfaces;
using ReferenceData.Domain;
using System.Xml.Linq;

namespace ReferenceData.Application.Services;

/// <summary>
/// Service to manage insurance policy types and associated business rules.
/// </summary>
public class PolicyTypeService(IPolicyTypeRepository repo)
{
    /// <summary>
    /// Fetches a paged list of policy types.
    /// </summary>
    public async Task<PagedResponse<PolicyTypeDto>> GetAllAsync(bool includeInactive, int page, int pageSize, CancellationToken ct)
    {
        bool? isActiveFilter = includeInactive ? null : true;

        var all = await repo.GetAllAsync(isActiveFilter, ct);
        var total = all.Count;

        var items = all.Skip((page - 1) * pageSize)
                       .Take(pageSize)
                       .Select(ToDto)
                       .ToList();

        return new PagedResponse<PolicyTypeDto>
        {
            Data = items,
            Page = page,
            PageSize = pageSize,
            Total = total
        };
    }

    /// <summary>
    /// Gets a policy type by its ID.
    /// </summary>
    public async Task<ServiceResult<PolicyTypeDto>> GetByIdAsync(int id, CancellationToken ct)
    {
        var entity = await repo.GetByIdAsync(id, ct);
        return entity is null
            ? ServiceResult<PolicyTypeDto>.Fail("Id", $"PolicyType with id {id} not found")
            : ServiceResult<PolicyTypeDto>.Ok(ToDto(entity));
    }

    /// <summary>
    /// Gets a policy type by its unique business code.
    /// </summary>
    public async Task<ServiceResult<PolicyTypeDto>> GetByCodeAsync(string code, CancellationToken ct)
    {
        var entity = await repo.GetByCodeAsync(code.ToUpper(), ct);
        return entity is null
            ? ServiceResult<PolicyTypeDto>.Fail("Code", $"PolicyType '{code}' not found")
            : ServiceResult<PolicyTypeDto>.Ok(ToDto(entity));
    }

    /// <summary>
    /// Creates a new policy type with basic validation for the code field.
    /// </summary>
    public async Task<ServiceResult<PolicyTypeDto>> CreateAsync(CreatePolicyTypeRequest request, CancellationToken ct)
    {
        var code = request.Code.Trim().ToUpper();

        if (string.IsNullOrWhiteSpace(code))
            return ServiceResult<PolicyTypeDto>.Fail("Code", "Code is required");

        if (await repo.ExistsAsync(code, ct))
            return ServiceResult<PolicyTypeDto>.Fail("Code", $"Code '{code}' already exists");

        var entity = new PolicyType
        {
            Code = code,
            Name = request.Name.Trim(),
            Description = request.Description ?? string.Empty,
            IsActive = true
        };

        var created = await repo.CreateAsync(entity, ct);
        return ServiceResult<PolicyTypeDto>.Ok(ToDto(created));
    }

    /// <summary>
    /// Updates the details of a policy type.
    /// </summary>
    public async Task<ServiceResult<PolicyTypeDto>> UpdateAsync(int id, UpdatePolicyTypeRequest request, CancellationToken ct)
    {
        var entity = await repo.GetByIdAsync(id, ct);
        if (entity is null)
            return ServiceResult<PolicyTypeDto>.Fail("Id", $"PolicyType with id {id} not found");

        entity.Name = request.Name.Trim();
        entity.IsActive = request.IsActive;
        entity.Description = request.Description ?? string.Empty;

        var updated = await repo.UpdateAsync(entity, ct);
        return ServiceResult<PolicyTypeDto>.Ok(ToDto(updated));
    }

    /// <summary>
    /// Soft deletes a policy type.
    /// </summary>
    public async Task<ServiceResult<bool>> DeleteAsync(int id, CancellationToken ct)
    {
        var entity = await repo.GetByIdAsync(id, ct);
        if (entity is null)
            return ServiceResult<bool>.Fail("Id", $"PolicyType with id {id} not found");

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
    private static PolicyTypeDto ToDto(PolicyType e) => new PolicyTypeDto
    {
        Id = e.Id,
        Code = e.Code,
        Name = e.Name,
        Description = e.Description,
        IsActive = e.IsActive
    };
}