using Domain.Models;
using Domain.Providers;
using Domain.Services;

namespace Cherish.RestApi.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly IAnalyticsProvider _analyticsProvider;
    private readonly ILogger<AnalyticsService> _logger;

    public AnalyticsService(
        IAnalyticsProvider analyticsProvider,
        ILogger<AnalyticsService> logger)
    {
        _analyticsProvider = analyticsProvider;
        _logger = logger;
    }

    public async Task<TeamDashboardData> GetTeamDashboardDataAsync(Guid companyId, TeamDashboardRequest request)
    {
        try
        {
            return await _analyticsProvider.GetTeamDashboardDataAsync(companyId, request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving team dashboard data for company: {CompanyId}", companyId);
            throw;
        }
    }

    public async Task<LeaderboardData> GetLeaderboardDataAsync(Guid companyId, LeaderboardRequest request)
    {
        try
        {
            return await _analyticsProvider.GetLeaderboardDataAsync(companyId, request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving leaderboard data for company: {CompanyId}", companyId);
            throw;
        }
    }

    public async Task<RecognitionAnalyticsData> GetRecognitionAnalyticsDataAsync(Guid companyId, RecognitionAnalyticsRequest request)
    {
        try
        {
            return await _analyticsProvider.GetRecognitionAnalyticsDataAsync(companyId, request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving recognition analytics data for company: {CompanyId}", companyId);
            throw;
        }
    }

    public async Task<ParticipationAnalyticsData> GetParticipationAnalyticsDataAsync(Guid companyId, ParticipationAnalyticsRequest request)
    {
        try
        {
            return await _analyticsProvider.GetParticipationAnalyticsDataAsync(companyId, request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving participation analytics data for company: {CompanyId}", companyId);
            throw;
        }
    }

    public async Task<OrganizationGraphData> GetOrganizationGraphDataAsync(Guid companyId, OrganizationGraphRequest request)
    {
        try
        {
            return await _analyticsProvider.GetOrganizationGraphDataAsync(companyId, request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving organization graph data for company: {CompanyId}", companyId);
            throw;
        }
    }

    public async Task<TopWordsAnalyticsData> GetTopWordsAnalyticsDataAsync(Guid companyId, TopWordsAnalyticsRequest request)
    {
        try
        {
            return await _analyticsProvider.GetTopWordsAnalyticsDataAsync(companyId, request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving top words analytics data for company: {CompanyId}", companyId);
            throw;
        }
    }
}

