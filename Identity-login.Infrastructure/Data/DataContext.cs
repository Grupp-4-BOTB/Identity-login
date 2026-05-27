using Identity_login.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity_login.Infrastructure.Data
{
    // Vi ärver från IdentityDbContext och skickar med din UserEntity på .NETs inbyggda Identity-system
    public class DataContext : IdentityDbContext<UserEntity>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {


        }
    }
}