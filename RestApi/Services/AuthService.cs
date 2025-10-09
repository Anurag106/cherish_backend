using Domain.Services;
using Domain.Providers;
using Domain.Models;

namespace Cherish.RestApi.Services;

public class AuthService : IAuthService
{
    private readonly IUserProvider _userProvider;
    private readonly ITokenService _tokenService;
    private readonly HashSet<string> _activeTokens = new();

    public AuthService(IUserProvider userProvider, ITokenService tokenService)
    {
        _userProvider = userProvider;
        _tokenService = tokenService;
    }

    public async Task<string?> LoginAsync(string username, string password)
    {
        var user = await _userProvider.GetUserByUsernameAsync(username);
        
        if (user == null || user.Password != password)
        {
            return null;
        }

        var token = _tokenService.GenerateToken(user);
        _activeTokens.Add(token);
        
        return token;
    }

    public async Task<bool> UpdatePasswordAsync(string username, string currentPassword, string newPassword)
    {
        var user = await _userProvider.GetUserByUsernameAsync(username);
        
        if (user == null || user.Password != currentPassword)
        {
            return false;
        }

        return await _userProvider.UpdateUserPasswordAsync(username, newPassword);
    }

    public async Task<bool> LogoutAsync(string token)
    {
        if (_activeTokens.Contains(token))
        {
            _activeTokens.Remove(token);
            return true;
        }
        
        return false;
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        return _tokenService.ValidateToken(token) && _activeTokens.Contains(token);
    }

    public async Task<User?> CreateUserAsync(string username, string password, Guid companyId)
    {
        // Check if user already exists
        var existingUser = await _userProvider.GetUserByUsernameAsync(username);
        if (existingUser != null)
        {
            return null; // User already exists
        }

        // Create new user
        return await _userProvider.CreateUserAsync(username, password, companyId);
    }
}
