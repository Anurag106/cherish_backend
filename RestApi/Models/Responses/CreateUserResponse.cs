using Domain.Models;

namespace Cherish.RestApi.Models.Responses;

public class CreateUserResponse
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public UserStatus Status { get; set; }
    public Guid? TeamId { get; set; }
    public string Department { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public DateTime? DateHired { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int TotalPoints { get; set; }
    public int AvailablePoints { get; set; }
    public DateTime CreatedAt { get; set; }
}
