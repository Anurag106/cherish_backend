namespace Domain.Models;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Employee;
    public UserStatus Status { get; set; } = UserStatus.Active;
    public Guid? TeamId { get; set; }
    public string Department { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public DateTime? DateHired { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int TotalPoints { get; set; } = 0;
    public int AvailablePoints { get; set; } = 0;
    public Guid CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public enum UserRole
{
    Employee = 0,
    Manager = 1,
    Admin = 2
}

public enum UserStatus
{
    Active = 0,
    Inactive = 1,
    Suspended = 2,
    Terminated = 3
}
