using AuthWebAPI.Models;
using AuthWebAPI.Entities;

namespace AuthWebAPI.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserDto request);
        Task<TokenResponseDto?> LoginAsync(UserDto request);
        Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenDto request);

    }
}