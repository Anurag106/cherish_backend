namespace Domain.Models;

/// <summary>
/// User reference model for social features.
/// 🔒 CRITICAL: This is NOT used for authentication! Authentication is handled by yogi_code/backend.
/// 📝 PURPOSE: Cache user info for social features (posts, comments, reactions).
/// 🔄 SYNC: User data is synced from yogi_code/backend via JWT claims or API calls.
/// 🆔 ID TYPE: String (matches ASP.NET Identity user IDs from yogi_code/backend).
/// </summary>
public class User
{
    /// <summary>
    /// User ID - Guid for now (TODO: migrate to string to match ASP.NET Identity)
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// User email - synced from JWT or API
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// First name - synced from JWT
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Last name - synced from JWT
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// Preferred first name (from Employee record)
    /// </summary>
    public string? PreferredFirstName { get; set; }
    
    /// <summary>
    /// Profile picture URL
    /// </summary>
    public string? ProfilePictureUrl { get; set; }
    
    /// <summary>
    /// Whether user is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// User mode: 0=Normal, 1=Benefactor, 2=Receiver, 3=Observer
    /// Maps to Employee.UserMode from yogi_code/backend
    /// </summary>
    public UserMode UserMode { get; set; } = UserMode.Normal;
    
    /// <summary>
    /// Employee status: 0=Active, 1=Inactive, 2=OnLeave, 3=Terminated, etc.
    /// Maps to Employee.Status from yogi_code/backend
    /// </summary>
    public EmployeeStatus EmployeeStatus { get; set; } = EmployeeStatus.Active;
    
    /// <summary>
    /// Team ID for team-based features (nullable)
    /// </summary>
    public Guid? TeamId { get; set; }
    
    /// <summary>
    /// Department - synced from JWT or Employee record
    /// </summary>
    public string? Department { get; set; }
    
    /// <summary>
    /// Job title
    /// </summary>
    public string? JobTitle { get; set; }
    
    /// <summary>
    /// Date hired
    /// </summary>
    public DateTime? DateHired { get; set; }
    
    /// <summary>
    /// Date of birth
    /// </summary>
    public DateTime? DateOfBirth { get; set; }
    
    /// <summary>
    /// Total points earned (for display only - source of truth is yogi_code/backend)
    /// </summary>
    public int TotalPoints { get; set; } = 0;
    
    /// <summary>
    /// Available points (for display only - source of truth is yogi_code/backend)
    /// </summary>
    public int AvailablePoints { get; set; } = 0;
    
    /// <summary>
    /// Company/Tenant ID - maps to Tenants.Id from yogi_code/backend
    /// </summary>
    public Guid CompanyId { get; set; }
    
    /// <summary>
    /// Company name (cached for display)
    /// </summary>
    public string CompanyName { get; set; } = string.Empty;
    
    /// <summary>
    /// When this record was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// When this record was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
    
    /// <summary>
    /// Last time user data was synced from yogi_code/backend
    /// </summary>
    public DateTime? LastSyncedAt { get; set; }
    
    /// <summary>
    /// Source of last sync: 'jwt_claims', 'api_sync', 'manual'
    /// </summary>
    public string SyncSource { get; set; } = "jwt_claims";
    
    // Computed properties
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string DisplayName => PreferredFirstName ?? FirstName;
}

/// <summary>
/// User mode - matches Employee.UserMode from yogi_code/backend
/// </summary>
public enum UserMode
{
    Normal = 0,
    Benefactor = 1,
    Receiver = 2,
    Observer = 3
}

/// <summary>
/// Employee status - matches Employee.Status from yogi_code/backend
/// </summary>
public enum EmployeeStatus
{
    Active = 0,
    Inactive = 1,
    OnLeave = 2,
    Terminated = 3,
    Retired = 4,
    PendingActivation = 5
}

// Legacy enums removed - use JWT roles and EmployeeStatus instead
// UserRole enum removed - roles come from JWT claims
// UserStatus enum replaced by EmployeeStatus
