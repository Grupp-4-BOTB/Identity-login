using Identity_login.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity_login.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateJwtToken(UserEntity user);

        string GenerateRefreshToken();
    }
}
