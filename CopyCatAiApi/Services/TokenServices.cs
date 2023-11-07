// Purpose: Handles the creation of JWT tokens for users.

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CopyCatAiApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace CopyCatAiApi.Services
{
    public class TokenServices
    {
        // Private fields
        private readonly IConfiguration _config;
        private readonly UserManager<UserModel> _userManager;

        // Constructor using dependency injection
        public TokenServices(IConfiguration config, UserManager<UserModel> userManager)
        {
            _config = config;
            _userManager = userManager;
        }

        // Create a JWT token
        public async Task<string> CreateToken(UserModel user)
        {
            // JWT Payload
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.UserName!)
            };

            // Get the roles for the user
            var roles = await _userManager.GetRolesAsync(user);

            // Add the roles to the claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            // Payload complete

            // Signature and encryption
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["tokensettings:tokenKey"]!));

            // Create the signing credentials, Sha512 is the hashing algorithm
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Create the token descriptor
            var options = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds
            );

            // Create the token handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // Create the token
            var token = tokenHandler.WriteToken(options);

            // Return the token
            return token;
        }   
    }
}