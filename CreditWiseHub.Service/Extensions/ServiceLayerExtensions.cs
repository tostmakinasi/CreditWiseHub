using CreditWiseHub.Core.Abstractions.Services;
using CreditWiseHub.Service.Helpers;
using CreditWiseHub.Service.Seeds;
using CreditWiseHub.Service.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CreditWiseHub.Service.Extensions
{
    public static class ServiceLayerExtensions
    {
        public static IServiceCollection AddServicesWithExtension(this IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAccountTypeService, AccountTypeService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IAutomaticPaymentHistoryService, AutomaticPaymentHistoryService>();
            services.AddScoped<IAutomaticPaymentService, AutomaticPaymentService>();
            services.AddScoped<ILoanApplicationService, LoanApplicationService>();
            services.AddScoped<ILoanTypeService, LoanTypeService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICustomerTicketService, CustomerTicketService>();
            services.AddScoped<AccountHelper>();
            services.AddScoped<SeedService>();
            return services;
        }
    }
}
