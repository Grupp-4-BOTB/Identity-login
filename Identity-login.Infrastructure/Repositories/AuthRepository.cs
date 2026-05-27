using Identity_login.Application.DTOs;
using Identity_login.Application.Interfaces;
using Identity_login.Domain.Entities;
using Microsoft.AspNetCore.Identity;


namespace Identity_login.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<UserEntity> _userManager;

        // Konstruktor för att kunna prata med Microsoft Identity-databasen
        public AuthRepository(UserManager<UserEntity> userManager)
        {
            _userManager = userManager;
        }

        // SKÄRM 1: Hämtar användaren för att se om e-posten finns och om kontot är bekräftat
        public async Task<UserEntity?> GetByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        // SKÄRM 2: Kontrollerar lösenordet och loggar in användaren
        public async Task<AuthResponseDto?> LoginAsync(LoginRequest request)
        {
            // Logik för inloggning mot databasen byggs här näst
            return null;
        }

        // SKÄRM 3: Sparar användaren i databasen när registreringsflödet slutförs
        public async Task<bool> CreateUserAsync(UserEntity user)
        {
            var result = await _userManager.CreateAsync(user);
            return result.Succeeded;
        }
    }
}