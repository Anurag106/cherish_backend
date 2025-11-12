using System.Security.Claims;

namespace Cherish.RestApi.Services;

/// <summary>
/// Service to extract user information from JWT token claims.
/// This service trusts tokens issued by yogi_code/backend and extracts user context.
/// </summary>
public interface IUserContextService
{
    string? GetUserId();
    string? GetEmail();
    Guid? GetTenantId();
    Guid? GetEmployeeId();
    string? GetDepartment();
    string? GetUserMode();
    string? GetEmployeeStatus();
    List<string> GetRoles();
    bool IsAdmin();
    bool IsSuperAdmin();
}

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public string? GetUserId()
    {
        return User?.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? User?.FindFirst("sub")?.Value;
    }

    public string? GetEmail()
    {
        return User?.FindFirst(ClaimTypes.Email)?.Value
            ?? User?.FindFirst("email")?.Value;
    }

    public Guid? GetTenantId()
    {
        var tenantIdClaim = User?.FindFirst("tenantId")?.Value;
        return tenantIdClaim != null && Guid.TryParse(tenantIdClaim, out var tenantId) 
            ? tenantId 
            : null;
    }

    public Guid? GetEmployeeId()
    {
        var employeeIdClaim = User?.FindFirst("employeeId")?.Value;
        return employeeIdClaim != null && Guid.TryParse(employeeIdClaim, out var employeeId) 
            ? employeeId 
            : null;
    }

    public string? GetDepartment()
    {
        return User?.FindFirst("department")?.Value;
    }

    public string? GetUserMode()
    {
        return User?.FindFirst("userMode")?.Value;
    }

    public string? GetEmployeeStatus()
    {
        return User?.FindFirst("employeeStatus")?.Value;
    }

    public List<string> GetRoles()
    {
        return User?.FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList() ?? new List<string>();
    }

    public bool IsAdmin()
    {
        var isAdminClaim = User?.FindFirst("isAdmin")?.Value;
        return isAdminClaim == "true";
    }

    public bool IsSuperAdmin()
    {
        var isSuperAdminClaim = User?.FindFirst("isSuperAdmin")?.Value;
        return isSuperAdminClaim == "true";
    }
}

