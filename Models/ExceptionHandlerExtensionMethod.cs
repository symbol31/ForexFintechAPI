using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ForexFintechAPI.Models;

//public static class ExceptionHandlerExtensionMethod
//{
//    public static IApplicationBuilder ExceptionHandling(this IApplicationBuilder app)
//    {
//        return app.UseMiddleware<ExceptionHandlerMiddleware>();
//    }
//}
public static class ExceptionHandlerExtensions
{
    public static void ExceptionHandling(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(builder =>
        {
            builder.Run(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/problem+json";

                var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (exceptionHandlerFeature != null)
                {
                    var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger("UnhandledException");
                    var exception = exceptionHandlerFeature.Error;

                    logger.LogError(exception, exception.StackTrace, exception.Message);

                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "An unhandled exception occurred",
                        Detail = exception.Message + exception.StackTrace
                    };

                    await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
                }
            });
        });
    }
}

