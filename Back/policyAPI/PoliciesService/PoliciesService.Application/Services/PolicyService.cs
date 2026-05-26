using PoliciesService.Application.Common;
using PoliciesService.Application.DTOs.Policy;
using PoliciesService.Application.Interfaces;
using PoliciesService.Application.Repositories;
using PoliciesService.Domain;

namespace PoliciesService.Application.Services
{
    public class PolicyService : IPolicyService
    {
        private readonly IPolicyRepository _repository;
        private readonly IReferenceDataClient _referenceDataClient;

        public PolicyService(IPolicyRepository repository, IReferenceDataClient referenceDataClient)
        {
            _repository = repository;
            _referenceDataClient = referenceDataClient;
        }

        public async Task<Result<PolicyResponseDTO>> GetByIdAsync(int id)
        {
            var policy = await _repository.GetByIdAsync(id);
            if (policy == null) return Result<PolicyResponseDTO>.Failure("Policy not found.");

            return Result<PolicyResponseDTO>.Success(MapToResponseDto(policy));
        }

        public async Task<Result<PolicyResponseDTO>> GetByNumberAsync(string policyNumber)
        {
            var policy = await _repository.GetByNumberAsync(policyNumber);
            if (policy == null) return Result<PolicyResponseDTO>.Failure("Policy not found.");

            return Result<PolicyResponseDTO>.Success(MapToResponseDto(policy));
        }

        public async Task<Result<PagedResult<PolicyResponseDTO>>> GetAllAsync(string? status, string? policyTypeCode, int page, int pageSize)
        {
            var (items, totalCount) = await _repository.GetAllAsync(status, policyTypeCode, page, pageSize);
            var dtos = items.Select(MapToResponseDto);
            return Result<PagedResult<PolicyResponseDTO>>.Success(new PagedResult<PolicyResponseDTO>(dtos, totalCount));
        }

        public async Task<Result<PolicyResponseDTO>> CreatePolicyAsync(PolicyRequestDTO dto)
        {
            // 1. Validation
            var validationResult = await ValidateExternalDataAsync(dto);
            if (!validationResult.IsSuccess) return Result<PolicyResponseDTO>.Failure(validationResult.ErrorMessage!);

            // 2. Date validation
            if (dto.EndDate <= dto.StartDate)
            {
                return Result<PolicyResponseDTO>.Failure("EndDate must be after StartDate.");
            }

            // 3. Generate Policy Number (POL-YYYY-XXXXX)
            // Using a simple counter or random part for now, in a real app this would be more controlled
            string randomPart = Guid.NewGuid().ToString("N")[..5].ToUpper();
            string policyNumber = $"POL-{DateTime.UtcNow.Year}-{randomPart}";

            var policy = new Policy
            {
                PolicyNumber = policyNumber,
                PolicyHolderId = dto.PolicyHolderId,
                PolicyTypeCode = dto.PolicyTypeCode,
                CoverageTypeCode = dto.CoverageTypeCode,
                CoverageAmount = dto.CoverageAmount,
                PremiumAmount = dto.PremiumAmount,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = dto.Status ?? "ACTIVE",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            var savedPolicy = await _repository.AddAsync(policy);
            return Result<PolicyResponseDTO>.Success(MapToResponseDto(savedPolicy));
        }

        public async Task<Result<PolicyResponseDTO>> UpdatePolicyAsync(int id, PolicyRequestDTO dto)
        {
            var existingPolicy = await _repository.GetByIdAsync(id);
            if (existingPolicy == null) return Result<PolicyResponseDTO>.Failure("Policy not found.");

            // 1. Validation
            var validationResult = await ValidateExternalDataAsync(dto);
            if (!validationResult.IsSuccess) return Result<PolicyResponseDTO>.Failure(validationResult.ErrorMessage!);

            // 2. Date validation
            if (dto.EndDate <= dto.StartDate)
            {
                return Result<PolicyResponseDTO>.Failure("EndDate must be after StartDate.");
            }

            // Basic updates
            existingPolicy.PolicyTypeCode = dto.PolicyTypeCode;
            existingPolicy.CoverageTypeCode = dto.CoverageTypeCode;
            existingPolicy.CoverageAmount = dto.CoverageAmount;
            existingPolicy.PremiumAmount = dto.PremiumAmount;
            existingPolicy.StartDate = dto.StartDate;
            existingPolicy.EndDate = dto.EndDate;
            existingPolicy.Status = dto.Status ?? existingPolicy.Status;
            existingPolicy.UpdatedAt = DateTime.UtcNow;

            var updatedPolicy = await _repository.UpdateAsync(existingPolicy);
            return Result<PolicyResponseDTO>.Success(MapToResponseDto(updatedPolicy));
        }

        private async Task<Result<bool>> ValidateExternalDataAsync(PolicyRequestDTO dto)
        {
            try
            {
                // Validate PolicyTypeCode exists and is active
                var ptResponse = await _referenceDataClient.GetPolicyTypeAsync(dto.PolicyTypeCode);
                if (ptResponse.Content?.Data == null || !ptResponse.Content.Data.IsActive)
                {
                    return Result<bool>.Failure($"Policy type '{dto.PolicyTypeCode}' is not active or not found.");
                }

                // Validate CoverageTypeCode exists and is active
                var ctResponse = await _referenceDataClient.GetCoverageTypeAsync(dto.CoverageTypeCode);
                if (ctResponse.Content?.Data == null || !ctResponse.Content.Data.IsActive)
                {
                    return Result<bool>.Failure($"Coverage type '{dto.CoverageTypeCode}' is not active or not found.");
                }

                return Result<bool>.Success(true);
            }
            catch (Exception)
            {
                // As per guide: Catch Refit exception and return 503 equivalent
                return Result<bool>.Failure("Reference Data service is currently unavailable (503).");
            }
        }

        public async Task<Result<bool>> DeletePolicyAsync(int id)
        {
            // Check if there are associated claims (Placeholder for now)
            bool hasClaims = await _repository.HasAssociatedClaimsAsync(id);
            if (hasClaims)
            {
                return Result<bool>.Failure("Cannot delete policy with associated claims.");
            }

            bool deleted = await _repository.DeleteAsync(id);
            return deleted ? Result<bool>.Success(true) : Result<bool>.Failure("Policy not found.");
        }

        private static PolicyResponseDTO MapToResponseDto(Policy policy)
        {
            return new PolicyResponseDTO
            {
                Id = policy.Id,
                PolicyNumber = policy.PolicyNumber,
                PolicyHolderId = policy.PolicyHolderId,
                PolicyTypeCode = policy.PolicyTypeCode,
                CoverageTypeCode = policy.CoverageTypeCode,
                CoverageAmount = policy.CoverageAmount,
                PremiumAmount = policy.PremiumAmount,
                StartDate = policy.StartDate,
                EndDate = policy.EndDate,
                Status = policy.Status,
                CreatedAt = policy.CreatedAt,
                UpdatedAt = policy.UpdatedAt
            };
        }
    }
}
