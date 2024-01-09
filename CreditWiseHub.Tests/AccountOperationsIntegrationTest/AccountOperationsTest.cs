using CreditWiseHub.Core.Dtos.Account;
using CreditWiseHub.Core.Dtos.Token;
using CreditWiseHub.Core.Dtos.User;
using CreditWiseHub.Core.Models;
using CreditWiseHub.Core.Responses;
using Microsoft.AspNetCore.Identity;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Reflection.PortableExecutable;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace CreditWiseHub.Tests.AccountOperationsIntegrationTest
{
    public class AccountOperationsTest : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public AccountOperationsTest(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;

        }

        

        public async Task<HttpClient> GetLoaggedClientAsync()
        {
            var client = _factory.CreateClient();
            LoginDto login = new()
            {
                TCKN = "44444444444",
                Password = "Password123"
            };
            var response  = await client.PostAsJsonAsync($"api/v1/Auth/CreateToken", login);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<ResponseTokenDto>();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,

            };

            var data = new Response<TokenDto>();
            var tokenDtoResponse = JsonConvert.DeserializeObject<ResponseTokenDto>("");
            //Burada gelen jsonu bir türlü deserialize edemedim. 
            //var data = tokenDtoResponse.data;
            //var accesstoken = tokenDtoResponse;
            //var tokenDto = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenDto>(data);

            //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            return client;
        }

        [Fact]
        public async Task CreateAccount_Returns_Ok()
        {

            // Arrange
            var userId = "1";
            var username = "User4455661";
            UserApp user = new() { UserName = username,Id =  userId };

            CreateAccountDto account = new()
            {
                Name = "TestAccount",
                AccountTypeId = 2,
                OpeningBalance = 500,
                Description = "Testaccount"
            };
            var client = await GetLoaggedClientAsync();
            var response = await client.PostAsJsonAsync($"api/v1/users/{username}/accounts", account);

            // Assert

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<Response<TokenDto>>();
            //JsonReader.
            //Response<TokenDto> responsedto = Newtonsoft.Json.JsonSerializer.Deserialize<Response<TokenDto>>(content.);
            //var machines = System.Text.Json.JsonSerializer.Deserialize<List<Machine>>(content);

            //Assert.NotNull(machines);

            //Assert.NotEmpty(machines);

            //Assert.Equal(7, machines.First().DepartmentId);
        }
    }

    public class ResponseTokenDto : Response<TokenDto>
    {

    }
}
