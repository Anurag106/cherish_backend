using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cherish.RestApi.Controllers;

/// <summary>
/// Base controller for all API controllers in backendv4.
/// Provides helper methods to extract user information from JWT claims.
/// JWT tokens are issued and validated by yogi_code/backend.
/// </summary>
[ApiController]
[Authorize]  // All endpoints require valid JWT token by default
public abstract class BaseApiController : ControllerBase
{
    /// <summary>
    /// Get user ID from JWT claims (sub claim)
    /// </summary>
    protected string? GetCurrentUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? User.FindFirst("sub")?.Value;
    }

    /// <summary>
    /// Get user email from JWT claims
    /// </summary>
    protected string? GetCurrentUserEmail()
    {
        return User.FindFirst(ClaimTypes.Email)?.Value 
            ?? User.FindFirst("email")?.Value;
    }

    /// <summary>
    /// Get tenant/company ID from JWT claims
    /// </summary>
    protected Guid? GetCurrentTenantId()
    {
        var tenantIdClaim = User.FindFirst("tenantId")?.Value;
        return tenantIdClaim != null && Guid.TryParse(tenantIdClaim, out var tenantId) 
            ? tenantId 
            : null;
    }

    /// <summary>
    /// Get employee ID from JWT claims
    /// </summary>
    protected Guid? GetCurrentEmployeeId()
    {
        var employeeIdClaim = User.FindFirst("employeeId")?.Value;
        return employeeIdClaim != null && Guid.TryParse(employeeIdClaim, out var employeeId) 
            ? employeeId 
            : null;
    }

    /// <summary>
    /// Get department from JWT claims
    /// </summary>
    protected string? GetCurrentDepartment()
    {
        return User.FindFirst("department")?.Value;
    }

    /// <summary>
    /// Get user mode from JWT claims
    /// </summary>
    protected string? GetCurrentUserMode()
    {
        return User.FindFirst("userMode")?.Value;
    }

    /// <summary>
    /// Get user roles from JWT claims
    /// </summary>
    protected List<string> GetCurrentUserRoles()
    {
        return User.FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();
    }

    /// <summary>
    /// Check if current user is admin
    /// </summary>
    protected bool IsCurrentUserAdmin()
    {
        return User.FindFirst("isAdmin")?.Value == "true";
    }

    /// <summary>
    /// Check if current user is super admin
    /// </summary>
    protected bool IsCurrentUserSuperAdmin()
    {
        return User.FindFirst("isSuperAdmin")?.Value == "true";
    }

    /// <summary>
    /// Get full name from JWT claims
    /// </summary>
    protected string? GetCurrentUserFullName()
    {
        return User.FindFirst("fullName")?.Value;
    }

    /// <summary>
    /// Ensure user is authenticated and has required info
    /// </summary>
    protected IActionResult? ValidateUserContext(out string userId, out Guid tenantId)
    {
        var userIdClaim = GetCurrentUserId();
        var tenantIdClaim = GetCurrentTenantId();

        if (string.IsNullOrEmpty(userIdClaim) || !tenantIdClaim.HasValue)
        {
            userId = string.Empty;
            tenantId = Guid.Empty;
            return Unauthorized(new { message = "Invalid or incomplete token claims" });
        }

        userId = userIdClaim;
        tenantId = tenantIdClaim.Value;
        return null;
    }

    /// <summary>
    /// Log user context for debugging
    /// </summary>
    protected void LogUserContext(ILogger logger)
    {
        logger.LogInformation("User Context: UserId={UserId}, TenantId={TenantId}, Email={Email}, Roles={Roles}",
            GetCurrentUserId(),
            GetCurrentTenantId(),
            GetCurrentUserEmail(),
            string.Join(", ", GetCurrentUserRoles()));
    }
}

