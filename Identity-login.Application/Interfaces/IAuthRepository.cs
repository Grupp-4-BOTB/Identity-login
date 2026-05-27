using Identity_login.Application.DTOs;
using Identity_login.Domain.Entities;

namespace Identity_login.Application.Interfaces
{
    public interface IAuthRepository
    {
        
        Task<AuthResponseDto?> LoginAsync(LoginRequest request);
        Task<UserEntity?> GetByEmailAsync(string email);
        Task<bool> CreateUserAsync(UserEntity user);
        
    }
}