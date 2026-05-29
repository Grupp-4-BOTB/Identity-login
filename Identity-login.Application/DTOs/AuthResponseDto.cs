
// AuthResponseDto är et datapaket (ett kvitto) som servern skickar tillbaka till frontend efter att
// en användare har loggat in eller registrerat sig. I stället for att skicka hela databasmodellen
// (som innehåller känslig information), skickar vi bara exakt det som Next.js behöver för att hantera sessionen.

// vi bara skickar exakt den data som frontendet faktiskt behöver, vilket sparar bandbredd
// och ökar säkerheten eftersom vi aldrig exponerar känslig databasinformation som lösenordshashar
namespace Identity_login.Application.DTOs
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        // Användarens e-postadress, så att frontendet kan spara den i localStorage och visa vem som är inloggad
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsAccountConfirmed { get; set; }
    }
}