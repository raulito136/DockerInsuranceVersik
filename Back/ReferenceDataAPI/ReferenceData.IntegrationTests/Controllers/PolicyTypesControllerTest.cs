using Microsoft.AspNetCore.Http.HttpResults;
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
    public class PolicyTypesControllerTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions;
        private const string BaseRoute = "/api/v1/policy-types";

        public PolicyTypesControllerTest(WebApplicationFactory<Program> factory)
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
        public async Task GetById_ReturnsOk()
        {
            var response = await _client.GetAsync($"{BaseRoute}/4");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task GetById_ReturnsNotFound_WhenIdDoesNotExist()
        {
            var response = await _client.GetAsync($"{BaseRoute}/99999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        #endregion

        #region GET - GetByCode

        [Fact]
        public async Task GetByCode_ReturnsOk()
        {
            var response = await _client.GetAsync($"{BaseRoute}/by-code/CYBER");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetByCode_ReturnsNotFound_WhenCodeDoesNotExist()
        {
            var response = await _client.GetAsync($"{BaseRoute}/by-code/NOT_EXISTING_CODE");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        #endregion
        #region POST - Create
        [Fact]
        public async Task Create_ReturnsOk()
        {
            var request = new CreatePolicyTypeRequest
            {
                Code = "TEST_CODE",
                Name = "Test Name",
                Description = "Test Description"
            };

            var response = await _client.PostAsJsonAsync(BaseRoute, request);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task Create_ReturnsConflict_WhenCodeAlreadyExists()
        {
            var request = new CreatePolicyTypeRequest
            {
                Code = "CYBER",
                Name = "Test Name",
                Description = "Test Description"
            };

            var response = await _client.PostAsJsonAsync(BaseRoute, request);
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task Create_ReturnsConflict_WhenCodeIsEmpty()
        {
            var request = new CreateClaimStatusRequest
            {
                Code = "",
                Name = "Test Name",
                Description = "Test Description"
            };

            var response = await _client.PostAsJsonAsync(BaseRoute, request);

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        #endregion

        #region PUT - Update

        [Fact]
        public async Task Update_ReturnsOk_WhenIdExists()
        {
            var request = new UpdatePolicyTypeRequest
            {
                Name = "Updated Name",
                Description = "Updated Description",
                IsActive = true
            };

            var response = await _client.PutAsJsonAsync($"{BaseRoute}/4", request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenIdDoesNotExist()
        {
            var request = new UpdatePolicyTypeRequest
            {
                Name = "Updated Name",
                Description = "Updated Description",
                IsActive = true
            };

            var response = await _client.PutAsJsonAsync($"{BaseRoute}/258972", request);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }


        /** Cosa curiosa, esto responde un Ok, no deveria por que le estoy pasando un request con un Code, y el UpdatePolicyTypeRequest no tiene ese campo ademas de que le falta el IsActive,
         * Responde el Ok por que todo lo que tenga de mas lo ignora y lo que falta lo pone por default, en este caso el IsActive lo pone por default en false, lo cual hace que la actualizacion se haga correctamente con un IsActive en false.
         */
        [Fact]
        public async Task Update_ReturnsOkExtra_WhenRequestHasExtraFields()
        {
            var request = new CreateClaimStatusRequest
            {
                Code= "Esto lo ignorara",
                Name = "Hola",
                Description = "Updated Description"
            };

            var response= await _client.PutAsJsonAsync($"{BaseRoute}/4", request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        #endregion

        #region DELETE - Delete

        [Fact]
        public async Task Delete_ReturnsNotContent_WhenIdExists()
        {
            var response= await _client.DeleteAsync($"{BaseRoute}/3");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
             // Confirmamos que el registro sigue ahi (SoftDelete)
            var getResponse = await _client.GetAsync($"{BaseRoute}/2");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenIdDoesNotExist()
        {
            var response = await _client.DeleteAsync($"{BaseRoute}/99999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }


        [Fact]
        public async Task Delete_ReturnsNotFound_WhenIdIsInvalid()
        {
            var response = await _client.DeleteAsync($"{BaseRoute}/invalid_id");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        #endregion

    }

}
