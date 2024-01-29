using System.Text.Json;
using ECommerceApp.Errors;

// namespace ECommerceApp.Middlewares
// {
    class ErrorHandlerMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (UserNotFoundException exception)
            {
                var errorResponse = new ErrorResponse
                {
                    Message = exception.Message,
                    ErrorCode = "USER_NOT_FOUND",
                    ErrorDescription = "The requested user was not found"
                };

                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";

                var serializer = JsonSerializer.Serialize(errorResponse);
                await context.Response.WriteAsync(serializer);

                var logger = context.RequestServices.GetService<ILogger<ErrorHandlerMiddleware>>();
                logger?.LogError(exception, "An error occured while processing the request");
            }
            catch (Exception exception)
            {
                var errorResponse = new ErrorResponse
                {
                    Message = exception.Message,
                    StackTrace = exception.StackTrace
                };

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var serializer = JsonSerializer.Serialize(errorResponse);
                await context.Response.WriteAsync(serializer);

                var logger = context.RequestServices.GetService<ILogger<ErrorHandlerMiddleware>>();
                logger?.LogError(exception, "An error occured while processing the request");
            }
        }
    }
// }