using Domain.Models;

namespace Domain.Services;

public interface IFollowService
{
    Task<FollowResponse?> FollowUserAsync(Guid companyId, Guid followerUserId, Guid followeeUserId, bool isFollowing = true);
    Task<FollowResponse?> FollowTeamAsync(Guid companyId, Guid followerUserId, Guid followeeTeamId, bool isFollowing = true);
    Task<FollowResponse?> FollowAsync(Guid companyId, Guid followerUserId, FollowRequest request);
    
    Task<FollowListResponse> GetFollowListAsync(Guid companyId, Guid userId);
    Task<List<UserFollowInfo>> GetUserFollowingUsersAsync(Guid companyId, Guid followerUserId);
    Task<List<UserFollowInfo>> GetUserFollowersAsync(Guid companyId, Guid followeeUserId);
    Task<List<TeamFollowInfo>> GetUserFollowingTeamsAsync(Guid companyId, Guid followerUserId);
    Task<List<UserFollowInfo>> GetTeamFollowersAsync(Guid companyId, Guid followeeTeamId);
    
    Task<bool> IsFollowingUserAsync(Guid companyId, Guid followerUserId, Guid followeeUserId);
    Task<bool> IsFollowingTeamAsync(Guid companyId, Guid followerUserId, Guid followeeTeamId);
}
