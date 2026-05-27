using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReferenceData.Application.DTOs;
using ReferenceData.Application.Services;
using ReferenceData.Application.Common;

namespace ReferenceData.Api.Controllers
{
    /// <summary>
    /// API Controller for managing geographic regions.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/regions")]
    public class RegionsController(RegionService service) : ControllerBase
    {
        /// <summary>
        /// Lists all regions with pagination support.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] bool includeInactive = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            var result = await service.GetAllAsync(includeInactive, page, pageSize, ct);
            return Ok(result);
        }

        /// <summary>
        /// Gets a region by its unique ID.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var result = await service.GetByIdAsync(id, ct);
            if (!result.IsSuccess)
                return NotFound(ApiResponse<RegionDto>.Fail(result.ErrorField!, result.ErrorMessage!));
            return Ok(ApiResponse<RegionDto>.Success(result.Value!));
        }

        /// <summary>
        /// Creates a new region entry.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRegionRequest request, CancellationToken ct)
        {
            var result = await service.CreateAsync(request, ct);
            if (!result.IsSuccess)
                return Conflict(ApiResponse<RegionDto>.Fail(result.ErrorField!, result.ErrorMessage!));
            return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id },
                ApiResponse<RegionDto>.Success(result.Value));
        }

        /// <summary>
        /// Updates the information of an existing region.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRegionRequest request, CancellationToken ct)
        {
            var result = await service.UpdateAsync(id, request, ct);
            if (!result.IsSuccess)
                return NotFound(ApiResponse<RegionDto>.Fail(result.ErrorField!, result.ErrorMessage!));
            return Ok(ApiResponse<RegionDto>.Success(result.Value!));
        }

        /// <summary>
        /// Permanently deletes a region record.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await service.DeleteAsync(id, ct);
            if (!result.IsSuccess)
                return NotFound(ApiResponse<bool>.Fail(result.ErrorField!, result.ErrorMessage!));
            return NoContent();
        }

        /// <summary>
        /// Activates the specified entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to activate.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A result indicating the outcome of the activation request. Returns a 204 No Content response if the
        /// activation is successful; otherwise, returns a 404 Not Found response if the entity does not exist.</returns>
        [HttpPut("{id:int}/activate")]
        public async Task<IActionResult> Activate(int id, CancellationToken ct)
        {
            var result = await service.ActivateAsync(id, ct);
            if (!result.IsSuccess)
                return NotFound(ApiResponse<bool>.Fail(result.ErrorField!, result.ErrorMessage!));
            return NoContent();
        }
    }
}