using CreditWiseHub.Core.Dtos;
using CreditWiseHub.Core.Dtos.Responses;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace CreditWiseHub.API.Middlewares
{
    public static class CustomExceptionHandler
    {
        public static IApplicationBuilder UseCustomException(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(config =>
            {
                config.Run(async context =>
                {
                    context.Response.ContentType = "application/json";

                    var exceptionHandler = context.Features.Get<IExceptionHandlerFeature>();

                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    ErrorDto errorDto = new ErrorDto(exceptionHandler.Error.Message, false);

                    var response = Response<NoDataDto>.Fail(errorDto, HttpStatusCode.InternalServerError);

                    await context.Response.WriteAsJsonAsync(response);

                });
            });
            return app;
        }
    }
}
