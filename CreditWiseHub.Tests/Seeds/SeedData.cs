using CreditWiseHub.Core.Configurations;
using CreditWiseHub.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Tests.Seeds
{
    public static class SeedData
    {
        public static List<UserApp> GenerateSeedUsers()
        {
            var seedUsers = new List<UserApp>();

            // Seed for Admin
            seedUsers.Add(new UserApp
            {
                UserName = "11111111111",
                Name = "Admin",
                Surname = "User",
                DateOfBirth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                Email = "admin@example.com" // Provide a valid email
            });

            // Seed for User
            seedUsers.Add(new UserApp
            {
                UserName = "22222222222",
                Name = "User",
                Surname = "User",
                DateOfBirth = new DateTime(1995, 5, 5, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                Email = "user@example.com" // Provide a valid email
            });

            // Seed for Auditor
            seedUsers.Add(new UserApp
            {
                UserName = "33333333333",
                Name = "Auditor",
                Surname = "User",
                DateOfBirth = new DateTime(1985, 10, 10, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                Email = "auditor@example.com" // Provide a valid email
            });

            // Seed for CashDesk
            seedUsers.Add(new UserApp
            {
                UserName = "44444444444",
                Name = "CashDesk",
                Surname = "User",
                DateOfBirth = new DateTime(1980, 3, 15, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                Email = "cashdesk@example.com" // Provide a valid email
            });

            // Seed for CustomerService
            seedUsers.Add(new UserApp
            {
                UserName = "55555555555",
                Name = "CustomerService",
                Surname = "User",
                DateOfBirth = new DateTime(1992, 8, 20,0,0,0,DateTimeKind.Utc),
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                Email = "customerservice@example.com" // Provide a valid email
            });

            return seedUsers;
        }

        public static UserTransactionLimit CreateLimitData(string userId)
        {

            return new UserTransactionLimit
            {
                UserId = userId,
                InstantTransactionLimit = DefaultsTransactionLimits.InstantTransactionLimits,
                DailyTransactionLimit = DefaultsTransactionLimits.MonthlyTransactionLimits,
                DailyTransactionAmount = 0,
                LastProcessDate = DateTime.UtcNow,
            };
        }

        public static Account CreateDefaultAccount(string userId, string accountNumber)
        {
            return new Account
            {
                AccountNumber = accountNumber,
                Name = "Default Account",
                Description = "Default account for the user",
                Balance = 0, 
                UserAppId = userId,
                AccountTypeId = 1,
                CreatedDate = DateTime.UtcNow
            };
        }

       
    }
}
