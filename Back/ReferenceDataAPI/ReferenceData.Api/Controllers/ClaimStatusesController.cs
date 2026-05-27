using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReferenceData.Application.DTOs;
using ReferenceData.Application.Services;
using ReferenceData.Application.Common;

namespace ReferenceData.Api.Controllers
{
    /// <summary>
    /// Provides endpoints for managing claim status reference data.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/claim-statuses")]
    public class ClaimStatusesController(ClaimStatusService service) : ControllerBase
    {
        /// <summary>
        /// Retrieves a paginated list of all claim statuses.
        /// </summary>
        /// <param name="includeInactive">Whether to include records marked as inactive.</param>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A paginated collection of claim statuses.</returns>
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
        /// Gets a specific claim status by its unique identifier.
        /// </summary>
        /// <param name="id">The unique ID of the claim status.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The requested claim status details.</returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var result = await service.GetByIdAsync(id, ct);
            if (!result.IsSuccess)
                return NotFound(ApiResponse<ClaimStatusDto>.Fail(result.ErrorField!, result.ErrorMessage!));
            return Ok(ApiResponse<ClaimStatusDto>.Success(result.Value!));
        }

        /// <summary>
        /// Gets a specific claim status by its business code.
        /// </summary>
        /// <param name="code">The unique string code of the claim status.</param>
        /// <param name="ct">Cancellation token.</param>
        [HttpGet("by-code/{code}")]
        public async Task<IActionResult> GetByCode(string code, CancellationToken ct)
        {
            var result = await service.GetByCodeAsync(code, ct);
            if (!result.IsSuccess)
                return NotFound(ApiResponse<ClaimStatusDto>.Fail(result.ErrorField!, result.ErrorMessage!));
            return Ok(ApiResponse<ClaimStatusDto>.Success(result.Value!));
        }

        /// <summary>
        /// Creates a new claim status record.
        /// </summary>
        /// <param name="request">The data to create the claim status.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The newly created claim status.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateClaimStatusRequest request, CancellationToken ct)
        {
            var result = await service.CreateAsync(request, ct);
            if (!result.IsSuccess)
                return Conflict(ApiResponse<ClaimStatusDto>.Fail(result.ErrorField!, result.ErrorMessage!));
            return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id },
                ApiResponse<ClaimStatusDto>.Success(result.Value));
        }

        /// <summary>
        /// Updates an existing claim status record.
        /// </summary>
        /// <param name="id">The unique ID of the record to update.</param>
        /// <param name="request">The updated data.</param>
        /// <param name="ct">Cancellation token.</param>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateClaimStatusRequest request, CancellationToken ct)
        {
            var result = await service.UpdateAsync(id, request, ct);
            if (!result.IsSuccess)
                return NotFound(ApiResponse<ClaimStatusDto>.Fail(result.ErrorField!, result.ErrorMessage!));
            return Ok(ApiResponse<ClaimStatusDto>.Success(result.Value!));
        }

        /// <summary>
        /// Deletes a claim status record from the system.
        /// </summary>
        /// <param name="id">The unique ID of the record to delete.</param>
        /// <param name="ct">Cancellation token.</param>
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

