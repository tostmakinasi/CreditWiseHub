using CreditWiseHub.Core.Dtos;
using CreditWiseHub.Core.Dtos.Responses;
using CreditWiseHub.Service.Exceptions;
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

                    var statusCode = exceptionHandler.Error switch
                    {
                        BadRequestException => HttpStatusCode.BadRequest,
                        NotFoundException => HttpStatusCode.NotFound,
                        _ => HttpStatusCode.InternalServerError
                    };

                    context.Response.StatusCode = (int)statusCode;
                    ErrorDto errorDto = new ErrorDto(exceptionHandler.Error.Message, false);

                    var response = Response<NoDataDto>.Fail(errorDto, statusCode);

                    await context.Response.WriteAsJsonAsync(response);

                });
            });
            return app;
        }
    }
}
