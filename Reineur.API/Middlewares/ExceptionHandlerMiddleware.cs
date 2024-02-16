using System.Net;

namespace Reineur.API.Middlewares
{
    public class ExceptionHandlerMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger)
        {
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {

                _logger.LogError("Something went wrong. Unhandle exception on Main project: \n\n{message} ", ex);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync(new ErrorDetail
                {
                    StatusCode = context.Response.StatusCode,
                    Message = $"Internal Serval Error: {ex.Message}."
                }.ToString());
            }
        }
    }

    public class ErrorDetail
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }

        public override string ToString()
        {
            return $"Error code: {StatusCode}\n Message: {Message}";
        }
    }
}
