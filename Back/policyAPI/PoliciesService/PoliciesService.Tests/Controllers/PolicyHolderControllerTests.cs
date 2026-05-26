// PoliciesService\PoliciesService.Tests\Controllers\PolicyHolderControllerTests.cs
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PoliciesService.Application.Common;
using PoliciesService.Application.DTOs.External;
using PoliciesService.Application.DTOs.PolicyHolder;
using PoliciesService.Application.Interfaces;
using PoliciesService.Infrastructure;
using PoliciesService.Tests.TestHelpers;
using Xunit;
using Moq;

namespace PoliciesService.Tests.Controllers
{
    public class PolicyHolderControllerTests : IClassFixture<PolicyApiFactory>
    {
        private readonly HttpClient _client;
        private readonly PolicyApiFactory _factory;
        private readonly Mock<IReferenceDataClient> _refitMock;

        public PolicyHolderControllerTests(PolicyApiFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _refitMock = factory.ReferenceDataClientMock;
        }

        private void SetupRefitMocks()
        {
            _refitMock.Invocations.Clear();
            var response = new Refit.ApiResponse<ReferenceDataPagedResponse<RegionDTO>>(
                new HttpResponseMessage(HttpStatusCode.OK),
                new ReferenceDataPagedResponse<RegionDTO> { Data = new List<RegionDTO> { new RegionDTO { Code = "US", Name = "United States" } } },
                new Refit.RefitSettings());
            _refitMock.Setup(x => x.GetAllRegionsAsync()).ReturnsAsync(response);
        }

        private async Task SeedDatabaseAsync(Func<AppDbContext, Task> seeder)
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await seeder(db);
        }

        [Fact]
        public async Task GetAll_ReturnsPaginatedHolders()
        {
            // Arrange
            var holders = MockDataGenerator.GeneratePolicyHolders(2010);
            await SeedDatabaseAsync(async db =>
            {
                db.PolicyHolders.AddRange(holders);
                await db.SaveChangesAsync();
            });

            // Act
            var response = await _client.GetAsync("/api/v1/policy-holders?page=1&pageSize=20");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<PaginatedResponse<PolicyHolderResponseDTO>>();
            content.Should().NotBeNull();
            content!.Data.Should().NotBeEmpty();
            content.Total.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetById_ExistingId_ReturnsHolderWithPolicies()
        {
            // Arrange
            var holder = MockDataGenerator.GeneratePolicyHolders(1).First();
            var policies = MockDataGenerator.GeneratePolicies(2, holder.Id);
            holder.Policies = policies;
            await SeedDatabaseAsync(async db =>
            {
                db.PolicyHolders.Add(holder);
                await db.SaveChangesAsync();
            });

            // Act
            var response = await _client.GetAsync($"/api/v1/policy-holders/{holder.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<ApiResponse<PolicyHolderResponseDTO>>();
            content.Should().NotBeNull();
            content!.Data.Should().NotBeNull();
            content.Data!.Id.Should().Be(holder.Id);
        }

        [Fact]
        public async Task Create_ValidHolder_ReturnsCreated()
        {
            // Arrange
            var request = MockDataGenerator.GeneratePolicyHolderRequest();
            SetupRefitMocks();

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/policy-holders", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var content = await response.Content.ReadFromJsonAsync<ApiResponse<PolicyHolderResponseDTO>>();
            content.Should().NotBeNull();
            content!.Data.Should().NotBeNull();
            content.Data!.FirstName.Should().Be(request.FirstName);
        }

        [Fact]
        public async Task Update_ExistingHolder_ReturnsOk()
        {
            // Arrange
            var holder = MockDataGenerator.GeneratePolicyHolders(1).First();
            await SeedDatabaseAsync(async db =>
            {
                db.PolicyHolders.Add(holder);
                await db.SaveChangesAsync();
            });

            var updateRequest = MockDataGenerator.GeneratePolicyHolderRequest();
            SetupRefitMocks();

            // Act
            var response = await _client.PutAsJsonAsync($"/api/v1/policy-holders/{holder.Id}", updateRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<ApiResponse<PolicyHolderResponseDTO>>();
            content.Should().NotBeNull();
            content!.Data.Should().NotBeNull();
            content.Data!.FirstName.Should().Be(updateRequest.FirstName);
        }

        [Fact]
        public async Task Delete_HolderWithoutPolicies_ReturnsNoContent()
        {
            // Arrange
            var holder = MockDataGenerator.GeneratePolicyHolders(1).First();
            await SeedDatabaseAsync(async db =>
            {
                db.PolicyHolders.Add(holder);
                await db.SaveChangesAsync();
            });

            // Act
            var response = await _client.DeleteAsync($"/api/v1/policy-holders/{holder.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_HolderWithActivePolicies_ReturnsBadRequest()
        {
            // Arrange
            var holder = MockDataGenerator.GeneratePolicyHolders(1).First();
            var policies = MockDataGenerator.GeneratePolicies(1, holder.Id);
            holder.Policies = policies;
            await SeedDatabaseAsync(async db =>
            {
                db.PolicyHolders.Add(holder);
                await db.SaveChangesAsync();
            });

            // Act
            var response = await _client.DeleteAsync($"/api/v1/policy-holders/{holder.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().NotBeNullOrEmpty();
        }
    }
}
