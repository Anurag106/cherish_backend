using Domain.Models;

namespace Cherish.RestApi.Models.Responses;

public class FollowResponse
{
    public FollowType Type { get; set; }
    public Guid TargetId { get; set; }
    public bool IsFollowing { get; set; }
    public DateTime LastModified { get; set; }
}

public class FollowListResponse
{
    public List<UserFollowInfoResponse> FollowingUsers { get; set; } = new();
    public List<TeamFollowInfoResponse> FollowingTeams { get; set; } = new();
    public List<UserFollowInfoResponse> Followers { get; set; } = new();
}

public class UserFollowInfoResponse
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Department { get; set; }
    public DateTime LastModified { get; set; }
}

public class TeamFollowInfoResponse
{
    public Guid TeamId { get; set; }
    public string TeamName { get; set; } = "";
    public Guid ManagerId { get; set; }
    public string ManagerName { get; set; } = "";
    public int MemberCount { get; set; }
    public DateTime LastModified { get; set; }
}
