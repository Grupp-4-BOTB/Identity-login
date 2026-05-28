using BCrypt;
using Identity_login.Application.DTOs;
using Identity_login.Application.Interfaces;
using Identity_login.Domain.Entities;

using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Identity_login.Application.Services;

public class AuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly ITokenService _tokenService;

    public AuthService(IAuthRepository authRepository, ITokenService tokenService)
    {
        _authRepository = authRepository;
        _tokenService = tokenService;
        
    }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _authRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return false;
        }

        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var newUser = new UserEntity
        {
            
            Email = request.Email,
            UserName = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            IsAccountConfirmed = false
        };

        // Här sparas lösenordshashen i bakgrunden via Identity om man vill så, 
        // men eftersom vi kör BCrypt skickar vi med den till vårt repository:
        return await _authRepository.CreateUserAsync(newUser, request.Password);
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginRequest request)
    {
        var user = await _authRepository.GetByEmailAsync(request.Email);
        if (user == null)
        {
            return null;
        }

        var authResponse = await _authRepository.LoginAsync(request);
        {
            if (authResponse == null)
            {
                return null;
            }
        }

        var newRefreshToken = _tokenService.GenerateRefreshToken();


        user.RefreshToken = newRefreshToken;
        user.RefreshTokenCreated = DateTime.UtcNow;
        user.RefreshTokenExpires = DateTime.UtcNow.AddDays(7);

        await _authRepository.UpdateUserAsync(user);
        authResponse.RefreshToken = newRefreshToken;

        var newAccessToken = _tokenService.GenerateJwtToken(user);
        authResponse.AccessToken = newAccessToken;


        return authResponse;
    }

    public async Task<bool> LogoutAsync(string email)
    {
        return true;
    }
}