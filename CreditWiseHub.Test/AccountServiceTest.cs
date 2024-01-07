using AutoMapper;
using CreditWiseHub.Core.Abstractions.Repositories;
using CreditWiseHub.Core.Abstractions.Services;
using CreditWiseHub.Core.Abstractions.UnitOfWorks;
using CreditWiseHub.Core.Dtos.Account;
using CreditWiseHub.Core.Dtos.Transactions;
using CreditWiseHub.Core.Models;
using CreditWiseHub.Service.Services;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Test
{
    public class AccountServiceTest
    {
        private readonly Mock<IAccountRepository> _accountRepository;
        private readonly Mock<IGenericRepository<AccountType, int>> _accountTypeRepository;
        private readonly Mock<ITransactionService> _transactionService;
        private readonly Mock<UserManager<UserApp>> _userManager;
        private readonly Mock<IGenericRepository<UserTransactionLimit, string>> _userTransactionLimitRepository;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IMapper> _mapper;

        public AccountServiceTest()
        {
            _accountRepository = new Mock<IAccountRepository>();
            _accountTypeRepository = new Mock<IGenericRepository<AccountType, int>>();
            _transactionService = new Mock<ITransactionService>();
            _userManager = new Mock<UserManager<UserApp>>(Mock.Of<IUserStore<UserApp>>(), null, null, null, null, null, null, null, null);
            _userTransactionLimitRepository = new Mock<IGenericRepository<UserTransactionLimit, string>>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _mapper = new Mock<IMapper>();
        }

        [Fact]
        public async Task DepositMoney_Success()
        {
            // Arrange
            var accountService = new AccountService(
                _accountRepository.Object,
                _mapper.Object,
                _unitOfWork.Object,
                _userManager.Object,
                _accountTypeRepository.Object,
                _transactionService.Object,
                _userTransactionLimitRepository.Object
            );

            var moneyProcessAmountDto = new MoneyProcessAmountDto
            {
                Amount = 1200,
            };


            _userManager.Setup(um => um.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(new UserApp { Name="testuser", UserName = "testuser", Id="testuser" });

            _accountTypeRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new AccountType { Id =1, MinimumOpeningBalance =50, IsActive=true, Name="TestType" });
           
            // Mock other dependencies as needed.

            //// Act
            //var result = await accountService.CreateByUserName("testuser", createAccountDto);

            //// Assert
            //Assert.Equal((int)HttpStatusCode.Created, result.StatusCode);
            //Assert.NotNull(result.Data);
            // Add more assertions based on your requirements.
        }

    }
}
