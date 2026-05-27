using Microsoft.AspNetCore.Identity;

namespace Identity_login.Domain.Entities
{
    public class UserEntity : IdentityUser
    {
        // Förnamn från ditt Shiko-formulär
        public string FirstName { get; set; } = string.Empty;

        // Efternamn från ditt Shiko-formulär
        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        // Status för att hålla koll på om användaren har slutfört "Almost There"-sidan
        public bool IsAccountConfirmed { get; set; } = false;


    }
}