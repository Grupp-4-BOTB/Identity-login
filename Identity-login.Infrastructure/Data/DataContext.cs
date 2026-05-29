using Identity_login.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


// DataContext är bryggan mellan min C#-kod och din Azure SQL-databas. Den använder ett verktyg som
// heter Entity Framework Core (EF Core) för ałł översätta C#-objekt till SQL-tabeller och vice versa.



namespace Identity_login.Infrastructure.Data
{
    // Vi ärver från IdentityDbContext och skickar med min UserEntity på .NETs inbyggda Identity-system
    // Deklarerar en offentlig klass som heter DataContext.
    // Den ärver från IdentityDbContext och vi skickar med vår UserEntity för att berätta
    // att systemet ska använda vår skräddarsydda användarmodell
    public class DataContext : IdentityDbContext<UserEntity>

    // Konstruktor: Tar emot inställningar (som till exempel vår Connection String från appsettings.json/User Secrets) 
    // via parametern options
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

            // Kolonet och ": base(options)" betyder att vi skickar vidare alla dessa inställningar 
            // till modersklassen (IdentityDbContext) så att den kan starta upp databaskopplingen på rätt sätt.
        }
    }
}