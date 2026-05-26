using Microsoft.AspNetCore.Mvc.Testing;
using ReferenceData.Application.Common;
using ReferenceData.Application.DTOs;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace ReferenceData.IntegrationTests.Controllers
{
    public class ClaimStatusesControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions;
        private const string BaseRoute = "/api/v1/claim-statuses";

        public ClaimStatusesControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        #region GET - GetAll
        [Fact]
        public async Task GetAll_ReturnsOk_WithPagination()
        {
            // Act
            var response = await _client.GetAsync($"{BaseRoute}?page=1&pageSize=10");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            // Nota: Aquí podrías deserializar a tu clase de paginación si la tienes
        }
        #endregion

        #region GET - GetById
        [Fact]
        public async Task GetById_ReturnsOk_WhenExists()
        {
            // Arrange: Primero creamos uno para asegurar que existe (o usamos un ID conocido de seed)
            var createRequest = new CreateClaimStatusRequest { Code = "EXISTING_ID", Name = "Existing" };
            var createRes = await _client.PostAsJsonAsync(BaseRoute, createRequest);
            var created = await createRes.Content.ReadFromJsonAsync<ApiResponse<ClaimStatusDto>>(_jsonOptions);
            int id = created.Data.Id;

            // Act
            var response = await _client.GetAsync($"{BaseRoute}/{id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<ClaimStatusDto>>(_jsonOptions);
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
            var code = "SEARCH_ME";
            await _client.PostAsJsonAsync(BaseRoute, new CreateClaimStatusRequest { Code = code, Name = "Search" });

            // Act
            var response = await _client.GetAsync($"{BaseRoute}/by-code/{code}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<ClaimStatusDto>>(_jsonOptions);
            Assert.Equal(code, result.Data.Code);
        }

        [Fact]
        public async Task GetByCode_ReturnsNotFound_WhenCodeDoesNotExist()
        {
            // Act
            var response = await _client.GetAsync($"{BaseRoute}/by-code/NON_EXISTENT_CODE");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        #endregion

        #region POST - Create
        [Fact]
        public async Task Create_ReturnsCreated_WithValidData()
        {
            // Arrange
            var request = new CreateClaimStatusRequest { Code = "NEW_STATUS", Name = "New Status Name" };

            // Act
            var response = await _client.PostAsJsonAsync(BaseRoute, request);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(response.Headers.Location); // Verifica que CreatedAtAction puso la URL
        }

        [Fact]
        public async Task Create_ReturnsConflict_WhenCodeAlreadyExists()
        {
            // Arrange
            var request = new CreateClaimStatusRequest { Code = "DUPLICATE", Name = "First" };
            await _client.PostAsJsonAsync(BaseRoute, request); // Primera vez

            // Act: Intentamos crear el mismo código
            var response = await _client.PostAsJsonAsync(BaseRoute, request);

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }
        #endregion

        #region PUT - Update
        [Fact]
        public async Task Update_ReturnsOk_WhenSuccessful()
        {
            // Arrange: Crear uno para editar
            var createRes = await _client.PostAsJsonAsync(BaseRoute, new CreateClaimStatusRequest { Code = "EDIT_ME", Name = "Original" });
            var created = await createRes.Content.ReadFromJsonAsync<ApiResponse<ClaimStatusDto>>(_jsonOptions);

            var updateRequest = new UpdateClaimStatusRequest { Name = "Updated Name", IsActive = false };

            // Act
            var response = await _client.PutAsJsonAsync($"{BaseRoute}/{created.Data.Id}", updateRequest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<ClaimStatusDto>>(_jsonOptions);
            Assert.Equal("Updated Name", result.Data.Name);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenIdDoesNotExist()
        {
            // Arrange
            var updateRequest = new UpdateClaimStatusRequest { Name = "No existo", IsActive = true };

            // Act
            var response = await _client.PutAsJsonAsync($"{BaseRoute}/88888", updateRequest);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        #endregion

        #region DELETE - Delete

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenIdDoesNotExist()
        {
            // Act
            var response = await _client.DeleteAsync($"{BaseRoute}/77777");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        #endregion
    }
}