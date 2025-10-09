using Domain.Services;
using Domain.Providers;
using Domain.Models;

namespace Cherish.RestApi.Services;

public class HashtagService : IHashtagService
{
    private readonly IHashtagProvider _hashtagProvider;
    private readonly IUserProvider _userProvider;
    private readonly ICompanyProvider _companyProvider;
    private readonly ILogger<HashtagService> _logger;

    public HashtagService(
        IHashtagProvider hashtagProvider,
        IUserProvider userProvider,
        ICompanyProvider companyProvider,
        ILogger<HashtagService> logger)
    {
        _hashtagProvider = hashtagProvider;
        _userProvider = userProvider;
        _companyProvider = companyProvider;
        _logger = logger;
    }

    public async Task<Hashtag?> GetHashtagByIdAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                _logger.LogWarning("Attempted to get hashtag with invalid ID: {Id}", id);
                return null;
            }

            return await _hashtagProvider.GetHashtagByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving hashtag with ID: {Id}", id);
            throw;
        }
    }

    public async Task<Hashtag?> GetHashtagByNameAsync(string name, Guid companyId)
    {
        try
        {
            if (string.IsNullOrEmpty(name))
            {
                _logger.LogWarning("Attempted to get hashtag with empty name");
                return null;
            }

            if (companyId == Guid.Empty)
            {
                _logger.LogWarning("Attempted to get hashtag with empty company GUID");
                return null;
            }

            return await _hashtagProvider.GetHashtagByNameAsync(name, companyId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving hashtag with name: {Name} for company: {CompanyId}", name, companyId);
            throw;
        }
    }

    public async Task<List<Hashtag>> GetAllHashtagsAsync()
    {
        try
        {
            return await _hashtagProvider.GetAllHashtagsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all hashtags");
            throw;
        }
    }

    public async Task<List<Hashtag>> GetHashtagsByCompanyIdAsync(Guid companyId)
    {
        try
        {
            if (companyId == Guid.Empty)
            {
                _logger.LogWarning("Attempted to get hashtags with empty company GUID");
                return new List<Hashtag>();
            }

            // Verify company exists
            var company = await _companyProvider.GetCompanyByIdAsync(companyId);
            if (company == null)
            {
                _logger.LogWarning("Attempted to get hashtags for non-existent company: {CompanyId}", companyId);
                return new List<Hashtag>();
            }

            return await _hashtagProvider.GetHashtagsByCompanyIdAsync(companyId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving hashtags for company: {CompanyId}", companyId);
            throw;
        }
    }

    public async Task<List<Hashtag>> GetHashtagsByCreatedByAsync(Guid createdBy)
    {
        try
        {
            if (createdBy == Guid.Empty)
            {
                _logger.LogWarning("Attempted to get hashtags with empty created by GUID");
                return new List<Hashtag>();
            }

            // Verify user exists (we'll need a method to get user by ID)
            // For now, we'll assume the user exists

            return await _hashtagProvider.GetHashtagsByCreatedByAsync(createdBy);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving hashtags for user: {CreatedBy}", createdBy);
            throw;
        }
    }

    public async Task<Hashtag?> CreateHashtagAsync(string name, string description, Guid companyId, Guid createdBy)
    {
        try
        {
            if (string.IsNullOrEmpty(name))
            {
                _logger.LogWarning("Attempted to create hashtag with empty name");
                return null;
            }

            if (companyId == Guid.Empty)
            {
                _logger.LogWarning("Attempted to create hashtag with empty company GUID");
                return null;
            }

            if (createdBy == Guid.Empty)
            {
                _logger.LogWarning("Attempted to create hashtag with empty created by GUID");
                return null;
            }

            // Check if hashtag with this name already exists in the company
            var existingHashtag = await _hashtagProvider.GetHashtagByNameAsync(name, companyId);
            if (existingHashtag != null)
            {
                _logger.LogWarning("Attempted to create hashtag with existing name: {Name} in company: {CompanyId}", name, companyId);
                return null;
            }

            // Verify company exists
            var company = await _companyProvider.GetCompanyByIdAsync(companyId);
            if (company == null)
            {
                _logger.LogWarning("Attempted to create hashtag for non-existent company: {CompanyId}", companyId);
                return null;
            }

            return await _hashtagProvider.CreateHashtagAsync(name, description, companyId, createdBy);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating hashtag with name: {Name} for company: {CompanyId}", name, companyId);
            throw;
        }
    }

    public async Task<Hashtag?> UpdateHashtagAsync(int id, string name, string description, Guid modifiedBy)
    {
        try
        {
            if (id <= 0)
            {
                _logger.LogWarning("Attempted to update hashtag with invalid ID: {Id}", id);
                return null;
            }

            if (string.IsNullOrEmpty(name))
            {
                _logger.LogWarning("Attempted to update hashtag with empty name");
                return null;
            }

            if (modifiedBy == Guid.Empty)
            {
                _logger.LogWarning("Attempted to update hashtag with empty modified by GUID");
                return null;
            }

            // Check if hashtag exists
            var existingHashtag = await _hashtagProvider.GetHashtagByIdAsync(id);
            if (existingHashtag == null)
            {
                _logger.LogWarning("Attempted to update non-existent hashtag with ID: {Id}", id);
                return null;
            }

            // Check if another hashtag with the same name exists in the same company
            var hashtagWithSameName = await _hashtagProvider.GetHashtagByNameAsync(name, existingHashtag.CompanyId);
            if (hashtagWithSameName != null && hashtagWithSameName.Id != id)
            {
                _logger.LogWarning("Attempted to update hashtag to existing name: {Name}", name);
                return null;
            }

            return await _hashtagProvider.UpdateHashtagAsync(id, name, description, modifiedBy);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating hashtag with ID: {Id} and name: {Name}", id, name);
            throw;
        }
    }

    public async Task<bool> DeleteHashtagAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                _logger.LogWarning("Attempted to delete hashtag with invalid ID: {Id}", id);
                return false;
            }

            // Check if hashtag exists
            var existingHashtag = await _hashtagProvider.GetHashtagByIdAsync(id);
            if (existingHashtag == null)
            {
                _logger.LogWarning("Attempted to delete non-existent hashtag with ID: {Id}", id);
                return false;
            }

            return await _hashtagProvider.DeleteHashtagAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting hashtag with ID: {Id}", id);
            throw;
        }
    }
}
