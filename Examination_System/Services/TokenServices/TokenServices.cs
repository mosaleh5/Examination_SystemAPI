using Examination_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Examination_System.Services.TokenServices
{
    public class TokenServices : ITokenServices
    {
        IConfiguration _configuration ; 
        public TokenServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<string> CreateTokenAsync(User user, UserManager<User> userManager, IList<string> role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName)
            };
            
            // Add each role as a separate claim
            foreach (var r in role)
            {
                claims.Add(new Claim(ClaimTypes.Role, r));
            }
            
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
            
            // Fixed: Use AddMinutes instead of AddHours, and correct config key
            var expirationMinutes = Convert.ToDouble(_configuration["JwtSettings:ExpirationInMinutes"]);
            
            var Token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes), // Changed to UtcNow and AddMinutes
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );
            return new JwtSecurityTokenHandler().WriteToken(Token);
        }
    }
}
