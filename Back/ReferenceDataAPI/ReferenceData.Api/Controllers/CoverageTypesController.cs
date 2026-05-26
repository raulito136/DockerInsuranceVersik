using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReferenceData.Application.DTOs;
using ReferenceData.Application.Services;
using ReferenceData.Application.Common;

namespace ReferenceData.Api.Controllers
{
    /// <summary>
    /// Controller for managing various types of insurance coverage.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/coverage-types")]
    public class CoverageTypesController(CoverageTypeService service) : ControllerBase
    {
        /// <summary>
        /// Retrieves all coverage types with optional pagination and filtering by status.
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
        /// Retrieves a coverage type by its ID.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var result = await service.GetByIdAsync(id, ct);
            if (!result.IsSuccess)
                return NotFound(ApiResponse<CoverageTypeDto>.Fail(result.ErrorField!, result.ErrorMessage!));
            return Ok(ApiResponse<CoverageTypeDto>.Success(result.Value!));
        }

        /// <summary>
        /// Retrieves a coverage type by its alphanumeric code.
        /// </summary>
        [HttpGet("by-code/{code}")]
        public async Task<IActionResult> GetByCode(string code, CancellationToken ct)
        {
            var result = await service.GetByCodeAsync(code, ct);
            if (!result.IsSuccess)
                return NotFound(ApiResponse<CoverageTypeDto>.Fail(result.ErrorField!, result.ErrorMessage!));
            return Ok(ApiResponse<CoverageTypeDto>.Success(result.Value!));
        }

        /// <summary>
        /// Registers a new coverage type in the system.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCoverageTypeRequest request, CancellationToken ct)
        {
            var result = await service.CreateAsync(request, ct);
            if (!result.IsSuccess)
                return Conflict(ApiResponse<CoverageTypeDto>.Fail(result.ErrorField!, result.ErrorMessage!));
            return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id },
                ApiResponse<CoverageTypeDto>.Success(result.Value));
        }

        /// <summary>
        /// Updates the details of an existing coverage type.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCoverageTypeRequest request, CancellationToken ct)
        {
            var result = await service.UpdateAsync(id, request, ct);
            if (!result.IsSuccess)
                return NotFound(ApiResponse<CoverageTypeDto>.Fail(result.ErrorField!, result.ErrorMessage!));
            return Ok(ApiResponse<CoverageTypeDto>.Success(result.Value!));
        }

        /// <summary>
        /// Removes a coverage type record.
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
