namespace Domain.Models;

public enum FollowType
{
    User = 1,
    Team = 2
}

public class UserFollowUser
{
    public Guid CompanyId { get; set; }
    public Guid FollowerUserId { get; set; }
    public Guid FolloweeUserId { get; set; }
    public DateTime LastModified { get; set; }
    public bool IsFollowed { get; set; }
}

public class UserFollowTeam
{
    public Guid CompanyId { get; set; }
    public Guid FollowerUserId { get; set; }
    public Guid FolloweeTeamId { get; set; }
    public DateTime LastModified { get; set; }
    public bool IsFollowed { get; set; }
}

public class FollowRequest
{
    public FollowType Type { get; set; }
    public Guid TargetId { get; set; } // User ID or Team ID
    public bool IsFollowing { get; set; } = true; // true to follow, false to unfollow
}

public class FollowResponse
{
    public FollowType Type { get; set; }
    public Guid TargetId { get; set; }
    public bool IsFollowing { get; set; }
    public DateTime LastModified { get; set; }
}

public class FollowListResponse
{
    public List<UserFollowInfo> FollowingUsers { get; set; } = new();
    public List<TeamFollowInfo> FollowingTeams { get; set; } = new();
    public List<UserFollowInfo> Followers { get; set; } = new();
}

public class UserFollowInfo
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Department { get; set; }
    public DateTime LastModified { get; set; }
}

public class TeamFollowInfo
{
    public Guid TeamId { get; set; }
    public string TeamName { get; set; } = "";
    public Guid ManagerId { get; set; }
    public string ManagerName { get; set; } = "";
    public int MemberCount { get; set; }
    public DateTime LastModified { get; set; }
}
