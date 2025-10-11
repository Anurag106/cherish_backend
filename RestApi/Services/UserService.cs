using Domain.Services;
using Domain.Providers;
using Domain.Models;

namespace Cherish.RestApi.Services;

public class UserService : IUserService
{
    private readonly IUserProvider _userProvider;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserProvider userProvider, ILogger<UserService> logger)
    {
        _userProvider = userProvider;
        _logger = logger;
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Attempted to get user with empty GUID");
                return null;
            }

            return await _userProvider.GetUserByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with ID: {Id}", id);
            throw;
        }
    }

    public async Task<List<User>> GetUserAutocompleteAsync(Guid companyId, string searchTerm, int limit = 3)
    {
        try
        {
            if (companyId == Guid.Empty)
            {
                _logger.LogWarning("Attempted to search users with empty company GUID");
                return new List<User>();
            }

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                _logger.LogWarning("Attempted to search users with empty search term");
                return new List<User>();
            }

            return await _userProvider.GetUserAutocompleteAsync(companyId, searchTerm, limit);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users for company {CompanyId} with term: {SearchTerm}", companyId, searchTerm);
            throw;
        }
    }

    public async Task<List<User>> GetTeammatesAsync(Guid userId, Guid companyId)
    {
        try
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("Attempted to get teammates with empty user GUID");
                return new List<User>();
            }

            if (companyId == Guid.Empty)
            {
                _logger.LogWarning("Attempted to get teammates with empty company GUID");
                return new List<User>();
            }

            return await _userProvider.GetTeammatesAsync(userId, companyId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving teammates for user {UserId} in company {CompanyId}", userId, companyId);
            throw;
        }
    }
}

