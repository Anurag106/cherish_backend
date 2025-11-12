namespace Domain.Models;

// Date range enums
public enum DateRange
{
    Last7Days = 0,
    Last30Days = 1,
    Last90Days = 2,
    Last6Months = 3,
    Last1Year = 4,
    Custom = 5
}

public enum RecognitionType
{
    Received = 0,
    Given = 1
}

public enum RecognitionCategory
{
    RecognitionReceived = 0,
    RecognitionRates = 1,
    Hashtags = 2,
    AddOns = 3
}

public enum RecognitionView
{
    Overview = 0,
    CompareRates = 1
}

public enum OverallFilter
{
    All = 0,
    Overall = 1,
    Department = 2,
    Location = 3
}

// Team Dashboard Models
public class TeamDashboardMetrics
{
    public int TotalRecognitions { get; set; }
    public int ActiveUsers { get; set; }
    public decimal AverageResponseTime { get; set; } // in hours
    public decimal EngagementRate { get; set; } // percentage
}

public class TopPerformer
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int RecognitionsReceived { get; set; }
    public string Department { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
}

public class RecognitionTrend
{
    public string Date { get; set; } = string.Empty; // "2024-01-15"
    public int Count { get; set; }
    public int Points { get; set; }
}

public class DepartmentBreakdown
{
    public string Department { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Percentage { get; set; }
}

public class HashtagDistribution
{
    public string Name { get; set; } = string.Empty;
    public int Value { get; set; }
    public decimal Percentage { get; set; }
}

public class TeamDashboardData
{
    public TeamDashboardMetrics Metrics { get; set; } = new();
    public List<TopPerformer> TopPerformers { get; set; } = new();
    public List<RecognitionTrend> RecognitionTrends { get; set; } = new();
    public List<DepartmentBreakdown> DepartmentBreakdown { get; set; } = new();
    public List<HashtagDistribution> HashtagDistribution { get; set; } = new();
}

// Leaderboard Models
public class LeaderboardEntry
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public List<HashtagValue> Values { get; set; } = new();
    public int Total { get; set; }
}

public class HashtagValue
{
    public string Hashtag { get; set; } = string.Empty;
    public int Value { get; set; }
    public string Color { get; set; } = "#000000";
}

public class LeaderboardData
{
    public List<LeaderboardEntry> Leaderboard { get; set; } = new();
    public List<string> AvailableHashtags { get; set; } = new();
    public List<string> AvailableDepartments { get; set; } = new();
    public int TotalEntries { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}

// Recognition Analytics Models
public class RecognitionMetric
{
    public int Total { get; set; }
    public TrendData Trend { get; set; } = new();
    public decimal PerMonth { get; set; }
}

public class TrendData
{
    public decimal Value { get; set; } // percentage change
    public string Period { get; set; } = string.Empty; // e.g., "August"
}

public class P2pVsAwardsData
{
    public RecognitionCountsData P2p { get; set; } = new();
    public RecognitionCountsData Awards { get; set; } = new();
}

public class RecognitionCountsData
{
    public int Count { get; set; }
    public int Points { get; set; }
}

public class RecognitionChartData
{
    public string Date { get; set; } = string.Empty; // "2024-01-01"
    public int P2p { get; set; }
    public int Awards { get; set; }
}

public class NumberOfUsersChartData
{
    public string Date { get; set; } = string.Empty;
    public int Users { get; set; }
}

public class RecognitionMetrics
{
    public RecognitionMetric RecognitionReceived { get; set; } = new();
    public RecognitionMetric PointReceived { get; set; } = new();
    public P2pVsAwardsData P2pVsAwards { get; set; } = new();
}

public class AppliedFilters
{
    public string TimeRange { get; set; } = string.Empty;
    public string View { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string? Hashtag { get; set; }
}

public class RecognitionAnalyticsData
{
    public RecognitionMetrics Metrics { get; set; } = new();
    public List<RecognitionChartData> RecognitionReceivedChart { get; set; } = new();
    public List<RecognitionChartData> PointReceivedChart { get; set; } = new();
    public List<NumberOfUsersChartData> NumberOfUsersChart { get; set; } = new();
    public AppliedFilters AppliedFilters { get; set; } = new();
}

// Participation Analytics Models
public class ParticipationMetrics
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
    public decimal ParticipationRate { get; set; } // percentage
    public decimal AverageRecognitionsPerUser { get; set; }
    public int NewUsersThisMonth { get; set; }
    public decimal GrowthRate { get; set; } // percentage
}

public class ActivityTrend
{
    public string Date { get; set; } = string.Empty; // "2024-01-15"
    public int ActiveUsers { get; set; }
    public int NewUsers { get; set; }
    public int RecognitionsGiven { get; set; }
    public int RecognitionsReceived { get; set; }
}

public class DepartmentParticipation
{
    public string Department { get; set; } = string.Empty;
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
    public decimal ParticipationRate { get; set; }
    public decimal AvgRecognitionsPerUser { get; set; }
}

public class EngagementLevel
{
    public string Level { get; set; } = string.Empty; // e.g., "Super Engaged"
    public string Description { get; set; } = string.Empty; // e.g., "20+ recognitions/month"
    public int UserCount { get; set; }
    public decimal Percentage { get; set; }
    public string Color { get; set; } = "#000000";
}

public class UserSegment
{
    public string Segment { get; set; } = string.Empty; // e.g., "Active Contributors"
    public int UserCount { get; set; }
    public decimal Percentage { get; set; }
    public string Trend { get; set; } = "stable"; // "up" | "down" | "stable"
    public decimal TrendValue { get; set; }
}

public class TopContributor
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int RecognitionsGiven { get; set; }
    public int RecognitionsReceived { get; set; }
    public string Department { get; set; } = string.Empty;
}

public class ParticipationAnalyticsData
{
    public ParticipationMetrics Metrics { get; set; } = new();
    public List<ActivityTrend> ActivityTrends { get; set; } = new();
    public List<DepartmentParticipation> DepartmentParticipation { get; set; } = new();
    public List<EngagementLevel> EngagementLevels { get; set; } = new();
    public List<UserSegment> UserSegments { get; set; } = new();
    public List<TopContributor> TopContributors { get; set; } = new();
}

// Organization Graph Models
public class OrganizationNode
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Initials { get; set; } = string.Empty; // e.g., "RS"
    public string Department { get; set; } = string.Empty;
    public string Color { get; set; } = "#000000";
    public int RecognitionsGiven { get; set; }
    public int RecognitionsReceived { get; set; }
    public int? X { get; set; }
    public int? Y { get; set; }
}

public class OrganizationEdge
{
    public string Source { get; set; } = string.Empty; // node id
    public string Target { get; set; } = string.Empty; // node id
    public int Weight { get; set; } // number of recognitions
}

public class OrganizationTeamMember
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Initials { get; set; } = string.Empty;
    public string Color { get; set; } = "#000000";
    public int Given { get; set; }
    public int Received { get; set; }
    public int Total { get; set; }
}

public class OrganizationGraphMetrics
{
    public int TotalNodes { get; set; }
    public int TotalConnections { get; set; }
    public decimal AverageConnections { get; set; }
    public string MostConnectedUser { get; set; } = string.Empty;
}

public class OrganizationGraphData
{
    public List<string> Breadcrumb { get; set; } = new(); // e.g., ["Cherish", "San Luis Obispo", "Alex's Team"]
    public List<OrganizationNode> Nodes { get; set; } = new();
    public List<OrganizationEdge> Edges { get; set; } = new();
    public List<OrganizationTeamMember> WithinTeam { get; set; } = new();
    public List<OrganizationTeamMember> OutsideTeam { get; set; } = new();
    public OrganizationGraphMetrics Metrics { get; set; } = new();
}

// Top Words Models
public class WordCloudEntry
{
    public string Word { get; set; } = string.Empty;
    public int Value { get; set; } // frequency count
    public string Color { get; set; } = "#000000";
}

public class MostUsedWord
{
    public string Word { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class TopWordsMetrics
{
    public MostUsedWord MostUsedWord { get; set; } = new();
    public int TotalUniqueWords { get; set; }
    public decimal AverageWordsPerMessage { get; set; }
}

public class TopWordEntry
{
    public int Rank { get; set; }
    public string Word { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Percentage { get; set; }
}

public class TopWordsFilters
{
    public string? Overall { get; set; }
    public string? Location { get; set; }
    public string? CurrencyCode { get; set; }
    public string? Group { get; set; }
    public string? Role { get; set; }
    public string? ManagersTeam { get; set; }
    public string? Tier { get; set; }
    public string? Title { get; set; }
}

public class TopWordsAppliedFilters
{
    public string DateRange { get; set; } = string.Empty;
    public TopWordsFilters Filters { get; set; } = new();
}

public class TopWordsAnalyticsData
{
    public List<WordCloudEntry> WordCloud { get; set; } = new(); // Top 20-50 words
    public TopWordsMetrics Metrics { get; set; } = new();
    public List<TopWordEntry> TopWordsList { get; set; } = new(); // Top 10 words
    public TopWordsAppliedFilters AppliedFilters { get; set; } = new();
}

// Request Models
public class TeamDashboardRequest
{
    public DateRange DateRange { get; set; } = DateRange.Last30Days;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Department { get; set; }
}

public class LeaderboardRequest
{
    public DateRange DateRange { get; set; } = DateRange.Last30Days;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Department { get; set; } = "all";
    public string Hashtag { get; set; } = "all";
    public RecognitionType RecognitionType { get; set; } = RecognitionType.Received;
    public string? SearchQuery { get; set; }
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 50;
}

public class RecognitionAnalyticsRequest
{
    public DateRange DateRange { get; set; } = DateRange.Last30Days;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Department { get; set; }
    public string? Hashtag { get; set; }
    public RecognitionView View { get; set; } = RecognitionView.Overview;
    public RecognitionCategory Category { get; set; } = RecognitionCategory.RecognitionReceived;
}

public class ParticipationAnalyticsRequest
{
    public DateRange DateRange { get; set; } = DateRange.Last30Days;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Department { get; set; }
}

public class OrganizationGraphRequest
{
    public Guid? TeamId { get; set; }
    public Guid? ManagerId { get; set; }
    public DateRange DateRange { get; set; } = DateRange.Last30Days;
}

public class TopWordsAnalyticsRequest
{
    public DateRange DateRange { get; set; } = DateRange.Last30Days;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public OverallFilter Overall { get; set; } = OverallFilter.All;
    public string? Location { get; set; }
    public string? CurrencyCode { get; set; }
    public string? Group { get; set; }
    public string? Role { get; set; }
    public string? ManagersTeam { get; set; }
    public string? Tier { get; set; }
    public string? Title { get; set; }
}

