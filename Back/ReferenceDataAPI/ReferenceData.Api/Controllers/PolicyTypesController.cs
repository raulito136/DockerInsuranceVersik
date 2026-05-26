using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReferenceData.Application.DTOs;
using ReferenceData.Application.Services;
using ReferenceData.Application.Common;


namespace ReferenceData.Api.Controllers
{
    /// <summary>
    /// Handles requests related to insurance policy types.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/policy-types")]
    public class PolicyTypesController(PolicyTypeService service) : ControllerBase
    {
        /// <summary>
        /// Fetches a list of all available policy types.
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
        /// Finds a policy type using its internal ID.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var result = await service.GetByIdAsync(id, ct);
            if (!result.IsSuccess)
                return NotFound(ApiResponse<PolicyTypeDto>.Fail(result.ErrorField!, result.ErrorMessage!));
            return Ok(ApiResponse<PolicyTypeDto>.Success(result.Value!));
        }

        /// <summary>
        /// Finds a policy type using its unique business code.
        /// </summary>
        [HttpGet("by-code/{code}")]
        public async Task<IActionResult> GetByCode(string code, CancellationToken ct)
        {
            var result = await service.GetByCodeAsync(code, ct);
            if (!result.IsSuccess)
                return NotFound(ApiResponse<PolicyTypeDto>.Fail(result.ErrorField!, result.ErrorMessage!));
            return Ok(ApiResponse<PolicyTypeDto>.Success(result.Value!));
        }

        /// <summary>
        /// Adds a new policy type to the database.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePolicyTypeRequest request, CancellationToken ct)
        {
            var result = await service.CreateAsync(request, ct);
            if (!result.IsSuccess)
                return Conflict(ApiResponse<PolicyTypeDto>.Fail(result.ErrorField!, result.ErrorMessage!));
            return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id },
                ApiResponse<PolicyTypeDto>.Success(result.Value));
        }

        /// <summary>
        /// Modifies an existing policy type.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePolicyTypeRequest request, CancellationToken ct)
        {
            var result = await service.UpdateAsync(id, request, ct);
            if (!result.IsSuccess)
                return NotFound(ApiResponse<PolicyTypeDto>.Fail(result.ErrorField!, result.ErrorMessage!));
            return Ok(ApiResponse<PolicyTypeDto>.Success(result.Value!));
        }

        /// <summary>
        /// Deletes a policy type based on the provided ID.
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