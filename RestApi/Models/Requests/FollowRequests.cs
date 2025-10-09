using System.ComponentModel.DataAnnotations;
using Domain.Models;

namespace Cherish.RestApi.Models.Requests;

public class FollowUserRequest
{
    [Required]
    public bool IsFollowing { get; set; } = true;
}

public class FollowTeamRequest
{
    [Required]
    public bool IsFollowing { get; set; } = true;
}

public class FollowRequest
{
    [Required]
    [Range(1, 2, ErrorMessage = "Type must be 1 (User) or 2 (Team)")]
    public FollowType Type { get; set; }

    [Required]
    public Guid TargetId { get; set; }

    [Required]
    public bool IsFollowing { get; set; } = true;
}
