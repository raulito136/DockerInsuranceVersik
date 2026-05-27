using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PoliciesService.Application.Common;
using PoliciesService.Application.DTOs.Policy;
using PoliciesService.Application.Services;

namespace PoliciesService.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/policies")]
    public class PolicyController : ControllerBase
    {
        #region Attributes
        private readonly IPolicyService _service;
        #endregion

        #region Constructors
        public PolicyController(IPolicyService service) {_service = service;}
        #endregion

        #region GET
        // List all, paginated and filterable by policytypecode (GET /api/v1/policies)
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? status,
            [FromQuery] string? policyTypeCode,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _service.GetAllAsync(status, policyTypeCode, page, pageSize);
            return Ok(PaginatedResponse<PolicyResponseDTO>.Create(result.Data!.Items, page, pageSize, result.Data.TotalCount));
        }

        // Get by id (GET /api/v1/policies/{id})
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(ApiResponse<object>.Error(result.ErrorMessage!));

            return Ok(ApiResponse<PolicyResponseDTO>.Success(result.Data!));
        }

        // Get by policy number, used by claims (GET /api/v1/policies/by-number/{policyNumber})
        [HttpGet("by-number/{policyNumber}")]
        public async Task<IActionResult> GetByNumber(string policyNumber)
        {
            var result = await _service.GetByNumberAsync(policyNumber);
            if (!result.IsSuccess)
                return NotFound(ApiResponse<object>.Error(result.ErrorMessage!));

            return Ok(ApiResponse<PolicyResponseDTO>.Success(result.Data!));
        }
        #endregion

        #region POST
        // Create a new policy (POST /api/v1/policies)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PolicyRequestDTO dto)
        {
            var result = await _service.CreatePolicyAsync(dto);
            if (!result.IsSuccess)
                return UnprocessableEntity(ApiResponse<object>.Error(result.ErrorMessage!));

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, ApiResponse<PolicyResponseDTO>.Success(result.Data));
        }
        #endregion

        #region PUT
        // Update a policy (PUT /api/v1/policies/{id})
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PolicyRequestDTO dto)
        {
            var result = await _service.UpdatePolicyAsync(id, dto);
            if (!result.IsSuccess)
                return NotFound(ApiResponse<object>.Error(result.ErrorMessage!));

            return Ok(ApiResponse<PolicyResponseDTO>.Success(result.Data!));
        }
        #endregion

        #region DELETE
        // Delete, only if no associated claims (DELETE /api/v1/policies/{id})
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeletePolicyAsync(id);
            if (!result.IsSuccess)
                return BadRequest(ApiResponse<object>.Error(result.ErrorMessage!));

            return NoContent();
        }
        #endregion
    }
}
