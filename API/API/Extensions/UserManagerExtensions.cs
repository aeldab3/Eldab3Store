using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Extensions
{
    public static class UserManagerExtensions
    {
        public static async Task<AppUser> FindUserByClaimsPrincipleWithAddressAsync(this UserManager<AppUser> input, ClaimsPrincipal User)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            return await input.Users.Include(x => x.Address).SingleOrDefaultAsync(x => x.Email == email);
        }

        public static async Task<AppUser> FindByEmailFromClaimsPrinciple(this UserManager<AppUser> input, ClaimsPrincipal User)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            return await input.Users.SingleOrDefaultAsync(x => x.Email == email);
        }
    }
}
