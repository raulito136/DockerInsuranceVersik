using Moq;
using Xunit;
using Claims.Application.Services;
using Claims.Application.Interfaces;
using Claims.Application.DTOs;
using Claims.Application.DTOs.External;

namespace Claims.Test;

public class ClaimServiceTests
{

    private readonly Mock<IClaimRepository> _mockClaimRepo;
    private readonly Mock<IClaimAuditRepository> _mockAuditRepo;
    private readonly Mock<IPoliciesClient> _mockPoliciesClient;
    private readonly Mock<IReferenceDataClient> _mockReferenceClient;

    private readonly ClaimService _claimService;

    public ClaimServiceTests()
    {

        _mockClaimRepo = new Mock<IClaimRepository>();
        _mockAuditRepo = new Mock<IClaimAuditRepository>();
        _mockPoliciesClient = new Mock<IPoliciesClient>();
        _mockReferenceClient = new Mock<IReferenceDataClient>();

        _claimService = new ClaimService(
            _mockClaimRepo.Object,
            _mockAuditRepo.Object,
            _mockPoliciesClient.Object,
            _mockReferenceClient.Object
        );
    }

    [Fact]
    public async Task CreateAsync_WhenPolicyDoesNotExist_ReturnsFailureResult()
    {

        var request = new CreateClaimRequest
        {
            PolicyNumber = "POL-NO-EXISTE",
            ClaimDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Amount = 1000,
            Description = "Test description"
        };

        _mockPoliciesClient
            .Setup(c => c.GetPolicyByNumberAsync(request.PolicyNumber))
            .ReturnsAsync(new PolicyApiResponse { Data = null });

        var result = await _claimService.CreateAsync(request);

        Assert.False(result.IsSuccess);

        Assert.Equal(404, result.StatusCode);

        Assert.Contains("not found", result.Errors[0].Message.ToLower());

        _mockClaimRepo.Verify(r => r.AddAsync(It.IsAny<Domain.Claim>()), Times.Never);
    }
}
