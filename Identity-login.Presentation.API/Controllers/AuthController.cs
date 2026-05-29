using System.Security.Principal;
using Identity_login.Application.DTOs;

using Identity_login.Application.Interfaces;
using Identity_login.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using RegisterRequest = Identity_login.Application.DTOs.RegisterRequest;

// AuthController fungerar som en dörrvakt för ditt API som tar emot och hanterar
// alla inkommande HTTP-anrop för inloggning, registrering och utloggning från din React-frontend.
// Dess uppgift är att kontrollera att den inskickade datan är giltig och skicka den vidare till AuthService som sköter själva logiken.

// Anger grund-URL:en för alla endpoints i denna fil.
namespace Identity_login.Presentation.API.Controllers;
// Bestämmer routingen för controllern; [controller] byts automatiskt ut mot "Auth" i URL-adressen (api/Auth)
[Route("api/[controller]")]
// Talar om för .NET att detta är en API-controller, vilket aktiverar automatisk validering av inkommande data (t.ex. Bad Request 400)
[ApiController]
// Deklarerar klassen AuthController som ärver från ControllerBase (standard för API:er utan HTML-vyer)
public class AuthController : ControllerBase //Den ingår i Microsofts officiella webb-bibliotek och ligger i följande namnrymd
{// Skapar en privat variabel för att hålla reda på vår autentiseringstjänst (affärslogiken)
    private readonly AuthService _authService;

    // Konstruktor för Dependency Injection: Här injiceras AuthService så att controllern kan använda dess funktioner
    public AuthController(AuthService authService)
    {
        // Sparar den injicerade tjänsten i vår privata variabel så att alla endpoints i klassen når den
        _authService = authService;
    }

    // ENDPOINT 1: Registrering. Lyssnar på en HTTP POST-request på URL:en /api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] Identity_login.Application.DTOs.RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.RegisterAsync(request);

        if (!result)
            return BadRequest(new { message = "Email is already registered or registration failed." });

        return Ok(new { message = "User registered successfully!" });
    }



    // ENDPOINT: Kontrollera om e-postadressen redan finns i Azure SQL
    [HttpPost("check-email")]
    public async Task<IActionResult> CheckEmail([FromBody] EmailCheckRequest request)
    {
        // Vi söker efter användaren i databasen med hjälp av e-postadressen
        var user = await _authService.GetByEmailAsync(request.Email);

        if (user != null)
        {
            // Användaren finns -> Skicka vidare till LOGIN (begär lösenord)
            return Ok(new { exists = true });
        }

        // Användaren finns INTE -> Skicka vidare till REGISTER (Almost There)
        return Ok(new { exists = false });
    }

    // En enkel DTO-klass för att ta emot e-postadressen från frontend
    public class EmailCheckRequest
    {
        public string Email { get; set; } = null!;
    }








    // ENDPOINT 2: Inloggning. Lyssnar på en HTTP POST-request på URL:en /api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Identity_login.Application.DTOs.LoginRequest loginRequest)
    {
        // Vi anropar den uppdaterade tjänsten
        var tokenResult = await _authService.LoginAsync(loginRequest);

        // Om lösenordet var fel eller användaren inte fanns
        if (tokenResult == null)
        {
            return Unauthorized(new { Meddelande = "Fel användarnamn eller lösenord" });
        }

        // HÄR SKICKAS Tokens tillbaka till klienten som en del av svaret.
        return Ok(new
        {
            Message = "Inloggning lyckades!",
            AccessToken = tokenResult.AccessToken,
            RefreshToken = tokenResult.RefreshToken
        });
    }





    // ENDPOINT 3: Utloggning. Lyssnar på en HTTP POST-request på URL:en /api/auth/logout
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
    {
       //  Skickar användarens e-post till vår service för att hantera utloggningslogiken
        var result = await _authService.LogoutAsync(request.Email);

       if (!result)
            return BadRequest(new { message = "Logout failed." });

       return Ok(new { message = "Logged out successfully!" });

   }
}