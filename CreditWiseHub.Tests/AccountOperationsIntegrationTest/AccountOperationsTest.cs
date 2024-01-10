using CreditWiseHub.Core.Dtos.Account;
using CreditWiseHub.Core.Dtos.Token;
using CreditWiseHub.Core.Dtos.Transactions;
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

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", content.Data.AccessToken);

            return client;
        }

        [Fact]
        public async Task CreateAccount_Returns_201Created()
        {

            // Arrange
            //Hesap Oluşturma
            var userId = "1";
            var username = "11111111111";
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
            var content = await response.Content.ReadAsStringAsync();
            var responseDto = JsonConvert.DeserializeObject<ResponseAccountInfoDto>(content);

            //Para Yatırma
            var senderAccountNumber = responseDto.Data.AccountNumber;
            MoneyProcessAmountDto amountDto = new()
            {
                Amount = 1000,
            };
            var expectedAmout = account.OpeningBalance + amountDto.Amount;

            var responseDeposit = await client.PostAsJsonAsync($"api/v1/accounts/{senderAccountNumber}/deposit", amountDto);
            var contentDeposit = await responseDeposit.Content.ReadAsStringAsync();
            var responseDepositDto = JsonConvert.DeserializeObject<ResponseAccountInfoDto>(contentDeposit);


            //Para Transferi
            
            var receiverUsername = "22222222222";
            var receiverUserAccountNumber = receiverUsername.Substring(0, 10);
            var receiverUserName = "User User";
            MoneyTransferDto transfer = new()
            {
                Amount = 1000,
                AccountInformation = new AffectedInBankAccountDto
                {
                    AccountNumber = receiverUserAccountNumber,
                    AccountHolderFullName = receiverUserName
                }
            };

            var responseTransfer = await client.PostAsJsonAsync($"api/v1/accounts/{senderAccountNumber}/InternalTransfer", transfer);
            var contentTransfer = await responseTransfer.Content.ReadAsStringAsync();
            var responseTransferDto = JsonConvert.DeserializeObject<ResponseTransactionStatusDto>(contentTransfer);

            //Hesap Bilgileri
            var expectedAfterTransferBalance = expectedAmout - transfer.Amount;
            var resReceiverAccountInfo = await client.GetAsync($"api/v1/accounts/{receiverUserAccountNumber}");
            var contentReceiverAccountInfo = await resReceiverAccountInfo.Content.ReadAsStringAsync();
            var responseReceiverAccountInfo = JsonConvert.DeserializeObject<ResponseAccountInfoDto>(contentReceiverAccountInfo);


            var resSenderAccountInfo = await client.GetAsync($"api/v1/accounts/{senderAccountNumber}");
            var contentSenderAccountInfo = await resSenderAccountInfo.Content.ReadAsStringAsync();
            var responseSenderAccountInfo = JsonConvert.DeserializeObject<ResponseAccountInfoDto>(contentSenderAccountInfo);


            // Assert

            response.EnsureSuccessStatusCode();
            responseDeposit.EnsureSuccessStatusCode();
            responseTransfer.EnsureSuccessStatusCode();

            Assert.NotNull(responseDto);
            Assert.NotNull(responseDto.Data);
            Assert.NotNull(responseDto.Data.AccountNumber);
            Assert.Equal(account.OpeningBalance, responseDto.Data.Balance);

            //Para Yatırma
            Assert.NotNull(responseDepositDto);
            Assert.NotNull(responseDepositDto.Data);
            Assert.NotNull(responseDepositDto.Data.AccountNumber);
            Assert.Equal(expectedAmout, responseDepositDto.Data.Balance);

            //Para Transferi
            Assert.NotNull(responseTransferDto);
            Assert.NotNull(responseTransferDto.Data);

            Assert.NotNull(responseReceiverAccountInfo);
            Assert.NotNull(responseReceiverAccountInfo.Data);
            Assert.Equal(transfer.Amount, responseReceiverAccountInfo.Data.Balance);

            Assert.NotNull(responseSenderAccountInfo);
            Assert.NotNull(responseSenderAccountInfo.Data);
            Assert.Equal(expectedAfterTransferBalance, responseSenderAccountInfo.Data.Balance);
        }



    }

    public abstract class Response
    {
        public ErrorDto Error { get; set; }
        public int StatusCode { get; set; }
    }

    public class ResponseTokenDto : Response
    {
        public TokenDto Data { get; set; }

    }
    public class ResponseAccountInfoDto : Response
    {
        public AccountInfoDto Data { get; set; }
    }

    public class ResponseUserAccountInfoDto : Response {

        public UserAccountsInfoDto Data { get; set; }

    }

    public class ResponseTransactionStatusDto : Response
    {

        public TransactionStatusDto Data { get; set; }

    }

}
