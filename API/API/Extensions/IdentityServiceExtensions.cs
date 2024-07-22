using Core.Entities.Identity;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            // Adds core identity services for the specified user type (AppUser). it configures the identity system with the basic setup required for user management.
            var builder = services.AddIdentityCore<AppUser>();

            // Creates a new IdentityBuilder to configure additional identity services.
            builder = new IdentityBuilder(builder.UserType, builder.Services);

            // Adds Entity Framework stores for identity, specifying the context (AppIdentityDbContext).
            //This sets up the necessary services to use Entity Framework for identity data storage.
            builder.AddEntityFrameworkStores<AppIdentityDbContext>();

            // Adds the SignInManager service for the specified user type (AppUser). That handles user sign-in operations.
            builder.AddSignInManager<SignInManager<AppUser>>();

            // It sets up the basic infrastructure needed for authentication in the application.
            //This line adds authentication services and specifies that JWT Bearer authentication is the default scheme. 
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                //The options parameter allows you to set various settings related to JWT authentication.
                .AddJwtBearer(
                  options =>
                  {
                      //This property sets the parameters used to validate tokens.
                      options.TokenValidationParameters = new TokenValidationParameters
                      {
                          //This setting ensures that the token’s signing key is validated.
                          ValidateIssuerSigningKey = true,

                          //This line sets the key that will be used to validate the token’s signature.
                          // It converts a secret key (stored in the configuration) from a UTF-8 encoded string to a byte array
                          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Token:Key"])),
                          //This sets the expected issuer of the token. 
                          ValidIssuer = config["Token:Issuer"], 
                          //This setting ensures that the token’s issuer is validated against the expected issuer (specified in ValidIssuer).
                          ValidateIssuer = true,
                          ValidateAudience = false,
                      };
                  }); 

            // Returns the modified IServiceCollection for further configuration.
            return services;
        }
    }
}
