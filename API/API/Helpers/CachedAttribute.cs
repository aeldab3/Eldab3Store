using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace API.Helpers
{
    //To cache the responses of specific API endpoints to improve performance and reduce the load on the server
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        // Private field to store the time-to-live (TTL) value in seconds
        private readonly int _timeToLiveSeconds;

        public CachedAttribute(int timeToLiveSeconds)
        {
            _timeToLiveSeconds = timeToLiveSeconds;
        }

        // This method is called before and after an action method is executed.
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Retrieve the IResponseCacheService instance from the dependency injection container.
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();

            // Generate a unique cache key based on the incoming HTTP request.
            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
            // Attempt to retrieve a cached response using the generated cache key.
            var cachedResponse = await cacheService.GetCachedResponseAsync
                (cacheKey);

            // If a cached response is found, short-circuit the request pipeline and return the cached response.
            if (!string.IsNullOrEmpty(cachedResponse))
            {
                var contentResult = new ContentResult
                {
                    Content = cachedResponse,
                    ContentType = "application/json",
                    StatusCode = 200,
                };
                context.Result = contentResult;
                return;
            }
            // If no cached response is found, proceed with the action method execution.
            var executedContext = await next();

            // After the action method has executed, check if the result is an OkObjectResult (HTTP 200).
            if (executedContext.Result is OkObjectResult okObjectResult)
            {
                // Cache the response with the generated cache key and TTL.
                await cacheService.CacheResponseAsync(cacheKey, okObjectResult.Value, TimeSpan.FromSeconds(_timeToLiveSeconds));
            }
        }

        // Helper method to generate a unique cache key based on the HTTP request.
        private static string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();
            // Start the key with the request path (e.g., "/api/values").
            keyBuilder.Append($"{request.Path}");
            // Append each query parameter in the request to the key, ordered by the parameter name.
            foreach ( var (key, value) in request.Query.OrderBy(x =>x.Key )){
                keyBuilder.Append($"|{key}-{value}");
            }

            return keyBuilder.ToString();
        }
    }
}
