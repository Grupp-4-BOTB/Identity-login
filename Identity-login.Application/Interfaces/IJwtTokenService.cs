
using System.Security.Claims;
using System.Text;
using Identity_login.Application.Interfaces;
using Identity_login.Domain.Entities;








namespace Identity_login.Application.Interfaces
{
    public interface IJwtTokenService
    {
      
        string CreateToken(UserEntity user);
    }
}