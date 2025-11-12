using Domain.Models;

namespace Domain.Providers;

public interface IUserProvider
{
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetUserByIdAsync(Guid id);
    Task<List<User>> GetUsersByCompanyIdAsync(Guid companyId);
    Task<User> CreateUserAsync(Guid userId, string email, string firstName, string lastName, Guid companyId);
    Task<bool> UpdateUserPointsAsync(Guid userId, int newTotalPoints, int newAvailablePoints);
    Task<bool> DeleteUserAsync(Guid userId);
    Task<List<User>> GetUsersAsync(Guid companyId, List<Guid>? userIds = null, EmployeeStatus? status = null, Guid? teamId = null, int pageNumber = 1, int pageSize = 20);
    Task<List<User>> GetUserAutocompleteAsync(Guid companyId, string searchTerm, int limit = 3);
    Task<List<User>> GetTeammatesAsync(Guid userId, Guid companyId);
    Task<User?> UpsertUserFromJwtAsync(Guid userId, string email, string firstName, string lastName, Guid companyId, string? department, UserMode userMode, EmployeeStatus employeeStatus);
}
