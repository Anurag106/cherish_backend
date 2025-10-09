using Domain.Models;

namespace Domain.Services;

public interface ITokenService
{
    string GenerateToken(User user);
    bool ValidateToken(string token);
    string? GetUsernameFromToken(string token);
    Guid? GetUserIdFromToken(string token);
    Guid? GetCompanyIdFromToken(string token);
    Guid? GetTeamIdFromToken(string token);
    UserRole? GetUserRoleFromToken(string token);
    UserStatus? GetUserStatusFromToken(string token);
}
