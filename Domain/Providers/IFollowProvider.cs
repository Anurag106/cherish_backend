using Domain.Models;

namespace Domain.Providers;

public interface IFollowProvider
{
    // User-to-User Follow Operations
    Task<UserFollowUser?> GetUserFollowUserAsync(Guid companyId, Guid followerUserId, Guid followeeUserId);
    Task<UserFollowUser> CreateOrUpdateUserFollowUserAsync(Guid companyId, Guid followerUserId, Guid followeeUserId, bool isFollowing);
    Task<List<UserFollowInfo>> GetUserFollowingUsersAsync(Guid companyId, Guid followerUserId);
    Task<List<UserFollowInfo>> GetUserFollowersAsync(Guid companyId, Guid followeeUserId);

    // User-to-Team Follow Operations
    Task<UserFollowTeam?> GetUserFollowTeamAsync(Guid companyId, Guid followerUserId, Guid followeeTeamId);
    Task<UserFollowTeam> CreateOrUpdateUserFollowTeamAsync(Guid companyId, Guid followerUserId, Guid followeeTeamId, bool isFollowing);
    Task<List<TeamFollowInfo>> GetUserFollowingTeamsAsync(Guid companyId, Guid followerUserId);
    Task<List<UserFollowInfo>> GetTeamFollowersAsync(Guid companyId, Guid followeeTeamId);

    // Validation Methods
    Task<bool> IsUserInSameCompanyAsync(Guid userId1, Guid userId2);
    Task<bool> IsUserInCompanyAsync(Guid userId, Guid companyId);
    Task<bool> IsTeamInCompanyAsync(Guid teamId, Guid companyId);
    Task<bool> IsUserInTeamAsync(Guid userId, Guid teamId);
}
