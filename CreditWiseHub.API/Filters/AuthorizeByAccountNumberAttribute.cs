using CreditWiseHub.Core.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace CreditWiseHub.API.Filters
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class AuthorizeByAccountNumberAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            var requestedAccountNumber = context.HttpContext.GetRouteValue("accountNumber")?.ToString();
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (user.IsInRole("User"))
            {
                var accountService = context.HttpContext.RequestServices.GetRequiredService<IAccountService>();

                var accountCheck = await accountService.DoesAccountBelongToUserId(requestedAccountNumber, userId);

                if (!accountCheck)
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }

            return;
        }
    }

}
