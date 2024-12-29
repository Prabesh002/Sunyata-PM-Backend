using Sunyata_PM_Backend.Models;

namespace Sunyata_PM_Backend.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> GenerateTokenAsync(User user);
        Task<User> AuthenticateUserAsync(string username, string password);
        Task<User> RegisterUserAsync(string username, string password);
    }
}
