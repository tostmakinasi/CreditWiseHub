using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace CreditWiseHub.API.Filters
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class AuthorizeByUsernameAttribute : Attribute, IAsyncAuthorizationFilter
    {
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
