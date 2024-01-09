using CreditWiseHub.Core.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CreditWiseHub.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CustomBaseController : ControllerBase
    {

        public IActionResult ActionResultInstance<T>(Response<T> response) where T : class => new ObjectResult(response)
        {
            StatusCode = response.statusCode
        };

        protected string GetAutUserId()
        {
            return HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
