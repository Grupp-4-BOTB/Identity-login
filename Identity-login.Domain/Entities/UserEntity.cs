using Microsoft.AspNetCore.Identity;

namespace Identity_login.Domain.Entities
{
    public class UserEntity : IdentityUser
    {
        // Förnamn från ditt Shiko-formulär
        public string FirstName { get; set; } = string.Empty;

        // Efternamn från ditt Shiko-formulär
        public string LastName { get; set; } = string.Empty;

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenCreated { get; set; }
        public DateTime? RefreshTokenExpires { get; set; }



        // Status för att hålla koll på om användaren har slutfört "Almost There"-sidan
        public bool IsAccountConfirmed { get; set; } = false;


    }
}