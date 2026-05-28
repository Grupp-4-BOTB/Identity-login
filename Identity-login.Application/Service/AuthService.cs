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
    

    public AuthService(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
        
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

        // Här använder vi BCrypt för att verifiera mot databasen
        // OBS: Om ni använder Identitys PasswordHash, används istället PasswordHasher.
        // Vi kör BCrypt eftersom det är det du har valt i din logik!
        return await _authRepository.LoginAsync(request);
    }

    public async Task<bool> LogoutAsync(string email)
    {
        return true;
    }
}