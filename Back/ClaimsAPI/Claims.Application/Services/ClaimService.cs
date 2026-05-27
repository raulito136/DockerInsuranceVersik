using Claims.Application.Common;
using Claims.Application.DTOs;
using Claims.Application.Interfaces;
using Claims.Domain;

namespace Claims.Application.Services;

/// <summary>
/// Servicio de lógica de negocio para Claims.
/// Aquí vive TODO el cerebro del microservicio: validaciones, reglas de negocio,
/// workflow de estados, llamadas a otros servicios, generación de ClaimNumber, auditoría.
/// 
/// Los controllers llaman a estos métodos y reciben un ServiceResult con el resultado o errores.
/// </summary>
public class ClaimService : IClaimService
{
    private readonly IClaimRepository _claimRepository;
    private readonly IClaimAuditRepository _auditRepository;
    private readonly IPoliciesClient _policiesClient;
    private readonly IReferenceDataClient _referenceDataClient;

    /// <summary>
    /// Transiciones válidas de status según el workflow:
    /// SUBMITTED → UNDER_REVIEW → APPROVED → PAID
    ///                           ↘ REJECTED
    /// La clave es el status actual, el valor es la lista de estados a los que puede pasar.
    /// </summary>
    private static readonly Dictionary<string, List<string>> ValidTransitions = new()
    {
        { "SUBMITTED", new List<string> { "UNDER_REVIEW" } },
        { "UNDER_REVIEW", new List<string> { "APPROVED", "REJECTED" } },
        { "APPROVED", new List<string> { "PAID" } },

    };

    public ClaimService(
        IClaimRepository claimRepository,
        IClaimAuditRepository auditRepository,
        IPoliciesClient policiesClient,
        IReferenceDataClient referenceDataClient)
    {
        _claimRepository = claimRepository;
        _auditRepository = auditRepository;
        _policiesClient = policiesClient;
        _referenceDataClient = referenceDataClient;
    }

    public async Task<ServiceResult<PaginatedResponse<ClaimResponse>>> GetAllAsync(
        int page, int pageSize, string? statusCode, string? policyNumber)
    {

        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var (claims, total) = await _claimRepository.GetAllAsync(page, pageSize, statusCode, policyNumber);

        var response = claims.Select(MapToResponse).ToList();

        var paginatedResponse = PaginatedResponse<ClaimResponse>.Success(response, page, pageSize, total);

        return ServiceResult<PaginatedResponse<ClaimResponse>>.Success(paginatedResponse);
    }

    public async Task<ServiceResult<ClaimResponse>> GetByIdAsync(int id)
    {
        var claim = await _claimRepository.GetByIdAsync(id);
        if (claim == null)
            return ServiceResult<ClaimResponse>.Failure("Id", "Claim not found", 404);

        var response = MapToResponse(claim);

        return ServiceResult<ClaimResponse>.Success(response);
    }

    public async Task<ServiceResult<ClaimResponse>> GetByClaimNumberAsync(string claimNumber)
    {
        var claim = await _claimRepository.GetByClaimNumberAsync(claimNumber);
        if (claim == null)
            return ServiceResult<ClaimResponse>.Failure("ClaimNumber", "Claim not found", 404);

        return ServiceResult<ClaimResponse>.Success(MapToResponse(claim));
    }

    public async Task<ServiceResult<ClaimResponse>> CreateAsync(CreateClaimRequest request)
    {

        var errors = ValidateCreateRequest(request);
        if (errors.Count > 0)
            return ServiceResult<ClaimResponse>.Failure(errors, 400);

        var policyResult = await ValidatePolicyAsync(request.PolicyNumber);
        if (!policyResult.IsValid)
            return ServiceResult<ClaimResponse>.Failure(policyResult.Field, policyResult.Message, policyResult.StatusCode);

        if (request.Amount > policyResult.CoverageAmount)
            return ServiceResult<ClaimResponse>.Failure("Amount",
                $"The claim amount (${request.Amount:N2}) exceeds the maximum coverage limit of ${policyResult.CoverageAmount:N2} for policy '{request.PolicyNumber}'.", 422);

        if (request.ClaimDate < policyResult.StartDate || request.ClaimDate > policyResult.EndDate)
            return ServiceResult<ClaimResponse>.Failure("ClaimDate",
                $"The claim date ({request.ClaimDate:yyyy-MM-dd}) must be within the policy period ({policyResult.StartDate:yyyy-MM-dd} to {policyResult.EndDate:yyyy-MM-dd}).", 422);

        var statusResult = await ValidateStatusCodeAsync("SUBMITTED");
        if (!statusResult.IsValid)
            return ServiceResult<ClaimResponse>.Failure(statusResult.Field, statusResult.Message, statusResult.StatusCode);

        var year = DateTime.UtcNow.Year;
        var sequence = await _claimRepository.GetNextSequenceNumberAsync(year);
        var claimNumber = $"CLM-{year}-{sequence:D5}";

        var claim = new Claim
        {
            ClaimNumber = claimNumber,
            PolicyNumber = request.PolicyNumber,
            ClaimDate = request.ClaimDate,
            StatusCode = "SUBMITTED",
            Amount = request.Amount,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _claimRepository.AddAsync(claim);

        return ServiceResult<ClaimResponse>.Success(MapToResponse(claim), 201);
    }

    public async Task<ServiceResult<ClaimResponse>> UpdateAsync(int id, UpdateClaimRequest request)
    {

        var claim = await _claimRepository.GetByIdAsync(id);
        if (claim == null)
            return ServiceResult<ClaimResponse>.Failure("Id", "Claim not found", 404);

        var errors = ValidateUpdateRequest(request);
        if (errors.Count > 0)
            return ServiceResult<ClaimResponse>.Failure(errors, 400);

        var policyResult = await ValidatePolicyAsync(request.PolicyNumber);
        if (!policyResult.IsValid)
            return ServiceResult<ClaimResponse>.Failure(policyResult.Field, policyResult.Message, policyResult.StatusCode);

        if (request.Amount > policyResult.CoverageAmount)
            return ServiceResult<ClaimResponse>.Failure("Amount",
                $"The claim amount (${request.Amount:N2}) exceeds the maximum coverage limit of ${policyResult.CoverageAmount:N2} for policy '{request.PolicyNumber}'.", 422);

        if (request.ClaimDate < policyResult.StartDate || request.ClaimDate > policyResult.EndDate)
            return ServiceResult<ClaimResponse>.Failure("ClaimDate",
                $"The claim date ({request.ClaimDate:yyyy-MM-dd}) must be within the policy period ({policyResult.StartDate:yyyy-MM-dd} to {policyResult.EndDate:yyyy-MM-dd}).", 422);

        var changedBy = "System";
        await RecordAuditChanges(claim, request, changedBy);

        claim.PolicyNumber = request.PolicyNumber;
        claim.ClaimDate = request.ClaimDate;
        claim.Amount = request.Amount;
        claim.Description = request.Description;
        claim.UpdatedAt = DateTime.UtcNow;

        await _claimRepository.UpdateAsync(claim);

        return ServiceResult<ClaimResponse>.Success(MapToResponse(claim));
    }

    public async Task<ServiceResult<ClaimResponse>> UpdateStatusAsync(int id, UpdateStatusRequest request)
    {

        var claim = await _claimRepository.GetByIdAsync(id);
        if (claim == null)
            return ServiceResult<ClaimResponse>.Failure("Id", "Claim not found", 404);

        var statusResult = await ValidateStatusCodeAsync(request.StatusCode);
        if (!statusResult.IsValid)
            return ServiceResult<ClaimResponse>.Failure(statusResult.Field, statusResult.Message, statusResult.StatusCode);

        if (!IsValidTransition(claim.StatusCode, request.StatusCode))
            return ServiceResult<ClaimResponse>.Failure("StatusCode",
                $"Invalid status transition from '{claim.StatusCode}' to '{request.StatusCode}'", 422);

        var audit = new ClaimAudit
        {
            ClaimId = claim.Id,
            ChangedBy = request.ChangedBy,
            FieldChanged = "StatusCode",
            OldValue = claim.StatusCode,
            NewValue = request.StatusCode,
            ChangedAt = DateTime.UtcNow
        };
        await _auditRepository.AddAsync(audit);

        claim.StatusCode = request.StatusCode;
        claim.UpdatedAt = DateTime.UtcNow;

        await _claimRepository.UpdateAsync(claim);

        return ServiceResult<ClaimResponse>.Success(MapToResponse(claim));
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var claim = await _claimRepository.GetByIdAsync(id);
        if (claim == null)
            return ServiceResult.Failure("Id", "Claim not found", 404);

        if (claim.StatusCode != "SUBMITTED")
            return ServiceResult.Failure("StatusCode",
                $"Cannot delete claim in '{claim.StatusCode}' status. Only claims in 'SUBMITTED' status can be deleted.", 422);

        await _claimRepository.DeleteAsync(claim);

        return ServiceResult.Success(204);
    }

    /// <summary>
    /// Valida que la póliza existe y está activa llamando al Policies Service.
    /// Si el servicio está caído, devuelve 503 en vez de lanzar excepción.
    /// </summary>
    private async Task<PolicyValidationResult> ValidatePolicyAsync(string policyNumber)
    {
        try
        {
            var response = await _policiesClient.GetPolicyByNumberAsync(policyNumber);

            if (response.Data == null)
                return PolicyValidationResult.Invalid("PolicyNumber", "Policy not found", 404);

            if (response.Data.Status != "ACTIVE")
                return PolicyValidationResult.Invalid("PolicyNumber",
                    $"Policy '{policyNumber}' is not active (current status: {response.Data.Status})", 422);

            return PolicyValidationResult.Valid(response.Data.CoverageAmount, response.Data.StartDate, response.Data.EndDate);
        }
        catch (Refit.ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return PolicyValidationResult.Invalid("PolicyNumber", $"Policy '{policyNumber}' not found", 404);
        }
        catch (Exception)
        {
            return PolicyValidationResult.Invalid("PolicyNumber",
                "Policies service is unavailable. Please try again later.", 503);
        }
    }

    /// <summary>
    /// Valida que un código de status existe y está activo en Reference Data.
    /// </summary>
    private async Task<StatusValidationResult> ValidateStatusCodeAsync(string code)
    {
        try
        {
            var response = await _referenceDataClient.GetClaimStatusByCodeAsync(code);

            if (response.Data == null)
                return StatusValidationResult.Invalid("StatusCode", $"Status code '{code}' not found", 404);

            if (!response.Data.IsActive)
                return StatusValidationResult.Invalid("StatusCode", $"Status code '{code}' is not active", 422);

            return StatusValidationResult.Valid();
        }
        catch (Refit.ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return StatusValidationResult.Invalid("StatusCode", $"Status code '{code}' not found", 404);
        }
        catch (Exception)
        {
            return StatusValidationResult.Invalid("StatusCode",
                "Reference Data service is unavailable. Please try again later.", 503);
        }
    }

    /// <summary>
    /// Valida si una transición de status es válida según el workflow definido.
    /// </summary>
    private static bool IsValidTransition(string currentStatus, string newStatus)
    {
        if (ValidTransitions.TryGetValue(currentStatus, out var allowedStatuses))
            return allowedStatuses.Contains(newStatus);

        return false;
    }

    /// <summary>
    /// Registra en la tabla de auditoría cada campo que cambió entre el claim actual y la request.
    /// </summary>
    private async Task RecordAuditChanges(Claim existing, UpdateClaimRequest request, string changedBy)
    {
        var now = DateTime.UtcNow;

        if (existing.PolicyNumber != request.PolicyNumber)
        {
            await _auditRepository.AddAsync(new ClaimAudit
            {
                ClaimId = existing.Id,
                ChangedBy = changedBy,
                FieldChanged = "PolicyNumber",
                OldValue = existing.PolicyNumber,
                NewValue = request.PolicyNumber,
                ChangedAt = now
            });
        }

        if (existing.ClaimDate != request.ClaimDate)
        {
            await _auditRepository.AddAsync(new ClaimAudit
            {
                ClaimId = existing.Id,
                ChangedBy = changedBy,
                FieldChanged = "ClaimDate",
                OldValue = existing.ClaimDate.ToString("yyyy-MM-dd"),
                NewValue = request.ClaimDate.ToString("yyyy-MM-dd"),
                ChangedAt = now
            });
        }

        if (existing.Amount != request.Amount)
        {
            await _auditRepository.AddAsync(new ClaimAudit
            {
                ClaimId = existing.Id,
                ChangedBy = changedBy,
                FieldChanged = "Amount",
                OldValue = existing.Amount.ToString("F2"),
                NewValue = request.Amount.ToString("F2"),
                ChangedAt = now
            });
        }

        if (existing.Description != request.Description)
        {
            await _auditRepository.AddAsync(new ClaimAudit
            {
                ClaimId = existing.Id,
                ChangedBy = changedBy,
                FieldChanged = "Description",
                OldValue = existing.Description,
                NewValue = request.Description,
                ChangedAt = now
            });
        }
    }

    /// <summary>
    /// Validaciones de campos obligatorios para crear un claim.
    /// </summary>
    private static List<ApiErrorItem> ValidateCreateRequest(CreateClaimRequest request)
    {
        var errors = new List<ApiErrorItem>();

        if (string.IsNullOrWhiteSpace(request.PolicyNumber))
            errors.Add(new ApiErrorItem("PolicyNumber", "Policy number is required"));

        if (request.ClaimDate == default)
            errors.Add(new ApiErrorItem("ClaimDate", "Claim date is required"));

        if (request.Amount <= 0)
            errors.Add(new ApiErrorItem("Amount", "Amount must be greater than 0"));

        if (string.IsNullOrWhiteSpace(request.Description))
            errors.Add(new ApiErrorItem("Description", "Description is required"));

        return errors;
    }

    /// <summary>
    /// Validaciones de campos obligatorios para actualizar un claim.
    /// </summary>
    private static List<ApiErrorItem> ValidateUpdateRequest(UpdateClaimRequest request)
    {
        var errors = new List<ApiErrorItem>();

        if (string.IsNullOrWhiteSpace(request.PolicyNumber))
            errors.Add(new ApiErrorItem("PolicyNumber", "Policy number is required"));

        if (request.ClaimDate == default)
            errors.Add(new ApiErrorItem("ClaimDate", "Claim date is required"));

        if (request.Amount <= 0)
            errors.Add(new ApiErrorItem("Amount", "Amount must be greater than 0"));

        if (string.IsNullOrWhiteSpace(request.Description))
            errors.Add(new ApiErrorItem("Description", "Description is required"));

        return errors;
    }

    /// <summary>
    /// Convierte una entidad Claim en un DTO ClaimResponse.
    /// Nunca exponemos entidades directamente al exterior.
    /// </summary>
    private static ClaimResponse MapToResponse(Claim claim)
    {
        return new ClaimResponse
        {
            Id = claim.Id,
            ClaimNumber = claim.ClaimNumber,
            PolicyNumber = claim.PolicyNumber,
            ClaimDate = claim.ClaimDate,
            StatusCode = claim.StatusCode,
            Amount = claim.Amount,
            Description = claim.Description,
            CreatedAt = claim.CreatedAt,
            UpdatedAt = claim.UpdatedAt
        };
    }

    private class PolicyValidationResult
    {
        public bool IsValid { get; set; }
        public string Field { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public decimal CoverageAmount { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        public static PolicyValidationResult Valid(decimal coverageAmount, DateOnly startDate, DateOnly endDate) =>
            new() { IsValid = true, CoverageAmount = coverageAmount, StartDate = startDate, EndDate = endDate };

        public static PolicyValidationResult Invalid(string field, string message, int statusCode) =>
            new() { IsValid = false, Field = field, Message = message, StatusCode = statusCode };
    }

    private class StatusValidationResult
    {
        public bool IsValid { get; set; }
        public string Field { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }

        public static StatusValidationResult Valid() => new() { IsValid = true };

        public static StatusValidationResult Invalid(string field, string message, int statusCode) =>
            new() { IsValid = false, Field = field, Message = message, StatusCode = statusCode };
    }
}
