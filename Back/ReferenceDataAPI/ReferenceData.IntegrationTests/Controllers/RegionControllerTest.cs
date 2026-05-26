using Microsoft.AspNetCore.Mvc.Testing;
using ReferenceData.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ReferenceData.IntegrationTests.Controllers
{
    public class RegionControllerTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions;
        private const string BaseRoute = "/api/v1/regions";

        public RegionControllerTest(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        #region GET - GetAll

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var response = await _client.GetAsync(BaseRoute);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        #endregion

        #region GET - GetById 

        [Fact]
        public async Task GetById_ReturnsOk_WhenExists()
        {
            var response = await _client.GetAsync($"{BaseRoute}/3");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenDoesNotExist()
        {
            var response = await _client.GetAsync($"{BaseRoute}/99999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        #endregion

        #region POST - Create

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var newRegion = new CreateRegionRequest
            {
                Code = "TEST",
                Name = "Test Region",
            };
            var response = await _client.PostAsJsonAsync(BaseRoute, newRegion);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task Create_ReturnsConflict_WhenDuplicate()
        {
            var duplicateRegion = new CreateRegionRequest
            {
                Code = "TEST",
                Name = "Duplicate Region"
            };
            var response = await _client.PostAsJsonAsync(BaseRoute, duplicateRegion);
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        #endregion
        #region PUT - Update

        [Fact]
        public async Task Update_ReturnsOk_WhenExists()
        {
            var updateRegion = new UpdateRegionRequest
            {
                Name = "Updated Region",
                IsActive = false
            };
            var response = await _client.PutAsJsonAsync($"{BaseRoute}/1", updateRegion);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenDoesNotExist()
        {
            var updateRegion = new UpdateRegionRequest
            {
                Name = "Updated Region",
                IsActive = false
            };
            var response = await _client.PutAsJsonAsync($"{BaseRoute}/9569", updateRegion);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        #endregion

        #region DELETE - Delete


        [Fact]
        public async Task Delete_ReturnsNoContent_WhenExists()
        {
            var response = await _client.DeleteAsync($"{BaseRoute}/2");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenDoesNotExist()
        {
            var response = await _client.DeleteAsync($"{BaseRoute}/99999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        #endregion

    }
}
