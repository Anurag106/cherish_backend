using Domain.Models;

namespace Domain.Services;

public interface IAuthService
{
    Task<string?> LoginAsync(string username, string password);
    Task<bool> UpdatePasswordAsync(string username, string currentPassword, string newPassword);
    Task<bool> LogoutAsync(string token);
    Task<bool> ValidateTokenAsync(string token);
    Task<User?> CreateUserAsync(string username, string password, Guid companyId);
}
