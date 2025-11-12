using Domain.Models;

namespace Cherish.RestApi.Models.Responses;

public class UserProfileResponse
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PreferredFirstName { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public UserMode UserMode { get; set; }
    public EmployeeStatus EmployeeStatus { get; set; }
    public Guid? TeamId { get; set; }
    public string? Department { get; set; }
    public string? JobTitle { get; set; }
    public DateTime? DateHired { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int TotalPoints { get; set; }
    public int AvailablePoints { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class UserListResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public UserMode UserMode { get; set; }
    public EmployeeStatus EmployeeStatus { get; set; }
    public Guid? TeamId { get; set; }
    public string? Department { get; set; }
    public string? JobTitle { get; set; }
    public int TotalPoints { get; set; }
    public int AvailablePoints { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UserMentionResponse
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Department { get; set; }
}