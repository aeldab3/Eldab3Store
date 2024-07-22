using Core.Entities.Identity;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    // responsible for generating JWT (JSON Web Tokens) for authenticating users
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        // This represents the key used to sign the JWT
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration config)
        {
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Token:Key"]));
        }

        public string CreateToken(AppUser user)
        {
            //Claims are pieces of information about the user, included in the token.
            var claims = new List<Claim>
            {
               new Claim(ClaimTypes.Email, user.Email),
               new Claim(ClaimTypes.GivenName, user.DisplayName)
            };

            //This specifies how the token will be signed.
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // defines the token's properties
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //Claims identity containing the user's claims.
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds, //Credentials used to sign the token.
                //Issuer of the token, fetched from the configuration.
                Issuer = _config["Token:Issuer"],
            };

            //JwtSecurityTokenHandler is used to create the JWT token using the tokenDescriptor.
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            //method converts the token to a string format.
            return tokenHandler.WriteToken(token);
        }
    }
}
