using API.Errors;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddScoped(typeof(IGenericRepository<>), (typeof(GenericRepository<>)));

            //This configuration allows the API to return a structured and consistent error response when model validation fails
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = ActionContext =>  //InvalidModelStateResponseFactory is used to define how the API should respond when the model state is invalid.
                {

                    // extracts error messages from the ModelState dictionary
                    var errors = ActionContext.ModelState
                    .Where(e => e.Value.Errors.Count > 0) //Filters entries in ModelState that have errors 
                    .SelectMany(x => x.Value.Errors) //Flattens the list of error messages
                    .Select(x => x.ErrorMessage).ToArray(); //Selects the error message strings and Converts the resulting into an array

                    //Creating a Custom Error Response
                    var errorResponse = new ApiValidationErrorResponse()
                    {
                        Errors = errors
                    };

                    return new BadRequestObjectResult(errorResponse); // is an HTTP 400 response
                };
            });
            return services;
        }
    }
}
