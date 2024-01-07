using CreditWiseHub.Core.Abstractions.Repositories;
using CreditWiseHub.Core.Abstractions.UnitOfWorks;
using CreditWiseHub.Repository.Repositories;
using CreditWiseHub.Repository.UnitOfWorks;
using Microsoft.Extensions.DependencyInjection;

namespace CreditWiseHub.Repository.Extensions
{
    public static class RepositoryLayerExtensions
    {
        public static IServiceCollection AddRepositoriesWithExtension(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ILoanApplicationRepository, LoanApplicationRepository>();
            services.AddScoped<IAutomaticPaymentsRegistrationRepository, AutomaticPaymentsRegistrationRepository>();
            services.AddScoped<ITicketRepository, TicketRepository>();

            return services;
        }
    }
}
