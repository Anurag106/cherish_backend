using Cherish.RestApi.Models.Responses;
using Domain.Models;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cherish.RestApi.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/analytics")]
[Authorize]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(
        IAnalyticsService analyticsService,
        ITokenService tokenService,
        ILogger<AnalyticsController> logger)
    {
        _analyticsService = analyticsService;
        _tokenService = tokenService;
        _logger = logger;
    }

    /// <summary>
    /// Get team dashboard analytics
    /// </summary>
    /// <param name="request">Dashboard query parameters</param>
    /// <returns>Team dashboard data</returns>
    [HttpGet("team-dashboard")]
    public async Task<ActionResult<ApiResponse<TeamDashboardData>>> GetTeamDashboard(
        [FromQuery] TeamDashboardRequest request)
    {
        try
        {
            var companyId = GetCompanyIdFromToken();
            if (companyId == null)
            {
                return Unauthorized(new ApiResponse<TeamDashboardData>
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var data = await _analyticsService.GetTeamDashboardDataAsync(companyId.Value, request);

            return Ok(new ApiResponse<TeamDashboardData>
            {
                Success = true,
                Message = "Team dashboard data retrieved successfully",
                Data = data
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving team dashboard data");
            return StatusCode(500, new ApiResponse<TeamDashboardData>
            {
                Success = false,
                Message = "An error occurred while retrieving team dashboard data"
            });
        }
    }

    /// <summary>
    /// Get leaderboard analytics
    /// </summary>
    /// <param name="request">Leaderboard query parameters</param>
    /// <returns>Leaderboard data</returns>
    [HttpGet("leaderboard")]
    public async Task<ActionResult<ApiResponse<LeaderboardData>>> GetLeaderboard(
        [FromQuery] LeaderboardRequest request)
    {
        try
        {
            var companyId = GetCompanyIdFromToken();
            if (companyId == null)
            {
                return Unauthorized(new ApiResponse<LeaderboardData>
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var data = await _analyticsService.GetLeaderboardDataAsync(companyId.Value, request);

            return Ok(new ApiResponse<LeaderboardData>
            {
                Success = true,
                Message = "Leaderboard data retrieved successfully",
                Data = data
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving leaderboard data");
            return StatusCode(500, new ApiResponse<LeaderboardData>
            {
                Success = false,
                Message = "An error occurred while retrieving leaderboard data"
            });
        }
    }

    /// <summary>
    /// Get recognition analytics
    /// </summary>
    /// <param name="request">Recognition analytics query parameters</param>
    /// <returns>Recognition analytics data</returns>
    [HttpGet("recognition")]
    public async Task<ActionResult<ApiResponse<RecognitionAnalyticsData>>> GetRecognitionAnalytics(
        [FromQuery] RecognitionAnalyticsRequest request)
    {
        try
        {
            var companyId = GetCompanyIdFromToken();
            if (companyId == null)
            {
                return Unauthorized(new ApiResponse<RecognitionAnalyticsData>
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var data = await _analyticsService.GetRecognitionAnalyticsDataAsync(companyId.Value, request);

            return Ok(new ApiResponse<RecognitionAnalyticsData>
            {
                Success = true,
                Message = "Recognition analytics data retrieved successfully",
                Data = data
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving recognition analytics data");
            return StatusCode(500, new ApiResponse<RecognitionAnalyticsData>
            {
                Success = false,
                Message = "An error occurred while retrieving recognition analytics data"
            });
        }
    }

    /// <summary>
    /// Get participation analytics
    /// </summary>
    /// <param name="request">Participation analytics query parameters</param>
    /// <returns>Participation analytics data</returns>
    [HttpGet("participation")]
    public async Task<ActionResult<ApiResponse<ParticipationAnalyticsData>>> GetParticipationAnalytics(
        [FromQuery] ParticipationAnalyticsRequest request)
    {
        try
        {
            var companyId = GetCompanyIdFromToken();
            if (companyId == null)
            {
                return Unauthorized(new ApiResponse<ParticipationAnalyticsData>
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var data = await _analyticsService.GetParticipationAnalyticsDataAsync(companyId.Value, request);

            return Ok(new ApiResponse<ParticipationAnalyticsData>
            {
                Success = true,
                Message = "Participation analytics data retrieved successfully",
                Data = data
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving participation analytics data");
            return StatusCode(500, new ApiResponse<ParticipationAnalyticsData>
            {
                Success = false,
                Message = "An error occurred while retrieving participation analytics data"
            });
        }
    }

    /// <summary>
    /// Get organization graph analytics
    /// </summary>
    /// <param name="request">Organization graph query parameters</param>
    /// <returns>Organization graph data</returns>
    [HttpGet("organization-graph")]
    public async Task<ActionResult<ApiResponse<OrganizationGraphData>>> GetOrganizationGraph(
        [FromQuery] OrganizationGraphRequest request)
    {
        try
        {
            var companyId = GetCompanyIdFromToken();
            if (companyId == null)
            {
                return Unauthorized(new ApiResponse<OrganizationGraphData>
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var data = await _analyticsService.GetOrganizationGraphDataAsync(companyId.Value, request);

            return Ok(new ApiResponse<OrganizationGraphData>
            {
                Success = true,
                Message = "Organization graph data retrieved successfully",
                Data = data
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving organization graph data");
            return StatusCode(500, new ApiResponse<OrganizationGraphData>
            {
                Success = false,
                Message = "An error occurred while retrieving organization graph data"
            });
        }
    }

    /// <summary>
    /// Get top words analytics
    /// </summary>
    /// <param name="request">Top words analytics query parameters</param>
    /// <returns>Top words analytics data</returns>
    [HttpGet("top-words")]
    public async Task<ActionResult<ApiResponse<TopWordsAnalyticsData>>> GetTopWords(
        [FromQuery] TopWordsAnalyticsRequest request)
    {
        try
        {
            var companyId = GetCompanyIdFromToken();
            if (companyId == null)
            {
                return Unauthorized(new ApiResponse<TopWordsAnalyticsData>
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var data = await _analyticsService.GetTopWordsAnalyticsDataAsync(companyId.Value, request);

            return Ok(new ApiResponse<TopWordsAnalyticsData>
            {
                Success = true,
                Message = "Top words analytics data retrieved successfully",
                Data = data
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving top words analytics data");
            return StatusCode(500, new ApiResponse<TopWordsAnalyticsData>
            {
                Success = false,
                Message = "An error occurred while retrieving top words analytics data"
            });
        }
    }

    private Guid? GetCompanyIdFromToken()
    {
        var token = GetTokenFromRequest();
        return _tokenService.GetCompanyIdFromToken(token);
    }

    private string GetTokenFromRequest()
    {
        return Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last() ?? string.Empty;
    }
}

