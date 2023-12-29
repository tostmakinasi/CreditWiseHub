using CreditWiseHub.Core.Dtos.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CreditWiseHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomBaseController : ControllerBase
    {

        public IActionResult ActionResultInstance<T>(Response<T> response) where T : class => new ObjectResult(response)
        {
            StatusCode = response.StatusCode
        };
    }
}
