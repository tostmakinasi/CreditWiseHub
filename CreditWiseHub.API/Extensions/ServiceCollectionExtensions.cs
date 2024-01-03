using CreditWiseHub.Core.Configurations;
using CreditWiseHub.Core.Dtos.Responses;
using CreditWiseHub.Core.Models;
using CreditWiseHub.Repository.Contexts;
using CreditWiseHub.Repository.Extensions;
using CreditWiseHub.Service.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text;

namespace CreditWiseHub.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddScopedWithExtension(this IServiceCollection services)
        {
            services.AddServicesWithExtension();
            services.AddRepositoriesWithExtension();

            return services;
        }

        public static IServiceCollection AddAuthenticationWithJwtOptionsExtension(this IServiceCollection services, CustomTokenOption tokenOptions)
        {

            services.AddIdentity<UserApp, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenOptions.SecurityKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

            return services;
        }

        public static IServiceCollection AddCustomValidationResponseExtension(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState.Values.Where(x => x.Errors.Count > 0).SelectMany(x => x.Errors).Select(x => x.ErrorMessage);

                    ErrorDto errorDto = new ErrorDto(errors.ToList(), true);
                    var response = Response<NoContentResult>.Fail(errorDto, HttpStatusCode.BadRequest);

                    return new BadRequestObjectResult(response);
                };

            });

            return services;
        }
    }
}
