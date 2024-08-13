using System.Security.Claims;

namespace API.Extensions
{

    //This method is designed to retrieve the email address associated with a user from their claims
    public static class ClaimsPrincipalExtensions
    {
        public static string RetrieveEmailFromPrincipal(this ClaimsPrincipal user)
        {
            return user?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
        }
    }
}
