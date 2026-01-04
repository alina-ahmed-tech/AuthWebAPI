using AuthWebAPI.Models;
using AuthWebAPI.Entities;

namespace AuthWebAPI.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserDto request);
        Task<string?> LoginAsync(UserDto request);

    }
}