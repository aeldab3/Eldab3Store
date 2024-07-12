using API.Errors;
using System.Net;
using System.Text.Json;

namespace API.Middleware
{
    //Handle exceptions thrown in the request pipeline.
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        //This method is called for each HTTP request. It either calls the next middleware in the pipeline or handles an exception if one is thrown.
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); //Calls the next middleware in the pipeline
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message); //Logs the exception
                context.Response.ContentType = "application/json"; //Sets the response content type to JSON
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; //Sets the response status code to 500

                var response = _env.IsDevelopment()
                    ? new ApiException((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString()) //If in development environment, includes the exception message and stack trace.
                    : new ApiException((int)HttpStatusCode.InternalServerError); //If not in development, includes a generic "server error" message.

                //Serializes the ApiException object to JSON with camel case property naming.
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var json = JsonSerializer.Serialize(response, options);

                //Writes the JSON response to the HTTP response body
                await context.Response.WriteAsync(json);
            }
        }
    }


}


//A stack trace is a report that provides information about the active stack frames at a particular point in time during the execution of a program.
//When an exception occurs, the stack trace shows the method calls that led to the exception,