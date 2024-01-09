using CreditWiseHub.Core.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace CreditWiseHub.API.Filters
{
    /// <summary>
    /// Custom authorization attribute for ensuring that the logged-in user with the 'User' role owns the specified account number.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class AuthorizeByAccountNumberAttribute : Attribute, IAsyncAuthorizationFilter
    {
        /// <summary>
        /// Validates the authorization based on the user's role and ownership of the specified account number.
        /// </summary>
        /// <param name="context">The authorization filter context.</param>
        /// <returns>A task representing the asynchronous operation of checking and setting the authorization result.</returns>
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
