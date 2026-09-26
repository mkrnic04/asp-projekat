using FluentValidation;
using System.Text.Json;

namespace BookReview.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";

                var errors = ex.Errors
                    .Select(x => new
                    {
                        Field = x.PropertyName,
                        Message = x.ErrorMessage
                    });

                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(errors));
            }
        }
    }
}