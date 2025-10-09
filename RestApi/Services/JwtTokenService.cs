using Domain.Services;
using Domain.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Cherish.RestApi.Services;

public class JwtTokenService : ITokenService
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationMinutes;

    public JwtTokenService(IConfiguration configuration)
    {
        _secretKey = configuration["Jwt:SecretKey"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
        _issuer = configuration["Jwt:Issuer"] ?? "Cherish";
        _audience = configuration["Jwt:Audience"] ?? "Cherish";
        _expirationMinutes = int.Parse(configuration["Jwt:ExpirationMinutes"] ?? "60");
    }

    public string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("user_id", user.Id.ToString()),
            new Claim("company_id", user.CompanyId.ToString()),
            new Claim("role", ((int)user.Role).ToString()),
            new Claim("status", ((int)user.Status).ToString())
        };

        // Add team_id claim if user has a team
        if (user.TeamId.HasValue)
        {
            claims.Add(new Claim("team_id", user.TeamId.Value.ToString()));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_expirationMinutes),
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public bool ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public string? GetUsernameFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.ReadJwtToken(token);

            return jwt.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
        }
        catch
        {
            return null;
        }
    }

    public Guid? GetUserIdFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.ReadJwtToken(token);

            var userIdClaim = jwt.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
            return userIdClaim != null ? Guid.Parse(userIdClaim) : null;
        }
        catch
        {
            return null;
        }
    }

    public Guid? GetCompanyIdFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.ReadJwtToken(token);

            var companyIdClaim = jwt.Claims.FirstOrDefault(x => x.Type == "company_id")?.Value;
            return companyIdClaim != null ? Guid.Parse(companyIdClaim) : null;
        }
        catch
        {
            return null;
        }
    }

    public Guid? GetTeamIdFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.ReadJwtToken(token);

            var teamIdClaim = jwt.Claims.FirstOrDefault(x => x.Type == "team_id")?.Value;
            return teamIdClaim != null ? Guid.Parse(teamIdClaim) : null;
        }
        catch
        {
            return null;
        }
    }

    public UserRole? GetUserRoleFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.ReadJwtToken(token);

            var roleClaim = jwt.Claims.FirstOrDefault(x => x.Type == "role")?.Value;
            return roleClaim != null ? (UserRole)int.Parse(roleClaim) : null;
        }
        catch
        {
            return null;
        }
    }

    public UserStatus? GetUserStatusFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.ReadJwtToken(token);

            var statusClaim = jwt.Claims.FirstOrDefault(x => x.Type == "status")?.Value;
            return statusClaim != null ? (UserStatus)int.Parse(statusClaim) : null;
        }
        catch
        {
            return null;
        }
    }
}
