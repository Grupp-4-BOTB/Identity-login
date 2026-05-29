using Identity_login.Application.Interfaces;
using Identity_login.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;


// TokenService är klassen som implementerar gränssnittet. Det är här i TokenService som den riktiga logiken
// och koden finns, som till exempel hur vi skapar en JwtSecurityTokenHandler,
// läser vår hemliga nyckel och genererar själva token-strängen.

namespace Identity_login.Application.Services
{// Deklarerar klassen TokenService som implementerar (följer kontraktet för) ITokenService
    public class TokenService : ITokenService
    {// En privat variabel för att kunna komma åt applikationens konfigurationsfiler (som appsettings.json eller User Secrets)
        private readonly IConfiguration _config;

        // Konstruktor för att kunna läsa vår Secret Key från appsettings.json
        public TokenService(IConfiguration config)
        {
            _config = config;
        }
        // En offentlig metod som tar emot en användare (UserEntity) och skapar en digital JWT-åtkomstnyckel
        public string GenerateJwtToken(UserEntity user)
        {// Skapar en instans av JwtSecurityTokenHandler, vilket är verktyget i .NET som faktiskt bygger och skriver ut JWT-tokens
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
            // Skapar en ritning (descriptor) som beskriver exakt hur vår token ska byggas och vad den ska innehålla
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // Sätter listan med användaruppgifter (claims) som identitet för denna token
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(15),
                // Sätter utgivaren (vem som skapade token), vilket hämtas från våra inställningar
                Issuer = _config["Jwt:Issuer"],
                // Sätter mottagaren (vem som har rätt att använda token, i detta fall ditt Next.js frontend)
                Audience = _config["Jwt:Audience"],
                // Signerar token digitalt med vår hemliga nyckel och krypteringsalgoritmen HmacSha256 för att förhindra manipulering
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            // Bygger upp själva token-objektet i minnet baserat på inställningarna i vår descriptor
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {// Skapar en tom array av bytes som ska fyllas med 64 slumpmässiga tal (totalt 512 bitars säkerhet)
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            // Fyller vår byte-array med helt slumpmässiga värden direkt från datorns hårdvarusäkerhet
            rng.GetBytes(randomNumber);
            // Konverterar de råa slumpmässiga bytesen till en ren och läsbar Base64-textsträng som returneras
            return Convert.ToBase64String(randomNumber);
        }
    }
}