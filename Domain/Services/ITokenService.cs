namespace Domain.Services;

/// <summary>
/// Legacy Token Service interface - kept for backward compatibility
/// Now implemented as an adapter that extracts info from JWT claims via HttpContext
/// </summary>
public interface ITokenService
{
    Guid? GetUserIdFromToken(string token);
    Guid? GetCompanyIdFromToken(string token);
    Guid? GetTeamIdFromToken(string token);
}

