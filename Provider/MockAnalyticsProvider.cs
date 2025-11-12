using Domain.Models;
using Domain.Providers;

namespace Provider;

public class MockAnalyticsProvider : IAnalyticsProvider
{
    private readonly Random _random = new();

    public Task<TeamDashboardData> GetTeamDashboardDataAsync(Guid companyId, TeamDashboardRequest request)
    {
        var data = new TeamDashboardData
        {
            Metrics = new TeamDashboardMetrics
            {
                TotalRecognitions = 245,
                ActiveUsers = 87,
                AverageResponseTime = 2.5m,
                EngagementRate = 68.4m
            },
            TopPerformers = GenerateTopPerformers(5),
            RecognitionTrends = GenerateRecognitionTrends(30),
            DepartmentBreakdown = GenerateDepartmentBreakdown(),
            HashtagDistribution = GenerateHashtagDistribution(6)
        };

        return Task.FromResult(data);
    }

    public Task<LeaderboardData> GetLeaderboardDataAsync(Guid companyId, LeaderboardRequest request)
    {
        var entries = GenerateLeaderboardEntries(request.Limit);
        var totalPages = (int)Math.Ceiling(entries.Count / (double)request.Limit);

        var data = new LeaderboardData
        {
            Leaderboard = entries.Skip((request.Page - 1) * request.Limit).Take(request.Limit).ToList(),
            AvailableHashtags = new List<string> { "Teamwork", "Innovation", "Leadership", "Quality", "Customer Focus" },
            AvailableDepartments = new List<string> { "Engineering", "Sales", "Marketing", "HR", "Operations" },
            TotalEntries = entries.Count,
            CurrentPage = request.Page,
            TotalPages = totalPages
        };

        return Task.FromResult(data);
    }

    public Task<RecognitionAnalyticsData> GetRecognitionAnalyticsDataAsync(Guid companyId, RecognitionAnalyticsRequest request)
    {
        var data = new RecognitionAnalyticsData
        {
            Metrics = new RecognitionMetrics
            {
                RecognitionReceived = new RecognitionMetric
                {
                    Total = 1245,
                    Trend = new TrendData { Value = 12.5m, Period = "August" },
                    PerMonth = 145.3m
                },
                PointReceived = new RecognitionMetric
                {
                    Total = 12500,
                    Trend = new TrendData { Value = 8.2m, Period = "August" },
                    PerMonth = 1450.2m
                },
                P2pVsAwards = new P2pVsAwardsData
                {
                    P2p = new RecognitionCountsData { Count = 980, Points = 9800 },
                    Awards = new RecognitionCountsData { Count = 265, Points = 2700 }
                }
            },
            RecognitionReceivedChart = GenerateRecognitionChartData(12),
            PointReceivedChart = GenerateRecognitionChartData(12),
            NumberOfUsersChart = GenerateNumberOfUsersChartData(12),
            AppliedFilters = new AppliedFilters
            {
                TimeRange = request.DateRange.ToString(),
                View = request.View.ToString(),
                Category = request.Category.ToString(),
                Department = request.Department,
                Hashtag = request.Hashtag
            }
        };

        return Task.FromResult(data);
    }

    public Task<ParticipationAnalyticsData> GetParticipationAnalyticsDataAsync(Guid companyId, ParticipationAnalyticsRequest request)
    {
        var data = new ParticipationAnalyticsData
        {
            Metrics = new ParticipationMetrics
            {
                TotalUsers = 150,
                ActiveUsers = 87,
                InactiveUsers = 63,
                ParticipationRate = 58.0m,
                AverageRecognitionsPerUser = 14.3m,
                NewUsersThisMonth = 12,
                GrowthRate = 8.7m
            },
            ActivityTrends = GenerateActivityTrends(30),
            DepartmentParticipation = GenerateDepartmentParticipation(),
            EngagementLevels = GenerateEngagementLevels(),
            UserSegments = GenerateUserSegments(),
            TopContributors = GenerateTopContributors(5)
        };

        return Task.FromResult(data);
    }

    public Task<OrganizationGraphData> GetOrganizationGraphDataAsync(Guid companyId, OrganizationGraphRequest request)
    {
        var nodes = GenerateOrganizationNodes(20);
        var edges = GenerateOrganizationEdges(nodes);

        var data = new OrganizationGraphData
        {
            Breadcrumb = new List<string> { "Cherish", "San Luis Obispo", "Engineering Team" },
            Nodes = nodes,
            Edges = edges,
            WithinTeam = GenerateOrganizationTeamMembers(5, "within"),
            OutsideTeam = GenerateOrganizationTeamMembers(3, "outside"),
            Metrics = new OrganizationGraphMetrics
            {
                TotalNodes = nodes.Count,
                TotalConnections = edges.Count,
                AverageConnections = Math.Round(edges.Count / (decimal)nodes.Count, 2),
                MostConnectedUser = nodes.OrderByDescending(n => _random.Next(100)).First().Name
            }
        };

        return Task.FromResult(data);
    }

    public Task<TopWordsAnalyticsData> GetTopWordsAnalyticsDataAsync(Guid companyId, TopWordsAnalyticsRequest request)
    {
        var words = new List<(string Word, int Count)>
        {
            ("great", 245), ("excellent", 198), ("teamwork", 187),
            ("outstanding", 165), ("awesome", 142), ("amazing", 128),
            ("fantastic", 115), ("brilliant", 98), ("wonderful", 87),
            ("incredible", 76), ("superb", 65), ("remarkable", 54),
            ("exceptional", 48), ("impressive", 42), ("phenomenal", 38),
            ("fabulous", 35), ("terrific", 32), ("magnificent", 28),
            ("spectacular", 25), ("stellar", 22)
        };

        var totalCount = words.Sum(w => w.Count);
        var topWords = words.Take(10).Select((w, i) => new TopWordEntry
        {
            Rank = i + 1,
            Word = w.Word,
            Count = w.Count,
            Percentage = Math.Round((w.Count / (decimal)totalCount) * 100, 2)
        }).ToList();

        var data = new TopWordsAnalyticsData
        {
            WordCloud = words.Select(w => new WordCloudEntry
            {
                Word = w.Word,
                Value = w.Count,
                Color = GetColorForWord(w.Word)
            }).ToList(),
            Metrics = new TopWordsMetrics
            {
                MostUsedWord = new MostUsedWord { Word = words[0].Word, Count = words[0].Count },
                TotalUniqueWords = 1247,
                AverageWordsPerMessage = 28.5m
            },
            TopWordsList = topWords,
            AppliedFilters = new TopWordsAppliedFilters
            {
                DateRange = request.DateRange.ToString(),
                Filters = new TopWordsFilters
                {
                    Overall = request.Overall.ToString(),
                    Location = request.Location,
                    CurrencyCode = request.CurrencyCode,
                    Group = request.Group,
                    Role = request.Role,
                    ManagersTeam = request.ManagersTeam,
                    Tier = request.Tier,
                    Title = request.Title
                }
            }
        };

        return Task.FromResult(data);
    }

    #region Private Helper Methods

    private List<TopPerformer> GenerateTopPerformers(int count)
    {
        var names = new[] { "Alex Johnson", "Sarah Martinez", "David Chen", "Emily Brown", "Michael Lee" };
        var departments = new[] { "Engineering", "Sales", "Marketing", "HR", "Operations" };

        return Enumerable.Range(0, count).Select(i => new TopPerformer
        {
            UserId = Guid.NewGuid(),
            FullName = names[i % names.Length],
            RecognitionsReceived = 45 - (i * 5),
            Department = departments[i % departments.Length],
            AvatarUrl = null
        }).ToList();
    }

    private List<RecognitionTrend> GenerateRecognitionTrends(int days)
    {
        return Enumerable.Range(0, days).Select(i =>
        {
            var date = DateTime.Now.AddDays(-i).ToString("yyyy-MM-dd");
            var count = _random.Next(15, 50);
            return new RecognitionTrend
            {
                Date = date,
                Count = count,
                Points = count * 10
            };
        }).Reverse().ToList();
    }

    private List<DepartmentBreakdown> GenerateDepartmentBreakdown()
    {
        var departments = new[]
        {
            ("Engineering", 85, 34.7m),
            ("Sales", 62, 25.3m),
            ("Marketing", 45, 18.4m),
            ("HR", 35, 14.3m),
            ("Operations", 18, 7.3m)
        };

        return departments.Select(d => new DepartmentBreakdown
        {
            Department = d.Item1,
            Count = d.Item2,
            Percentage = d.Item3
        }).ToList();
    }

    private List<HashtagDistribution> GenerateHashtagDistribution(int count)
    {
        var hashtags = new[]
        {
            ("Teamwork", 98, 40.0m),
            ("Innovation", 65, 26.5m),
            ("Leadership", 45, 18.4m),
            ("Quality", 28, 11.4m),
            ("Customer Focus", 9, 3.7m),
            ("Excellence", 5, 2.0m)
        };

        return hashtags.Take(count).Select(h => new HashtagDistribution
        {
            Name = h.Item1,
            Value = h.Item2,
            Percentage = h.Item3
        }).ToList();
    }

    private List<LeaderboardEntry> GenerateLeaderboardEntries(int count)
    {
        var names = new[] { "Alex Johnson", "Sarah Martinez", "David Chen", "Emily Brown", "Michael Lee", 
                           "Jessica Wilson", "Robert Taylor", "Amanda Anderson", "Daniel Thomas", "Jennifer Garcia" };
        var departments = new[] { "Engineering", "Sales", "Marketing", "HR", "Operations" };
        var hashtags = new[] { "Teamwork", "Innovation", "Leadership", "Quality", "Customer Focus" };
        var colors = new[] { "#3b82f6", "#10b981", "#f59e0b", "#ef4444", "#8b5cf6" };

        return Enumerable.Range(0, count).Select(i =>
        {
            var entry = new LeaderboardEntry
            {
                UserId = Guid.NewGuid(),
                Username = $"user{i + 1}",
                Name = names[i % names.Length],
                Department = departments[i % departments.Length],
                Total = 250 - (i * 10),
                Values = new List<HashtagValue>()
            };

            // Add 2-3 hashtag values per entry
            var numHashtags = _random.Next(2, 4);
            for (int j = 0; j < numHashtags; j++)
            {
                var hashtagIndex = _random.Next(hashtags.Length);
                entry.Values.Add(new HashtagValue
                {
                    Hashtag = hashtags[hashtagIndex],
                    Value = _random.Next(20, 80),
                    Color = colors[j % colors.Length]
                });
            }

            return entry;
        }).OrderByDescending(e => e.Total).ToList();
    }

    private List<RecognitionChartData> GenerateRecognitionChartData(int months)
    {
        return Enumerable.Range(0, months).Select(i =>
        {
            var date = DateTime.Now.AddMonths(-months + i + 1).ToString("yyyy-MM-dd");
            var p2p = _random.Next(50, 120);
            var awards = _random.Next(10, 50);
            return new RecognitionChartData
            {
                Date = date,
                P2p = p2p,
                Awards = awards
            };
        }).ToList();
    }

    private List<NumberOfUsersChartData> GenerateNumberOfUsersChartData(int months)
    {
        return Enumerable.Range(0, months).Select(i =>
        {
            var date = DateTime.Now.AddMonths(-months + i + 1).ToString("yyyy-MM-dd");
            return new NumberOfUsersChartData
            {
                Date = date,
                Users = _random.Next(60, 100)
            };
        }).ToList();
    }

    private List<ActivityTrend> GenerateActivityTrends(int days)
    {
        return Enumerable.Range(0, days).Select(i =>
        {
            var date = DateTime.Now.AddDays(-i).ToString("yyyy-MM-dd");
            return new ActivityTrend
            {
                Date = date,
                ActiveUsers = _random.Next(40, 70),
                NewUsers = i == 0 ? _random.Next(1, 5) : 0,
                RecognitionsGiven = _random.Next(20, 60),
                RecognitionsReceived = _random.Next(20, 60)
            };
        }).Reverse().ToList();
    }

    private List<DepartmentParticipation> GenerateDepartmentParticipation()
    {
        var departments = new[]
        {
            ("Engineering", 45, 38, 7, 84.4m, 18.2m),
            ("Sales", 30, 25, 5, 83.3m, 16.5m),
            ("Marketing", 25, 15, 10, 60.0m, 12.8m),
            ("HR", 20, 12, 8, 60.0m, 14.0m),
            ("Operations", 30, 15, 15, 50.0m, 9.5m)
        };

        return departments.Select(d => new DepartmentParticipation
        {
            Department = d.Item1,
            TotalUsers = d.Item2,
            ActiveUsers = d.Item3,
            InactiveUsers = d.Item4,
            ParticipationRate = d.Item5,
            AvgRecognitionsPerUser = d.Item6
        }).ToList();
    }

    private List<EngagementLevel> GenerateEngagementLevels()
    {
        return new List<EngagementLevel>
        {
            new() { Level = "Super Engaged", Description = "20+ recognitions/month", UserCount = 25, Percentage = 16.7m, Color = "#10b981" },
            new() { Level = "Highly Engaged", Description = "10-19 recognitions/month", UserCount = 35, Percentage = 23.3m, Color = "#3b82f6" },
            new() { Level = "Moderately Engaged", Description = "5-9 recognitions/month", UserCount = 27, Percentage = 18.0m, Color = "#f59e0b" },
            new() { Level = "Low Engagement", Description = "1-4 recognitions/month", UserCount = 40, Percentage = 26.7m, Color = "#ef4444" },
            new() { Level = "Not Engaged", Description = "0 recognitions/month", UserCount = 23, Percentage = 15.3m, Color = "#6b7280" }
        };
    }

    private List<UserSegment> GenerateUserSegments()
    {
        return new List<UserSegment>
        {
            new() { Segment = "Active Contributors", UserCount = 87, Percentage = 58.0m, Trend = "up", TrendValue = 5.2m },
            new() { Segment = "Occasional Users", UserCount = 45, Percentage = 30.0m, Trend = "stable", TrendValue = 0m },
            new() { Segment = "New Users", UserCount = 12, Percentage = 8.0m, Trend = "up", TrendValue = 8.7m },
            new() { Segment = "Inactive Users", UserCount = 6, Percentage = 4.0m, Trend = "down", TrendValue = -2.3m }
        };
    }

    private List<TopContributor> GenerateTopContributors(int count)
    {
        var names = new[] { "Alex Johnson", "Sarah Martinez", "David Chen", "Emily Brown", "Michael Lee" };
        var departments = new[] { "Engineering", "Sales", "Marketing", "HR", "Operations" };

        return Enumerable.Range(0, count).Select(i => new TopContributor
        {
            UserId = Guid.NewGuid(),
            Name = names[i % names.Length],
            RecognitionsGiven = 45 - (i * 3),
            RecognitionsReceived = 50 - (i * 4),
            Department = departments[i % departments.Length]
        }).ToList();
    }

    private List<OrganizationNode> GenerateOrganizationNodes(int count)
    {
        var firstNames = new[] { "Alex", "Sarah", "David", "Emily", "Michael", "Jessica", "Robert", "Amanda", "Daniel", "Jennifer" };
        var lastNames = new[] { "Johnson", "Martinez", "Chen", "Brown", "Lee", "Wilson", "Taylor", "Anderson", "Thomas", "Garcia" };
        var departments = new[] { "Engineering", "Sales", "Marketing", "HR", "Operations" };
        var colors = new[] { "#3b82f6", "#10b981", "#f59e0b", "#ef4444", "#8b5cf6", "#ec4899", "#14b8a6", "#f97316" };

        return Enumerable.Range(0, count).Select(i =>
        {
            var firstName = firstNames[i % firstNames.Length];
            var lastName = lastNames[i % lastNames.Length];
            var name = $"{firstName} {lastName}";
            return new OrganizationNode
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Initials = $"{firstName[0]}{lastName[0]}",
                Department = departments[i % departments.Length],
                Color = colors[i % colors.Length],
                RecognitionsGiven = _random.Next(10, 50),
                RecognitionsReceived = _random.Next(15, 60)
            };
        }).ToList();
    }

    private List<OrganizationEdge> GenerateOrganizationEdges(List<OrganizationNode> nodes)
    {
        var edges = new List<OrganizationEdge>();
        
        // Create connections between random nodes
        for (int i = 0; i < nodes.Count * 2; i++)
        {
            var source = nodes[_random.Next(nodes.Count)];
            var target = nodes[_random.Next(nodes.Count)];
            
            if (source.Id != target.Id && !edges.Any(e => e.Source == source.Id && e.Target == target.Id))
            {
                edges.Add(new OrganizationEdge
                {
                    Source = source.Id,
                    Target = target.Id,
                    Weight = _random.Next(1, 10)
                });
            }
        }

        return edges;
    }

    private List<OrganizationTeamMember> GenerateOrganizationTeamMembers(int count, string type)
    {
        var firstNames = new[] { "Alex", "Sarah", "David", "Emily", "Michael", "Jessica", "Robert", "Amanda", "Daniel", "Jennifer" };
        var lastNames = new[] { "Johnson", "Martinez", "Chen", "Brown", "Lee", "Wilson", "Taylor", "Anderson", "Thomas", "Garcia" };
        var colors = new[] { "#3b82f6", "#10b981", "#f59e0b", "#ef4444", "#8b5cf6", "#ec4899", "#14b8a6", "#f97316" };

        return Enumerable.Range(0, count).Select(i =>
        {
            var firstName = firstNames[i % firstNames.Length];
            var lastName = lastNames[i % lastNames.Length];
            var name = $"{firstName} {lastName}";
            var given = _random.Next(10, 50);
            var received = _random.Next(15, 60);
            return new OrganizationTeamMember
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Initials = $"{firstName[0]}{lastName[0]}",
                Color = colors[i % colors.Length],
                Given = given,
                Received = received,
                Total = given + received
            };
        }).ToList();
    }

    private string GetColorForWord(string word)
    {
        var colors = new[] { "#3b82f6", "#10b981", "#f59e0b", "#ef4444", "#8b5cf6", "#ec4899", "#14b8a6", "#f97316" };
        return colors[Math.Abs(word.GetHashCode()) % colors.Length];
    }

    #endregion
}

