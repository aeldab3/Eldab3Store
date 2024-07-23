using Microsoft.OpenApi.Models;

namespace API.Extensions
{
    public static class SwaggerServiceExtensions
    {
        public static IServiceCollection AddSwaggerDocumentaion(this IServiceCollection services)
        {

            services.AddSwaggerGen(c =>
            {
                //All codes below to make swagger accept a token

                // Add basic Swagger documentation with versioning
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

                // Define a security schema for JWT authentication
                var securitySchema = new OpenApiSecurityScheme
                {
                    // A description of the security scheme to be displayed in the Swagger UI
                    Description = "JWT Auth Bearer Schema",
                    // The name of the header where the JWT token should be included in requests
                    Name = "Authorization",
                    // Specifies that the token should be passed in the HTTP headers
                    In = ParameterLocation.Header,
                    // Sets the type of security scheme to HTTP
                    Type = SecuritySchemeType.Http,
                    // Defines the scheme used for authorization as "bearer"
                    Scheme = "bearer",
                    // Creates a reference to this security scheme
                    Reference = new OpenApiReference
                    {
                        // Specifies that this reference is to a security scheme
                        Type = ReferenceType.SecurityScheme,
                        // A unique identifier for this security scheme
                        Id = "Bearer"
                    }
                };
                // Adds the "Bearer" security definition to Swagger
                c.AddSecurityDefinition("Bearer", securitySchema);
                // Creates a security requirement that specifies the "Bearer" security scheme must be used
                var securityRequirement = new OpenApiSecurityRequirement { { securitySchema, new[] { "Bearer" } } };

                // Adds the security requirement to Swagger's configuration
                // meaning that any endpoints that require authentication will expect a JWT token to be present in the requests.
                c.AddSecurityRequirement(securityRequirement);
            });

            return services;
        }
    }
}
