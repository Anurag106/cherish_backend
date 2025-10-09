using Domain.Models;
using Domain.Providers;
using Domain.Services;

namespace Cherish.RestApi.Services;

public class FollowService : IFollowService
{
    private readonly IFollowProvider _followProvider;
    private readonly ILogger<FollowService> _logger;

    public FollowService(IFollowProvider followProvider, ILogger<FollowService> logger)
    {
        _followProvider = followProvider;
        _logger = logger;
    }

    public async Task<FollowResponse?> FollowUserAsync(Guid companyId, Guid followerUserId, Guid followeeUserId, bool isFollowing = true)
    {
        try
        {
            // Validate users are in the same company
            if (!await _followProvider.IsUserInSameCompanyAsync(followerUserId, followeeUserId))
            {
                _logger.LogWarning("Users {FollowerId} and {FolloweeId} are not in the same company", followerUserId, followeeUserId);
                return null;
            }

            // Prevent self-following
            if (followerUserId == followeeUserId)
            {
                _logger.LogWarning("User {UserId} cannot follow themselves", followerUserId);
                return null;
            }

            var result = await _followProvider.CreateOrUpdateUserFollowUserAsync(companyId, followerUserId, followeeUserId, isFollowing);

            return new FollowResponse
            {
                Type = FollowType.User,
                TargetId = followeeUserId,
                IsFollowing = result.IsFollowed,
                LastModified = result.LastModified
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error following user {FolloweeId} by user {FollowerId}", followeeUserId, followerUserId);
            return null;
        }
    }

    public async Task<FollowResponse?> FollowTeamAsync(Guid companyId, Guid followerUserId, Guid followeeTeamId, bool isFollowing = true)
    {
        try
        {
            // Validate user and team are in the same company
            if (!await _followProvider.IsUserInCompanyAsync(followerUserId, companyId) ||
                !await _followProvider.IsTeamInCompanyAsync(followeeTeamId, companyId))
            {
                _logger.LogWarning("User {UserId} or team {TeamId} not in company {CompanyId}", followerUserId, followeeTeamId, companyId);
                return null;
            }

            // Prevent following own team
            if (await _followProvider.IsUserInTeamAsync(followerUserId, followeeTeamId))
            {
                _logger.LogWarning("User {UserId} cannot follow their own team {TeamId}", followerUserId, followeeTeamId);
                return null;
            }

            var result = await _followProvider.CreateOrUpdateUserFollowTeamAsync(companyId, followerUserId, followeeTeamId, isFollowing);

            return new FollowResponse
            {
                Type = FollowType.Team,
                TargetId = followeeTeamId,
                IsFollowing = result.IsFollowed,
                LastModified = result.LastModified
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error following team {TeamId} by user {UserId}", followeeTeamId, followerUserId);
            return null;
        }
    }

    public async Task<FollowResponse?> FollowAsync(Guid companyId, Guid followerUserId, FollowRequest request)
    {
        return request.Type switch
        {
            FollowType.User => await FollowUserAsync(companyId, followerUserId, request.TargetId, request.IsFollowing),
            FollowType.Team => await FollowTeamAsync(companyId, followerUserId, request.TargetId, request.IsFollowing),
            _ => null
        };
    }

    public async Task<FollowListResponse> GetFollowListAsync(Guid companyId, Guid userId)
    {
        try
        {
            var followingUsers = await GetUserFollowingUsersAsync(companyId, userId);
            var followingTeams = await GetUserFollowingTeamsAsync(companyId, userId);
            var followers = await GetUserFollowersAsync(companyId, userId);

            return new FollowListResponse
            {
                FollowingUsers = followingUsers,
                FollowingTeams = followingTeams,
                Followers = followers
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting follow list for user {UserId}", userId);
            return new FollowListResponse();
        }
    }

    public async Task<List<UserFollowInfo>> GetUserFollowingUsersAsync(Guid companyId, Guid followerUserId)
    {
        try
        {
            return await _followProvider.GetUserFollowingUsersAsync(companyId, followerUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting following users for user {UserId}", followerUserId);
            return new List<UserFollowInfo>();
        }
    }

    public async Task<List<UserFollowInfo>> GetUserFollowersAsync(Guid companyId, Guid followeeUserId)
    {
        try
        {
            return await _followProvider.GetUserFollowersAsync(companyId, followeeUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting followers for user {UserId}", followeeUserId);
            return new List<UserFollowInfo>();
        }
    }

    public async Task<List<TeamFollowInfo>> GetUserFollowingTeamsAsync(Guid companyId, Guid followerUserId)
    {
        try
        {
            return await _followProvider.GetUserFollowingTeamsAsync(companyId, followerUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting following teams for user {UserId}", followerUserId);
            return new List<TeamFollowInfo>();
        }
    }

    public async Task<List<UserFollowInfo>> GetTeamFollowersAsync(Guid companyId, Guid followeeTeamId)
    {
        try
        {
            return await _followProvider.GetTeamFollowersAsync(companyId, followeeTeamId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting followers for team {TeamId}", followeeTeamId);
            return new List<UserFollowInfo>();
        }
    }

    public async Task<bool> IsFollowingUserAsync(Guid companyId, Guid followerUserId, Guid followeeUserId)
    {
        try
        {
            var follow = await _followProvider.GetUserFollowUserAsync(companyId, followerUserId, followeeUserId);
            return follow?.IsFollowed ?? false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {FollowerId} follows user {FolloweeId}", followerUserId, followeeUserId);
            return false;
        }
    }

    public async Task<bool> IsFollowingTeamAsync(Guid companyId, Guid followerUserId, Guid followeeTeamId)
    {
        try
        {
            var follow = await _followProvider.GetUserFollowTeamAsync(companyId, followerUserId, followeeTeamId);
            return follow?.IsFollowed ?? false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {UserId} follows team {TeamId}", followerUserId, followeeTeamId);
            return false;
        }
    }
}
