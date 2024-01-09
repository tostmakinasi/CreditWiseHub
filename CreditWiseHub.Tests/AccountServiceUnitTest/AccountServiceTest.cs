using AutoMapper;
using CreditWiseHub.Core.Abstractions.Repositories;
using CreditWiseHub.Core.Abstractions.Services;
using CreditWiseHub.Core.Abstractions.UnitOfWorks;
using CreditWiseHub.Core.Configurations;
using CreditWiseHub.Core.Dtos;
using CreditWiseHub.Core.Dtos.Account;
using CreditWiseHub.Core.Dtos.AutomaticPayment;
using CreditWiseHub.Core.Dtos.Transactions;
using CreditWiseHub.Core.Enums;
using CreditWiseHub.Core.Models;
using CreditWiseHub.Core.Responses;
using CreditWiseHub.Repository.Contexts;
using CreditWiseHub.Service.Exceptions;
using CreditWiseHub.Service.Helpers;
using CreditWiseHub.Service.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CreditWiseHub.Tests.AccountServiceUnitTest
{
    public class AccountServiceTest
    {
        private readonly Mock<IAccountRepository> _mockAccountRepository;
        private readonly Mock<IGenericRepository<AccountType, int> > _mockAccountTypeRepository;
        private readonly Mock<ITransactionService> _mockTransactionService;
        private readonly Mock<UserManager<UserApp>> _mockUserManager;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly AccountService _accountService;
        private readonly Mock<GuidHelper> _mockGuidHelper;
        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
        public AccountServiceTest()
        {
            _mockAccountRepository = new Mock<IAccountRepository>();
            _mockAccountTypeRepository = new Mock<IGenericRepository<AccountType, int>> ();
            _mockTransactionService = new Mock<ITransactionService>();
            _mockUserManager = MockUserManager();
            _mockMapper = new Mock<IMapper>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockGuidHelper = new Mock<GuidHelper>();


            _accountService = new AccountService(
            _mockAccountRepository.Object,
            _mockMapper.Object,
            _mockUnitOfWork.Object,
            _mockUserManager.Object,
            _mockAccountTypeRepository.Object,
            _mockTransactionService.Object,
            _mockGuidHelper.Object);

            
        }

        private static Mock<UserManager<UserApp>> MockUserManager()
        {
            var store = new Mock<IUserStore<UserApp>>();
            var options = new Mock<IOptions<IdentityOptions>>();
            var passwordHasher = new Mock<IPasswordHasher<UserApp>>();
            var userValidators = new List<IUserValidator<UserApp>>();
            var passwordValidators = new List<IPasswordValidator<UserApp>>();
            var keyNormalizer = new Mock<ILookupNormalizer>();
            var errors = new Mock<IdentityErrorDescriber>();
            var services = new Mock<IServiceProvider>();
            var logger = new Mock<ILogger<UserManager<UserApp>>>();

            return new Mock<UserManager<UserApp>>(
                store.Object, options.Object, passwordHasher.Object,
                userValidators, passwordValidators, keyNormalizer.Object,
                errors.Object, services.Object, logger.Object
            );
        }

        [Fact]
        public async Task DepositMoneyByAccountNumber_UpdatesBalanceCorrectly_ShouldReturn_200OK()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var accountNumber = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();
            var amount = 100;
            var initialBalance = 200;
            var expectedBalance = initialBalance + amount;
            var account = new Account { Id = accountId, AccountNumber= accountNumber, Balance = initialBalance };

            var createDepositDto = new MoneyProcessAmountDto { Amount = amount };

            _mockAccountRepository.Setup(repo => repo.GetAccountByAccountNumber(accountNumber)).ReturnsAsync(account);

            _mockTransactionService.Setup(service =>
                service.AddDepositWithdrawalProcess(account, amount, It.IsAny<string>(), false, TransactionType.Deposit, true))
                .ReturnsAsync(new Response<TransactionStatusDto>());
            // Act
            var response = await _accountService.DepositMoneyByAccountNumber(accountNumber, createDepositDto);

            // Assert
            Assert.Equal(expectedBalance, account.Balance);

            Assert.Equal((int)HttpStatusCode.OK, response.statusCode);
        }

        [Fact]
        public async Task WithdrawMoneyByAccountNumber_UpdatesBalanceCorrectly_ShouldReturn_200OK()
        {
            //Arrange
            var accountId = Guid.NewGuid();
            var accountNumber = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();
            var amount = 100;
            var initialBalance = 200;
            var expectedBalance = initialBalance - amount;
            var account = new Account { Id = accountId, AccountNumber = accountNumber, Balance = initialBalance, UserApp = new UserApp()
            {
                Id = "a",
                UserTransactionLimit = new UserTransactionLimit()
                {

                    UserId = "a",
                    InstantTransactionLimit = DefaultsTransactionLimits.InstantTransactionLimits,
                    DailyTransactionLimit = DefaultsTransactionLimits.MonthlyTransactionLimits,
                    DailyTransactionAmount = 0,
                    LastProcessDate = DateTime.UtcNow,
                }
            }
            };

            var Dto = new MoneyProcessAmountDto { Amount = amount };


            _mockAccountRepository.Setup(repo => repo.GetAccountByAccountNumberWithUserAndUserLimits(accountNumber)).ReturnsAsync(account);

            _mockTransactionService.Setup(service =>
                service.AddDepositWithdrawalProcess(account, amount, It.IsAny<string>(), false, TransactionType.Deposit, true))
                .ReturnsAsync(new Response<TransactionStatusDto>());

            // Act

            var response = await _accountService.WithdrawMoneyByAccountNumber(accountNumber, Dto);

            // Assert

            Assert.Equal(expectedBalance, account.Balance);

            Assert.Equal((int)HttpStatusCode.OK, response.statusCode);
        }

        [Fact]
        public async Task WithdrawMoneyByAccountNumber_InsufficientBalance_ShouldReturn_400BadRequest()
        {
            //Arrange
            var accountId = Guid.NewGuid();
            var accountNumber = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();
            var amount = 1000;
            var initialBalance = 200;
            var expectedBalance = initialBalance;
            var account = new Account
            {
                Id = accountId,
                AccountNumber = accountNumber,
                Balance = initialBalance,
                UserApp = new UserApp()
                {
                    Id = "a",
                    UserTransactionLimit = new UserTransactionLimit()
                    {

                        UserId = "a",
                        InstantTransactionLimit = DefaultsTransactionLimits.InstantTransactionLimits,
                        DailyTransactionLimit = DefaultsTransactionLimits.MonthlyTransactionLimits,
                        DailyTransactionAmount = 0,
                        LastProcessDate = DateTime.UtcNow,
                    }
                }
            };

            var Dto = new MoneyProcessAmountDto { Amount = amount };


            _mockAccountRepository.Setup(repo => repo.GetAccountByAccountNumberWithUserAndUserLimits(accountNumber)).ReturnsAsync(account);

            _mockTransactionService.Setup(service =>
                service.AddDepositWithdrawalProcess(account, amount, It.IsAny<string>(), false, TransactionType.Deposit, true))
                .ReturnsAsync(new Response<TransactionStatusDto>());

            // Act

            var response = await _accountService.WithdrawMoneyByAccountNumber(accountNumber, Dto);

            // Assert
            Assert.Equal(expectedBalance, account.Balance);
            Assert.Equal((int)HttpStatusCode.BadRequest, response.statusCode);
        }

        [Fact]
        public async Task WithdrawMoneyForAutomaticPayment_UpdatesBalanceCorrectly_ShouldReturn_200OK()
        {
            //Arrange
            var accountId = Guid.NewGuid();
            var accountNumber = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();
            var userID = "a";
            var amount = 100;
            var initialBalance = 200;
            var expectedBalance = initialBalance - amount;
            var account = new Account
            {
                Id = accountId,
                AccountNumber = accountNumber,
                Balance = initialBalance,
                UserApp = new UserApp()
                {
                    Id = userID,
                    UserTransactionLimit = new UserTransactionLimit()
                    {

                        UserId = userID,
                        InstantTransactionLimit = DefaultsTransactionLimits.InstantTransactionLimits,
                        DailyTransactionLimit = DefaultsTransactionLimits.MonthlyTransactionLimits,
                        DailyTransactionAmount = 0,
                        LastProcessDate = DateTime.UtcNow,
                    }
                }
            };

            var Dto = new PaymentProcessDto { PaymentAmount = amount, UserId = userID };
            var transactionDto = new TransactionStatusDto { TransactionDate = DateTime.UtcNow, TransactionId = 2121, TransactionStatus = "OK" };

            _mockAccountRepository.Setup(repo => repo.GetUserDefaultAccountByUserIdWithUserAndUserLimits(userID)).ReturnsAsync(account);

            _mockTransactionService.Setup(service =>
                service.AddDepositWithdrawalProcess(account, amount, It.IsAny<string>(), false, TransactionType.Withdraw, true))
                .ReturnsAsync( Response<TransactionStatusDto>.Success(transactionDto,HttpStatusCode.OK));

            // Act

            var response = await _accountService.WithdrawMoneyForAutomaticPayment(Dto);

            // Assert

            Assert.Equal(expectedBalance, account.Balance);

            Assert.Equal((int)HttpStatusCode.OK, response.statusCode);
        }

        [Fact]
        public async Task WithdrawMoneyForAutomaticPayment_DoesNotUpdatesBalance_ShouldReturn_404NotFound()
        {
            //Arrange
            var accountId = Guid.NewGuid();
            var accountNumber = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();
            var userID = "a";
            var amount = 100;
            var initialBalance = 200;
            var expectedBalance = initialBalance;
            var account = new Account
            {
                Id = accountId,
                AccountNumber = accountNumber,
                Balance = initialBalance,
            };

            var Dto = new PaymentProcessDto { PaymentAmount = amount, UserId = userID };
            var transactionDto = new TransactionStatusDto { TransactionDate = DateTime.UtcNow, TransactionId = 2121, TransactionStatus = "OK" };

            _mockTransactionService.Setup(service =>
                service.AddDepositWithdrawalProcess(account, amount, It.IsAny<string>(), false, TransactionType.Withdraw, true))
                .ReturnsAsync(Response<TransactionStatusDto>.Success(transactionDto, HttpStatusCode.OK));

            // Act

            var response = await _accountService.WithdrawMoneyForAutomaticPayment(Dto);

            // Assert

            Assert.Equal(expectedBalance, account.Balance);

            Assert.Equal((int)HttpStatusCode.NotFound, response.statusCode);
        }

        [Fact]
        public async Task CreateByUserName_CreatesAccountCorrectly_ShouldReturn_201Created()
        {
            //Arrange
            string username = "username";
            var user = new UserApp { Id= "id", UserName = username };
            var accountId = Guid.NewGuid();
            var createAccountDto = new CreateAccountDto()
            {
                Name = "New Account",
                AccountTypeId = 1,
                OpeningBalance = 500,
                Description = "description",
            };
            var accountType = new AccountType
            {
                Name = "TestType",
                MinimumOpeningBalance = 100,
                Description = "TestDescription"
            };
            var mappedAccpunt = new Account { Name = createAccountDto.Name, AccountTypeId = createAccountDto.AccountTypeId, Description = createAccountDto.Description };

            var expectedAccount = new Account
            {
                Id = accountId,
                UserAppId = username,
                AccountNumber = "TESTACCOUNT",
                AccountTypeId = accountType.Id,
                AccountType = accountType,
                CreatedDate = DateTime.UtcNow,
                Balance = createAccountDto.OpeningBalance
            };
            var expectedAccountInfoDto = new AccountInfoDto
            {
                Id = expectedAccount.Id.ToString(),
                Name = expectedAccount.Name,
                AccountNumber = expectedAccount.AccountNumber,
                AccountTypeName = expectedAccount.AccountType.Name,
                Balance = expectedAccount.Balance

            };

            _mockUserManager.Setup(x => x.FindByNameAsync(username)).ReturnsAsync(user);
            _mockAccountTypeRepository.Setup(x => x.GetByIdAsync(createAccountDto.AccountTypeId)).ReturnsAsync(accountType);
            _mockAccountRepository.Setup(x=> x.UserHaveDefaultAccountAnyAsync(x=> x.AccountTypeId == 1 && x.IsActive && x.UserAppId == user.Id)).ReturnsAsync(false);
            _mockMapper.Setup(x => x.Map<Account>(createAccountDto)).Returns(mappedAccpunt);
            _mockGuidHelper.Setup(x => x.NewGuid()).Returns(accountId);
            _mockGuidHelper.Setup(x => x.GetAccountNumberBaseGuid()).Returns(expectedAccount.AccountNumber);
            _mockAccountRepository.Setup(x => x.AnyAsync(x => x.AccountNumber == expectedAccount.AccountNumber)).ReturnsAsync(false);
            _mockMapper.Setup(x => x.Map<AccountInfoDto>(mappedAccpunt)).Returns(expectedAccountInfoDto);
            //Act

            var result = await _accountService.CreateByUserName(username, createAccountDto);

            //Asserts
            Assert.NotNull(result);
            Assert.Equal(result.data, expectedAccountInfoDto);
            Assert.Equal((int)HttpStatusCode.Created, result.statusCode);

        }

        [Fact]
        public async Task CreateByUserName_UserDoesNotExists_ShouldReturn_404NotFound()
        {
            //Arrange
            string username = "username";
            var createAccountDto = new CreateAccountDto() ;

            _mockUserManager.Setup(x => x.FindByNameAsync(username)).ReturnsAsync((UserApp)null);
            
            //Act

            var result = await _accountService.CreateByUserName(username, createAccountDto);

            //Asserts
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.NotFound, result.statusCode);

        }

        [Fact]
        public async Task CreateByUserName_OpeningAmountLessThanMinimumAmount_ShouldReturn_400BadRequest()
        {
            //Arrange
            string username = "username";
            var user = new UserApp { Id = "id", UserName = username };
            var createAccountDto = new CreateAccountDto()
            {
                Name = "New Account",
                AccountTypeId = 1,
                OpeningBalance = 100,
                Description = "description",
            };
            var accountType = new AccountType
            {
                Name = "TestType",
                MinimumOpeningBalance = 1000,
                Description = "TestDescription"
            };

            _mockUserManager.Setup(x => x.FindByNameAsync(username)).ReturnsAsync(user);
            _mockAccountTypeRepository.Setup(x => x.GetByIdAsync(createAccountDto.AccountTypeId)).ReturnsAsync(accountType);
            //Act

            var result = await _accountService.CreateByUserName(username, createAccountDto);

            //Asserts
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.statusCode);

        }

        [Fact]
        public async Task CreateByUserName_UserAlreadyHaveDefaultAccountType_ShouldReturn_400BadRequest()
        {
            //Arrange
            string username = "username";
            var user = new UserApp { Id = "id", UserName = username };
            var createAccountDto = new CreateAccountDto()
            {
                Name = "New Account",
                AccountTypeId = 1,
                OpeningBalance = 100,
                Description = "description",
            };
            var accountType = new AccountType
            {
                Name = "TestType",
                MinimumOpeningBalance = 1000,
                Description = "TestDescription"
            };

            _mockUserManager.Setup(x => x.FindByNameAsync(username)).ReturnsAsync(user);
            _mockAccountTypeRepository.Setup(x => x.GetByIdAsync(createAccountDto.AccountTypeId)).ReturnsAsync(accountType);
            _mockAccountRepository.Setup(x => x.UserHaveDefaultAccountAnyAsync(x => x.AccountTypeId == 1 && x.IsActive && x.UserAppId == user.Id)).ReturnsAsync(true);
            //Act

            var result = await _accountService.CreateByUserName(username, createAccountDto);

            //Asserts
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.statusCode);

        }

        [Fact]
        public async Task GetAccountInfoByAccountNumber_ShouldReturn_200OK()
        {
            //Arrange
            var accountID = Guid.NewGuid();
            var accountType = new AccountType
            {
                Name = "TestType",
                MinimumOpeningBalance = 100,
                Description = "TestDescription"
            };
            var expectedAccount = new Account
            {
                Id = accountID,
                AccountNumber = "TESTACCOUNT",
                AccountTypeId = accountType.Id,
                AccountType = accountType,
                CreatedDate = DateTime.UtcNow,
                Balance = 15003
            };
            var expectedAccountInfoDto = new AccountInfoDto
            {
                Id = expectedAccount.Id.ToString(),
                Name = expectedAccount.Name,
                AccountNumber = expectedAccount.AccountNumber,
                AccountTypeName = expectedAccount.AccountType.Name,
                Balance = expectedAccount.Balance

            };

            _mockAccountRepository.Setup(x => x.GetAccountByAccountNumber(expectedAccount.AccountNumber)).ReturnsAsync(expectedAccount);
            _mockMapper.Setup(x=> x.Map<AccountInfoDto>(expectedAccount)).Returns(expectedAccountInfoDto);

            //Act

            var response = await _accountService.GetAccountInfoByAccountNumber(expectedAccount.AccountNumber);

            //Assert

            Assert.NotNull(response);
            Assert.Equal(response.data, expectedAccountInfoDto);

        }

        [Fact]
        public async Task GetAccountBalanceByAccountNumber_ShouldReturn_200OK()
        {
            //Arrange

            var expectedAccount = new Account
            {
                AccountNumber = "TESTACCOUNT",
                Balance = 12312
            };

            _mockAccountRepository.Setup(x => x.GetAccountByAccountNumber(expectedAccount.AccountNumber)).ReturnsAsync(expectedAccount);


            //Act

            var response = await _accountService.GetAccountBalanceByAccountNumber(expectedAccount.AccountNumber);

            //Assert

            Assert.NotNull(response);
            Assert.Equal(response.data.Balance, expectedAccount.Balance);

        }

        [Fact]
        public async Task InternalMoneyTransfer_ShouldReturn_200OK()
        {
            //Arrange
            var senderAccount = new Account
            {
                Id = Guid.NewGuid(),
                AccountNumber = Guid.NewGuid().ToString("N").Substring(0,10),
                Balance = 5000,
                UserApp = new UserApp()
                {
                    Id = "user",
                    UserTransactionLimit = new UserTransactionLimit()
                    {

                        UserId = "user",
                        InstantTransactionLimit = DefaultsTransactionLimits.InstantTransactionLimits,
                        DailyTransactionLimit = DefaultsTransactionLimits.MonthlyTransactionLimits,
                        DailyTransactionAmount = 0,
                        LastProcessDate = DateTime.UtcNow,
                    }
                }
            };
            
            var receiverAccount = new Account
            {
                Id = Guid.NewGuid(),
                AccountNumber = Guid.NewGuid().ToString("N").Substring(0, 10),
                Balance = 5000,
                UserApp = new UserApp()
                {
                    Id = "user",
                    Name = "test",
                    Surname = "receiver"
                }
            };

            MoneyTransferDto moneyTransferDto = new()
            {
                Amount = 100,
                AccountInformation = new()
                {
                    AccountHolderFullName = "test receiver",
                    AccountNumber = receiverAccount.AccountNumber
                }
            };

            TransactionStatusDto transactionStatusDto = new()
            {
                TransactionId = 12331,
                TransactionStatus = "Complated",
                TransactionDate = DateTime.UtcNow,
            };

            var expectedReceiverAccountBalance = receiverAccount.Balance + moneyTransferDto.Amount;
            var expectedSenderAccountBalance = senderAccount.Balance - moneyTransferDto.Amount;
            _mockAccountRepository.Setup(x => x.GetAccountByAccountNumberWithUserAndUserLimits(senderAccount.AccountNumber)).ReturnsAsync(senderAccount);
            _mockAccountRepository.Setup(x => x.GetAccountByAccountNumber(moneyTransferDto.AccountInformation.AccountNumber)).ReturnsAsync(receiverAccount);
            _mockTransactionService.Setup(x => x.CreateInternalTransaction(receiverAccount, senderAccount, moneyTransferDto)).ReturnsAsync(Response<TransactionStatusDto>.Success(transactionStatusDto, HttpStatusCode.OK));

            //Act

            var response = await _accountService.InternalMoneyTransfer(senderAccount.AccountNumber, moneyTransferDto);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(expectedReceiverAccountBalance, receiverAccount.Balance);
            Assert.Equal(expectedSenderAccountBalance, senderAccount.Balance);
            Assert.Equal(response.data, transactionStatusDto);
            Assert.Equal((int)HttpStatusCode.OK, response.statusCode);
        }

        [Fact]
        public async Task ExternalTransfer_InComingTransfer_ShouldReturn_200OK()
        {
            //Arrange
            var inBankAccount = new Account
            {
                Id = Guid.NewGuid(),
                AccountNumber = Guid.NewGuid().ToString("N").Substring(0, 10),
                Balance = 5000,
                UserApp = new UserApp()
                {
                    Id = "user",
                    UserTransactionLimit = new UserTransactionLimit()
                    {

                        UserId = "user",
                        InstantTransactionLimit = DefaultsTransactionLimits.InstantTransactionLimits,
                        DailyTransactionLimit = DefaultsTransactionLimits.MonthlyTransactionLimits,
                        DailyTransactionAmount = 0,
                        LastProcessDate = DateTime.UtcNow,
                    }
                }
            };

            MoneyExternalTransferDto moneyExternalTransferDto = new()
            {
                TransferType = MoneyTransferType.Incoming,
                Amount = 500,
                AccountInformation = new()
                {
                    AccountNumber = "externalAccountNumber",
                    BankName = "AlbarakaTürk",
                    OwnerFullName = "Sender Account Holder"
                }
            };

            TransactionStatusDto transactionStatusDto = new()
            {
                TransactionId = 12331,
                TransactionStatus = "Complated",
                TransactionDate = DateTime.UtcNow,
            };
            var expectedBalance = inBankAccount.Balance + moneyExternalTransferDto.Amount;
            _mockAccountRepository.Setup(x => x.GetAccountByAccountNumberWithUserAndUserLimits(inBankAccount.AccountNumber)).ReturnsAsync(inBankAccount);
            _mockTransactionService.Setup(x => x.CreateExternalTransaction(inBankAccount, moneyExternalTransferDto)).ReturnsAsync(Response<TransactionStatusDto>.Success(transactionStatusDto, HttpStatusCode.OK));

            //Act

            var response = await _accountService.ExternalTransfer(inBankAccount.AccountNumber, moneyExternalTransferDto);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(response.data, transactionStatusDto);
            Assert.Equal(expectedBalance, inBankAccount.Balance);
            Assert.Equal((int)HttpStatusCode.OK, response.statusCode);
        }

        [Fact]
        public async Task ExternalTransfer_OutGoingTransfer_ShouldReturn_200OK()
        {
            //Arrange
            var inBankAccount = new Account
            {
                Id = Guid.NewGuid(),
                AccountNumber = Guid.NewGuid().ToString("N").Substring(0, 10),
                Balance = 5000,
                UserApp = new UserApp()
                {
                    Id = "user",
                    UserTransactionLimit = new UserTransactionLimit()
                    {

                        UserId = "user",
                        InstantTransactionLimit = DefaultsTransactionLimits.InstantTransactionLimits,
                        DailyTransactionLimit = DefaultsTransactionLimits.MonthlyTransactionLimits,
                        DailyTransactionAmount = 0,
                        LastProcessDate = DateTime.UtcNow,
                    }
                }
            };
            
            MoneyExternalTransferDto moneyExternalTransferDto = new()
            {
                TransferType = MoneyTransferType.Outgoing,
                Amount = 500,
                AccountInformation = new()
                {
                    AccountNumber = "externalAccountNumber",
                    BankName = "AlbarakaTürk",
                    OwnerFullName = "Sender Account Holder"
                }
            };

            TransactionStatusDto transactionStatusDto = new()
            {
                TransactionId = 12331,
                TransactionStatus = "Complated",
                TransactionDate = DateTime.UtcNow,
            };

            var expectedBalance = inBankAccount.Balance - moneyExternalTransferDto.Amount;

            _mockAccountRepository.Setup(x => x.GetAccountByAccountNumberWithUserAndUserLimits(inBankAccount.AccountNumber)).ReturnsAsync(inBankAccount);
            _mockTransactionService.Setup(x => x.CreateExternalTransaction(inBankAccount, moneyExternalTransferDto)).ReturnsAsync(Response<TransactionStatusDto>.Success(transactionStatusDto, HttpStatusCode.OK));

            //Act

            var response = await _accountService.ExternalTransfer(inBankAccount.AccountNumber, moneyExternalTransferDto);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(response.data, transactionStatusDto);
            Assert.Equal(expectedBalance, inBankAccount.Balance);
            Assert.Equal((int)HttpStatusCode.OK, response.statusCode);
        }

        [Fact]
        public async Task UpdateAccountAsync()
        {
            //Arrange
            var expectedAccount = new Account
            {
                Name = "Test",
                AccountNumber = "TESTACCOUNT",
                Balance = 12312
            };

            _mockAccountRepository.Setup(x => x.GetAccountByAccountNumber(expectedAccount.AccountNumber)).ReturnsAsync(expectedAccount);

            UpdateAccountDto updateAccountDto = new()
            {
                Name = "NewTestAccountName",
                Description = "NewAccountDescription"
            };

            //Act
            var response = await _accountService.UpdateAccountAsync(expectedAccount.AccountNumber,updateAccountDto);
            //Assert
            Assert.NotNull(response);
            Assert.Equal(response.data, (NoDataDto)null);
            Assert.Equal(expectedAccount.Name, updateAccountDto.Name);
            Assert.Equal(expectedAccount.Description, updateAccountDto.Description);
            Assert.Equal((int)HttpStatusCode.OK,response.statusCode);
        }

        [Fact]
        public async Task CloseAccount_ShouldReturn_204NoContent()
        {
            //Arrange
            var closedAccount = new Account
            {
                Id = Guid.NewGuid(),
                AccountNumber = Guid.NewGuid().ToString("N").Substring(0, 10),
                Balance = 5000,
                AccountTypeId = 2,
                UserAppId = "user",
            };

            var userDefaultAccount = new Account
            {
                Id = Guid.NewGuid(),
                AccountNumber = Guid.NewGuid().ToString("N").Substring(0, 10),
                Balance = 270,
                AccountTypeId = 1,
                UserAppId = "user",
            };
            var expectedDefaultAccountBalance = userDefaultAccount.Balance + closedAccount.Balance;
            var expectedClosedAccountBalance = 0;
            _mockAccountRepository.Setup(x => x.GetAccountByAccountNumber(closedAccount.AccountNumber)).ReturnsAsync(closedAccount);
            _mockAccountRepository.Setup(x => x.GetUserDefaultAccountByUserIdWithUserAndUserLimits(closedAccount.UserAppId)).ReturnsAsync(userDefaultAccount);

            //Act
            var response = await _accountService.CloseAccount(closedAccount.AccountNumber);

            //Assert

            _mockAccountRepository.Setup(x => x.GetAccountByAccountNumber(closedAccount.AccountNumber)).ReturnsAsync((Account)null);
            await Assert.ThrowsAsync<NotFoundException>(() => _accountService.CheckAccountByAccountNumber(closedAccount.AccountNumber));

            Assert.NotNull(response);
            Assert.Equal(response.data, (NoDataDto)null);
            Assert.Equal(expectedDefaultAccountBalance,userDefaultAccount.Balance);
            Assert.Equal(expectedClosedAccountBalance, closedAccount.Balance);
            Assert.Equal((int)HttpStatusCode.NoContent, response.statusCode);
        }
    }
}
