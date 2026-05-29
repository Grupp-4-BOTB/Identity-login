using Identity_login.Application.DTOs;
using Identity_login.Application.Interfaces;
using Identity_login.Domain.Entities;
using Identity_login.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;


// AuthRepository används som databaslagret i vår applikation för att sköta all direkt datakommunikation kring användare.
// Genom att implementera IAuthRepository kapslar vi in Microsoft Identitys UserManager och vår DataContext på ett ställe.
// Det innebär att affärslogiken (AuthService) slipper veta hur SQL-frågor eller databasuppdateringar görs i bakgrunden.


// Definierar att denna klass ligger i mappen Repositories inom infrastrukturlagret
namespace Identity_login.Infrastructure.Repositories
{// Deklarerar klassen AuthRepository som implementerar (följer reglerna för) gränssnittet IAuthRepository
    public class AuthRepository : IAuthRepository
    {// Skapar en privat variabel för Microsofts UserManager för att kunna hantera användare i Identity-systemet
        private readonly UserManager<UserEntity> _userManager;
        // Skapar en privat variabel för vår tokentjänst för att kunna skapa digitala nycklar (JWT)
        private readonly ITokenService _tokenService;
        // Skapar en privat variabel för vår DataContext för att kunna spara ändringar direkt i SQL-tabellerna
        private readonly DataContext _dataContext;

        // Konstruktor för att kunna prata med Microsoft Identity-databasen
        public AuthRepository(UserManager<UserEntity> userManager, ITokenService tokenService, DataContext dataContext)
        {
            // Sparar det injicerade UserManager-verktyget i vår privata variabel
            _userManager = userManager;
            // Sparar den injicerade tokentjänsten i vår privata variabel
            _tokenService = tokenService;
            // Sparar den injicerade DataContext i vår privata variabel
            _dataContext = dataContext;
        }

        //  Hämtar användaren för att se om e-posten finns och om kontot är bekräftat
        public async Task<UserEntity?> GetByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        //  Metod som hanterar själva inloggningskontrollen i databasen

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






        //  Sparar användaren i databasen när registreringsflödet slutförs
        public async Task<bool> CreateUserAsync(UserEntity user, string password)
        {
            var result = await _userManager.CreateAsync(user,password);
            return result.Succeeded;
        }


        // Metod som används för att uppdatera en befintlig användares fält
        // (till exempel när RefreshToken sätts till NULL vid utloggning
        public async Task UpdateUserAsync(UserEntity user)
        {
            // Informerar Entity Framework Core om att användarobjektets data har ändrats och behöver uppdateras
            _dataContext.Users.Update(user);
            // Sparar ändringarna permanent i Azure SQL-databasen genom att köra en asynkron SQL UPDATE-fråga
            await _dataContext.SaveChangesAsync();
        }
    }
}