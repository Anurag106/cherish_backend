using Domain.Models;

namespace Domain.Services;

public interface IUserService
{
    Task<User?> GetUserByIdAsync(Guid id);
    Task<List<User>> GetUserAutocompleteAsync(Guid companyId, string searchTerm, int limit = 3);
    Task<List<User>> GetTeammatesAsync(Guid userId, Guid companyId);
}

