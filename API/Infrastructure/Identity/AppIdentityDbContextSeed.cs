using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedUsersAsync(UserManager<AppUser> userManager)
        {
            if(!userManager.Users.Any()) //checks If there are no users in the UserManager
            {
                var user = new AppUser
                {
                    DisplayName = "Ahmed",
                    Email = "Ahmed@test.com",
                    UserName = "Ahmed@test.com",
                    Address = new Address
                    {
                        FirstName = "Ahmed",
                        LastName = "Eldab3",
                        City = "Elzayat",
                        State = "Gharbia",
                        Street = "Ahmed-Shawky",
                        ZipCode = "911 ",
                        

                    }
                };
                await userManager.CreateAsync(user, "Pa$$w0rd");
            }
        }
    }
}
