using CreditWiseHub.Core.Enums;
using CreditWiseHub.Core.Models;
using CreditWiseHub.Repository.Contexts;
using CreditWiseHub.Service.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Tests.Seeds
{
    public class SeedService
    {
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<UserApp> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public SeedService(AppDbContext appDbContext, UserManager<UserApp> userManager, RoleManager<IdentityRole> roleManager)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            var isExistsRole = await _roleManager.RoleExistsAsync(RoleNames.User.ToString());
            if (!isExistsRole)
            {
                foreach (var role in Enum.GetNames(typeof(RoleNames)))
                {
                    await _roleManager.CreateAsync(new IdentityRole { Name = role.ToString() });
                }
            }

            var accountTypes = new List<AccountType>{
                new AccountType
                {
                    Id = 1,
                    MinimumOpeningBalance = 0,
                    CreatedDate = DateTime.UtcNow,
                    Name = "Vadesiz Hesap",
                    Description = "Kullanıcı ilk kayıt olurken açılan hesap türü"
                },
                new AccountType
                {
                    Id = 2,
                    Name = "Vadeli Hesap",
                    MinimumOpeningBalance = 500,
                    Description = "Vadeli mevduat hesabı",
                    CreatedDate = DateTime.UtcNow,
                },
                new AccountType
                {
                    Id = 3,
                    Name = "Arı Hesap",
                    MinimumOpeningBalance = 100,
                    Description = "Günlük vadeli mevduat hesabı",
                    CreatedDate = DateTime.UtcNow,
                }};
            accountTypes.ForEach(x =>
            {
                var existingaccountType = _appDbContext.AccountTypes.Where(y => y.Id == x.Id).FirstOrDefault();

                if (existingaccountType is null)
                {
                    _appDbContext.AccountTypes.AddAsync(x);
                    _appDbContext.SaveChangesAsync();
                }
            });

            var loantypes = new List<LoanType>()
           {
               new LoanType
                {
                    Id=1,
                    Name = "İhtiyaç Kredisi",
                    InterestRate = 4,
                    MaxCreditScore = 500,
                    MinCreditScore = 100,
                    MaxInstallmentOption = 12,
                    MinInstallmentOption = 4,
                },
                new LoanType
                {
                    Id=2,
                    Name = "Ev Kredisi",
                    InterestRate = 10,
                    MaxCreditScore = 1000,
                    MinCreditScore = 600,
                    MaxInstallmentOption = 36,
                    MinInstallmentOption = 4,
                }
           };

            loantypes.ForEach(x =>
            {
                var existingaccountType = _appDbContext.LoanTypes.Where(y => y.Id == x.Id).FirstOrDefault();

                if (existingaccountType is null)
                {
                    _appDbContext.LoanTypes.Add(x);
                    _appDbContext.SaveChanges();
                }
            });

            var seedUsers = SeedData.GenerateSeedUsers();
            
            foreach (var user in seedUsers)
            {
                var existingUser = await _userManager.FindByNameAsync(user.UserName);

                if (existingUser == null)
                {
                    var result = await _userManager.CreateAsync(user, "Password123"); // Provide a default password

                    if (result.Succeeded)
                    {
                        // Assign roles if needed
                        await _userManager.AddToRoleAsync(user, GetRoleNameFromUserName(user.Name));

                        //add transaction limits
                        await _appDbContext.UserTransactionLimits.AddAsync(SeedData.CreateLimitData(user.Id));
                        await _appDbContext.SaveChangesAsync();

                        //add default account
                        await _appDbContext.Accounts.AddAsync(SeedData.CreateDefaultAccount(user.Id, Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper()));
                        await _appDbContext.SaveChangesAsync();
                    }
                }
            }
        }

        private string GetRoleNameFromUserName(string userName)
        {

            if (userName.Contains("Admin"))
            {
                return RoleNames.Admin.ToString();
            }
            else if (userName.Contains("Auditor"))
            {
                return RoleNames.Auditor.ToString();
            }
            else if (userName.Contains("CashDesk"))
            {
                return RoleNames.CashDesk.ToString();
            }
            else if (userName.Contains("CustomerService"))
            {
                return RoleNames.CustomerService.ToString();
            }

            return RoleNames.User.ToString(); 
        }
    }
}
