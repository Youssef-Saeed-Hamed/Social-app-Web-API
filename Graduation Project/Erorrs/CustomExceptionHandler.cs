using System.Net;

namespace Graduation_Project.Erorrs
{
    public class CustomExceptionHandler
    {
        private readonly RequestDelegate _next; 
        private readonly ILogger<CustomExceptionHandler> _logger;
        private readonly IHostEnvironment _environment;
        public CustomExceptionHandler(RequestDelegate next, ILogger<CustomExceptionHandler> logger, IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                //context.Response.ContentType = "application/json";
                var response = _environment.IsDevelopment() ?
                    new ApiExcepthionResponse((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace) :
                    new ApiExcepthionResponse((int)HttpStatusCode.InternalServerError);
                await context.Response.WriteAsJsonAsync(response);

            }
        }
    }
}
