using Identity_login.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

//ITokenService är ett interface (gränssnitt). Det fungerar som ett kontrakt eller en ritning.
//Det innehåller ingen logik eller kodblock, utan bestämmer bara vilka metoder
//som måste finnas i applikationen föratt hantera tokens.
namespace Identity_login.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateJwtToken(UserEntity user);

        string GenerateRefreshToken();
    }
}
