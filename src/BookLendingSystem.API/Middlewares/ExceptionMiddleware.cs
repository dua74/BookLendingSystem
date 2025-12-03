using System.Net;
using System.Text.Json;
using BookLendingSystem.Application.Exceptions;

namespace BookLendingSystem.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
              
                await _next(httpContext);
            }
            catch (Exception ex)
            {
               
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

        
            var statusCode = exception switch
            {
                NotFoundException => (int)HttpStatusCode.NotFound, 
                BadRequestException => (int)HttpStatusCode.BadRequest, 
                _ => (int)HttpStatusCode.InternalServerError 
            };

            context.Response.StatusCode = statusCode;

            var response = new
            {
                statusCode = statusCode,
                message = exception.Message,
                
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
