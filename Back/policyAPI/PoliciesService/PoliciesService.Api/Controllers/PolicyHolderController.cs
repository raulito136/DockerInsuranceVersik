using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PoliciesService.Application.Common;
using PoliciesService.Application.DTOs.PolicyHolder;
using PoliciesService.Application.Services;

namespace PoliciesService.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/policy-holders")]
    public class PolicyHolderController : ControllerBase
    {
        #region Attributes
        private readonly IPolicyHolderService _service;
        #endregion

        #region Constructors
        public PolicyHolderController(IPolicyHolderService service) { _service = service; }
        #endregion

        #region GET
        // List all, with pagination (GET /api/v1/policy-holders)
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _service.GetAllAsync(page, pageSize);
            return Ok(PaginatedResponse<PolicyHolderResponseDTO>.Create(result.Data!.Items, page, pageSize, result.Data.TotalCount));
        }

        // Get by ID, includes policies (GET /api/v1/policy-holders/{id})
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetDetailByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(ApiResponse<object>.Error(result.ErrorMessage!));

            return Ok(ApiResponse<PolicyHolderDetailResponseDTO>.Success(result.Data!));
        }
        #endregion

        #region POST
        // Create a new policy holder (POST /api/v1/policy-holders)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PolicyHolderRequestDTO dto)
        {
            var result = await _service.CreatePolicyHolderAsync(dto);
            if (!result.IsSuccess)
                return UnprocessableEntity(ApiResponse<object>.Error(result.ErrorMessage!));

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, ApiResponse<PolicyHolderResponseDTO>.Success(result.Data));
        }
        #endregion

        #region PUT
        // Update a policy holder (PUT /api/v1/policy-holders/{id})
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PolicyHolderRequestDTO dto)
        {
            var result = await _service.UpdatePolicyHolderAsync(id, dto);
            if (!result.IsSuccess)
                return NotFound(ApiResponse<object>.Error(result.ErrorMessage!));

            return Ok(ApiResponse<PolicyHolderResponseDTO>.Success(result.Data!));
        }
        #endregion

        #region DELETE
        // Delete, only if no active policies (DELETE /api/v1/policy-holder/{id})
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeletePolicyHolderAsync(id);
            if (!result.IsSuccess)
                return BadRequest(ApiResponse<object>.Error(result.ErrorMessage!));

            return NoContent();
        }
        #endregion
    }
}
