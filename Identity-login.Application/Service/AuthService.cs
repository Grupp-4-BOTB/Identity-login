using BCrypt;
using Identity_login.Application.DTOs;
using Identity_login.Application.Interfaces;
using Identity_login.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

// AuthService används för att samla och hantera applikationens affärslogik kring säkerhet på ett och samma ställe.
// Genom att separera denna logik från controllern följer vi principen om att varje klass bara ska ha ett ansvarsområde.
// Den fungerar som en länk som kopplar ihop databaslagret (AuthRepository) med verktyget för att skapa tokens (TokenService).



namespace Identity_login.Application.Services;

// Skapar klassen AuthService som hanterar själva logiken för registrering, inloggning och utloggning
public class AuthService
{
    // Skapar två privata variabler för att hålla reda på databasverktyget och tokenverktyget
    private readonly IAuthRepository _authRepository;
    private readonly ITokenService _tokenService;

    // Konstruktor: Här injiceras databaslagret (repository) och tokentjänsten så att klassen kan använda dem
    public AuthService(IAuthRepository authRepository, ITokenService tokenService)
    {
        _authRepository = authRepository;
        _tokenService = tokenService;
        
    }
    // En asynkron metod för att registrera en ny användare i systemet
    // En asynkron metod är att koden kan utföra uppgifter i bakgrunden utan att blockera eller frysa resten av programmet.
    public async Task<bool> RegisterAsync(RegisterRequest request)
    // Kollar först i databasen om det redan finns en användare med den inskickade e-postadressen
    {
        var existingUser = await _authRepository.GetByEmailAsync(request.Email);
        // Om användaren redan finns, avbryts registreringen och metoden returnerar false
        if (existingUser != null)
        {
            return false;
        }
        // Krypterar (hashar) användarens lösenord med BCrypt så att det inte sparas i klartext i databasen
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
        //  Skapar upp ett nytt användarobjekt (UserEntity) med informationen från registreringsformuläret
        var newUser = new UserEntity
        {
            
            Email = request.Email,
            UserName = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            IsAccountConfirmed = true,

            // Sätter tomma eller standardvärden för Refresh Token vid registreringen
            RefreshToken = "",
            RefreshTokenCreated = DateTime.UtcNow,
            RefreshTokenExpires = DateTime.UtcNow
        };

        // Här sparas lösenordshashen i bakgrunden via Identity om man vill så, 
        // men eftersom vi kör BCrypt skickar vi med den till vårt repository:
        return await _authRepository.CreateUserAsync(newUser, request.Password);
    }

    // En asynkron metod som hanterar när en användare försöker logga in
    public async Task<AuthResponseDto?> LoginAsync(LoginRequest request)
    {
        // Letar efter användaren i databasen med hjälp av den inskickade e-postadressen
        var user = await _authRepository.GetByEmailAsync(request.Email);
        if (user == null)
        {
            return null;
        }
        // Skickar loginuppgifterna till databaslagret för att verifiera lösenordet och hämta grundsvaret
        var authResponse = await _authRepository.LoginAsync(request);
        {
            if (authResponse == null)
            {
                return null;
            }
        }
        // Genererar en helt ny, lång och slumpmässig Refresh Token via vår tokentjänst
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        // Sparar den nya Refresh Token på användarobjektet
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenCreated = DateTime.UtcNow;
        user.RefreshTokenExpires = DateTime.UtcNow.AddDays(7);
        // Uppdaterar användaren i databasen så att den nya Refresh Token sparas säkert
        await _authRepository.UpdateUserAsync(user);
        // Lägger till den nya Refresh Token i svaret som ska skickas tillbaka till frontend
        authResponse.RefreshToken = newRefreshToken;
        // Skapar en kortvarig digital åtkomstnyckel (JWT Access Token) som innehåller användarens uppgifter
        var newAccessToken = _tokenService.GenerateJwtToken(user);
        // Lägger till denna Access Token i svaret till frontend
        authResponse.AccessToken = newAccessToken;

        // Returnerar det kompletta paketet med tokens och användarinfo tillbaka till controllern
        return authResponse;
    }

    public async Task<bool> LogoutAsync(string email)
    {
        // Letar upp användaren i databasen baserat på e-postadressen som skickades med från frontend
        var user = await _authRepository.GetByEmailAsync(email);
        // Om användaren inte hittas kan vi inte logga ut personen, och returnerar false
        if (user == null) return false;

        // Vi tömmer användarens Refresh Token i databasen genom att sätta den till null
        user.RefreshToken = null;
        user.RefreshTokenCreated = DateTime.MinValue;
        user.RefreshTokenExpires = DateTime.MinValue;
        // Sparar de tömda värdena i Azure SQL-databasen via vårt repository
        await _authRepository.UpdateUserAsync(user);
        // Returnerar true för att bekräfta att utloggningen lyckades i databasen
        return true;
    }

    // En hjälpmetod för att hämta en användares profil via e-postadress
    public async Task<UserEntity?> GetByEmailAsync(string email)
    {
        // Ber databaslagret att hämta och returnera användaren med denna e-postadress
        return await _authRepository.GetByEmailAsync(email);
    }
}