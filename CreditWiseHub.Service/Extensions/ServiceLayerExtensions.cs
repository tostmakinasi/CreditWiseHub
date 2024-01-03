using CreditWiseHub.Core.Abstractions.Services;
using CreditWiseHub.Service.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CreditWiseHub.Service.Extensions
{
    public static class ServiceLayerExtensions
    {
        public static IServiceCollection AddServicesWithExtension(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAccountTypeService, AccountTypeService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITransactionService, TransactionService>();


            return services;
        }
    }
}
