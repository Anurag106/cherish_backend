using Domain.Models;

namespace Domain.Services;

public interface IAnalyticsService
{
    // Team Dashboard
    Task<TeamDashboardData> GetTeamDashboardDataAsync(Guid companyId, TeamDashboardRequest request);
    
    // Leaderboard
    Task<LeaderboardData> GetLeaderboardDataAsync(Guid companyId, LeaderboardRequest request);
    
    // Recognition Analytics
    Task<RecognitionAnalyticsData> GetRecognitionAnalyticsDataAsync(Guid companyId, RecognitionAnalyticsRequest request);
    
    // Participation Analytics
    Task<ParticipationAnalyticsData> GetParticipationAnalyticsDataAsync(Guid companyId, ParticipationAnalyticsRequest request);
    
    // Organization Graph
    Task<OrganizationGraphData> GetOrganizationGraphDataAsync(Guid companyId, OrganizationGraphRequest request);
    
    // Top Words
    Task<TopWordsAnalyticsData> GetTopWordsAnalyticsDataAsync(Guid companyId, TopWordsAnalyticsRequest request);
}

