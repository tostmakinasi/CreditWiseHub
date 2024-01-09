using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace CreditWiseHub.API.Filters
{
    /// <summary>
    /// An attribute that defines an authorization filter that allows only the user's own username to call the method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class AuthorizeByUsernameAttribute : Attribute, IAsyncAuthorizationFilter
    {
        /// <summary>
        /// Asynchronously checks the user's role and claim and decides whether to allow or forbid the method execution.
        /// </summary>
        /// <param name="context">The authorization filter context.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            var requestedUsername = context.HttpContext.GetRouteValue("username")?.ToString();

            if (user.IsInRole("User") && !user.HasClaim(ClaimTypes.Name, requestedUsername))
            {
                context.Result = new ForbidResult();
                return;
            }

            return;
        }
    }



}
