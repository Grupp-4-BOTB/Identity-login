// Inkluderar Data Transfer Objects (DTOs), vilket behövs för att använda AuthResponseDto
using Identity_login.Application.DTOs;
// Inkluderar domänentiteter, vilket behövs för att applikationen ska känna till UserEntity
using Identity_login.Domain.Entities;


// Definierar ett unikt namnrymd (namespace) för att organisera projektets interface strukturellt
namespace Identity_login.Application.Interfaces
{
    // Deklarerar ett offentligt (public) gränssnitt som heter IAuthRepository
    public interface IAuthRepository
    {
        // En metod för inloggning som tar emot loginuppgifter (LoginRequest) 
        // och returnerar ett asynkront svar (Task) som innehåller token-info (AuthResponseDto) eller null
        Task<AuthResponseDto?> LoginAsync(LoginRequest request);


        // En metod för att hämta en specifik användare från databasen baserat på deras e-postadress.
        // Returnerar en UserEntity eller null om användaren inte hittas
        Task<UserEntity?> GetByEmailAsync(string email);

        // En metod för att registrera en ny användare i databasen med ett lösenord.
        // Returnerar true om skapandet lyckades, annars false
        Task<bool> CreateUserAsync(UserEntity user, string password);
        // En metod för att uppdatera en befintlig användares data i databasen (t.ex. när vi ändrar eller tömmer RefreshToken).
        // Returnerar en tom asynkron Task när operationen är klar
        Task UpdateUserAsync(UserEntity user);


    }
}