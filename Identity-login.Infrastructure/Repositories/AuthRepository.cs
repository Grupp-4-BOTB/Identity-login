using Identity_login.Application.DTOs;
using Identity_login.Application.Interfaces;
using Identity_login.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Identity_login.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly ITokenService _tokenService;

        // Konstruktor för att kunna prata med Microsoft Identity-databasen
        public AuthRepository(UserManager<UserEntity> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        // SKÄRM 1: Hämtar användaren för att se om e-posten finns och om kontot är bekräftat
        public async Task<UserEntity?> GetByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        // SKÄRM 2: Kontrollerar lösenordet och loggar in användaren

        public async Task<AuthResponseDto?> LoginAsync(LoginRequest request)
        {
            // 1. Leta efter användaren i Azure via e-post
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) return null;

            // 2. Kontrollera lösenordet med Identitys inbyggda funktion
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

            if (isPasswordValid)
            {
                // 3. Om lösenordet är rätt, skapa din JWT-token via vår nya tjänst!
                var generatedToken = _tokenService.GenerateJwtToken(user);

                return new AuthResponseDto
                {
                    Token = generatedToken,
                    IsSuccess = true,
                    Message = "Inloggningen lyckades!"
                };
            }

            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = "Felaktigt lösenord eller e-postadress."
            };
        }






        // SKÄRM 3: Sparar användaren i databasen när registreringsflödet slutförs
        public async Task<bool> CreateUserAsync(UserEntity user, string password)
        {
            var result = await _userManager.CreateAsync(user,password);
            return result.Succeeded;
        }
    }
}