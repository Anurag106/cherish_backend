using Domain.Services;
using System.Security.Claims;

namespace Cherish.RestApi.Services;

/// <summary>
/// Adapter that implements ITokenService using JWT claims from HttpContext
/// This is a compatibility layer - new code should use BaseApiController methods directly
/// </summary>
public class JwtClaimsAdapter : ITokenService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public JwtClaimsAdapter(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    /// <summary>
    /// Get user ID from JWT claims (sub claim)
    /// NOTE: JWT contains string user ID, but legacy code expects Guid
    /// This will return null for string IDs that aren't valid GUIDs
    /// </summary>
    public Guid? GetUserIdFromToken(string token)
    {
        // Token parameter is ignored - we get claims from HttpContext instead
        var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? User?.FindFirst("sub")?.Value;
        
        if (string.IsNullOrEmpty(userId))
            return null;
            
        // Try to parse as Guid (will fail for string IDs)
        if (Guid.TryParse(userId, out var guid))
            return guid;
        
        // For string IDs, we can't convert to Guid
        // Log this so we know migration is needed
        Console.WriteLine($"[JwtClaimsAdapter] User ID is string, not Guid: {userId}. Database migration needed!");
        return null;
    }
    
    /// <summary>
    /// Get company/tenant ID from JWT claims
    /// </summary>
    public Guid? GetCompanyIdFromToken(string token)
    {
        // Token parameter is ignored - we get claims from HttpContext instead
        var tenantId = User?.FindFirst("tenantId")?.Value;
        
        if (string.IsNullOrEmpty(tenantId))
            return null;
            
        if (Guid.TryParse(tenantId, out var guid))
            return guid;
        
        return null;
    }
    
    /// <summary>
    /// Get team ID from JWT claims
    /// </summary>
    public Guid? GetTeamIdFromToken(string token)
    {
        // Token parameter is ignored - we get claims from HttpContext instead
        var teamId = User?.FindFirst("teamId")?.Value;
        
        if (string.IsNullOrEmpty(teamId))
            return null;
            
        if (Guid.TryParse(teamId, out var guid))
            return guid;
        
        return null;
    }
}

