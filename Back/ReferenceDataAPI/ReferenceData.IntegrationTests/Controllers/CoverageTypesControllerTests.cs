using Microsoft.AspNetCore.Mvc.Testing;
using ReferenceData.Application.Common;
using ReferenceData.Application.DTOs;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace ReferenceData.IntegrationTests.Controllers
{
    public class CoverageTypesControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions;
        private const string BaseRoute = "/api/v1/coverage-types";

        public CoverageTypesControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        #region GET - GetAll
        [Fact]
        public async Task GetAll_ReturnsOk_WithPagination()
        {
            // Act
            var response = await _client.GetAsync($"{BaseRoute}?page=1&pageSize=10&includeInactive=true");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        #endregion

        #region GET - GetById
        [Fact]
        public async Task GetById_ReturnsOk_WhenExists()
        {
            // Arrange
            var createRequest = new CreateCoverageTypeRequest { Code = "COV_01", Name = "Full Coverage" };
            var createRes = await _client.PostAsJsonAsync(BaseRoute, createRequest);
            var created = await createRes.Content.ReadFromJsonAsync<ApiResponse<CoverageTypeDto>>(_jsonOptions);
            int id = created.Data.Id;

            // Act
            var response = await _client.GetAsync($"{BaseRoute}/{id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<CoverageTypeDto>>(_jsonOptions);
            Assert.Equal(id, result.Data.Id);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenDoesNotExist()
        {
            // Act
            var response = await _client.GetAsync($"{BaseRoute}/99999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        #endregion

        #region GET - GetByCode
        [Fact]
        public async Task GetByCode_ReturnsOk_WhenExists()
        {
            // Arrange
            var code = "UNIQUE_CODE_XYZ";
            await _client.PostAsJsonAsync(BaseRoute, new CreateCoverageTypeRequest { Code = code, Name = "Unique Name" });

            // Act
            var response = await _client.GetAsync($"{BaseRoute}/by-code/{code}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<CoverageTypeDto>>(_jsonOptions);
            Assert.Equal(code, result.Data.Code);
        }
        #endregion

        #region POST - Create
        [Fact]
        public async Task Create_ReturnsCreated_WithValidData()
        {
            // Arrange
            var request = new CreateCoverageTypeRequest { Code = "NEW_COV", Name = "Standard Liability" };

            // Act
            var response = await _client.PostAsJsonAsync(BaseRoute, request);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(response.Headers.Location);
        }

        [Fact]
        public async Task Create_ReturnsConflict_WhenCodeAlreadyExists()
        {
            // Arrange
            var request = new CreateCoverageTypeRequest { Code = "DUP_CODE", Name = "First" };
            await _client.PostAsJsonAsync(BaseRoute, request);

            // Act
            var response = await _client.PostAsJsonAsync(BaseRoute, request);

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }
        #endregion

        #region PUT - Update
        [Fact]
        public async Task Update_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var createRes = await _client.PostAsJsonAsync(BaseRoute, new CreateCoverageTypeRequest { Code = "UPDATE_ME", Name = "Old Name" });
            var created = await createRes.Content.ReadFromJsonAsync<ApiResponse<CoverageTypeDto>>(_jsonOptions);

            var updateRequest = new UpdateCoverageTypeRequest { Name = "New Updated Name", IsActive = true };

            // Act
            var response = await _client.PutAsJsonAsync($"{BaseRoute}/{created.Data.Id}", updateRequest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<CoverageTypeDto>>(_jsonOptions);
            Assert.Equal("New Updated Name", result.Data.Name);
        }
        #endregion

        #region DELETE - Delete
        [Fact]
        public async Task Delete_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            var createRes = await _client.PostAsJsonAsync(BaseRoute, new CreateCoverageTypeRequest { Code = "DELETE_ME", Name = "To be deleted" });
            var created = await createRes.Content.ReadFromJsonAsync<ApiResponse<CoverageTypeDto>>(_jsonOptions);

            // Act
            var response = await _client.DeleteAsync($"{BaseRoute}/{created.Data.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenIdDoesNotExist()
        {
            // Act
            var response = await _client.DeleteAsync($"{BaseRoute}/88877");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        #endregion
    }
}