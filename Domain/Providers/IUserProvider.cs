using Domain.Models;

namespace Domain.Providers;

public interface IUserProvider
{
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User?> GetUserByIdAsync(Guid id);
    Task<List<User>> GetUsersByCompanyIdAsync(Guid companyId);
    Task<User> CreateUserAsync(string username, string password, Guid companyId);
    Task<bool> UpdateUserPasswordAsync(string username, string newPassword);
    Task<bool> DeleteUserAsync(string username);
    Task<bool> UpdateUserPointsAsync(Guid userId, int newTotalPoints, int newAvailablePoints);
    Task<List<User>> GetUsersAsync(Guid companyId, List<Guid>? userIds = null, UserStatus? status = null, Guid? teamId = null, UserRole? role = null, int pageNumber = 1, int pageSize = 20);
}
