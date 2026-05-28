using Identity_login.Application.Interfaces;
using Identity_login.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Identity_login.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;

        // Konstruktor för att kunna läsa vår Secret Key från appsettings.json
        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateJwtToken(UserEntity user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            // Hämtar den hemliga nyckeln vi skrev i appsettings.json
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Secret"]!);

            // Här packar vi in den information om användaren som ska ligga inuti vår Token (Claims)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(15),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}