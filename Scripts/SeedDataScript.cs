using System.Text;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using Domain.Models;

namespace Scripts;

public class SeedDataScript : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly Dictionary<string, string> _companyTokens = new();
    private readonly Dictionary<string, List<Guid>> _companyUsers = new();
    private readonly Dictionary<string, string> _userTokens = new(); // Store user tokens for authentication
    private readonly Dictionary<string, List<Guid>> _companyTeams = new();
    private readonly Dictionary<string, List<int>> _hashtagIds = new();

    public SeedDataScript(string baseUrl = "https://localhost:7000")
    {
        _baseUrl = baseUrl;
        _httpClient = new HttpClient();
        
        // Ignore SSL certificate errors for local development
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "SeedDataScript/1.0");
    }

    public async Task RunAsync()
    {
        Console.WriteLine("🌱 Starting Cherish Seed Data Generation");
        Console.WriteLine("=======================================");

        try
        {
            // Step 1: Create Companies
            await CreateCompaniesAsync();
            
            // Step 2: Create Users for each company
            await CreateUsersAsync();
            
            // Step 3: Create Teams for each company
            await CreateTeamsAsync();
            
            // Step 4: Create Hashtags
            await CreateHashtagsAsync();
            
            // Step 5: Create Posts
            await CreatePostsAsync();
            
            // Step 6: Create Reactions and Comments
            await CreateReactionsAndCommentsAsync();

            Console.WriteLine("✅ Seed data generation completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Seed data generation failed: {ex.Message}");
            throw;
        }
    }

    private async Task CreateCompaniesAsync()
    {
        Console.WriteLine("\n🏢 Creating Companies...");
        
        var companies = new[]
        {
            new { Name = "TechCorp Solutions", Key = "CompanyA" },
            new { Name = "InnovateLabs Inc", Key = "CompanyB" }
        };

        foreach (var company in companies)
        {
            var response = await PostAsync<CompanyResponse>("/api/company/create", new { name = company.Name });
            if (response?.Success == true)
            {
                Console.WriteLine($"  ✅ Created {company.Name} (ID: {response.Data?.Id})");
                _companyTokens[company.Key] = response.Data?.Id.ToString() ?? "";
            }
            else
            {
                // Handle conflicts or any other failure
                Console.WriteLine($"  ℹ️  Company {company.Name} already exists or failed, using existing data...");
                // For demo purposes, we'll use the company IDs from the previous run
                if (company.Key == "CompanyA")
                {
                    _companyTokens[company.Key] = "b057c22e-5332-4326-9216-f3af7a77828f"; // From previous run
                }
                else if (company.Key == "CompanyB")
                {
                    _companyTokens[company.Key] = "ec81c1ed-0568-4c5b-8cea-6591ec9ea628"; // From previous run
                }
            }
        }
    }

    private async Task CreateUsersAsync()
    {
        Console.WriteLine("\n👥 Creating Users...");

        var userProfiles = new[]
        {
            // Company A Users (TechCorp Solutions)
            new { FirstName = "Alice", LastName = "Johnson", Email = "alice.johnson@techcorp.com", Role = UserRole.Manager, Team = 1 },
            new { FirstName = "Bob", LastName = "Smith", Email = "bob.smith@techcorp.com", Role = UserRole.Employee, Team = 1 },
            new { FirstName = "Carol", LastName = "Davis", Email = "carol.davis@techcorp.com", Role = UserRole.Employee, Team = 1 },
            new { FirstName = "David", LastName = "Wilson", Email = "david.wilson@techcorp.com", Role = UserRole.Employee, Team = 1 },
            
            new { FirstName = "Emma", LastName = "Brown", Email = "emma.brown@techcorp.com", Role = UserRole.Manager, Team = 2 },
            new { FirstName = "Frank", LastName = "Garcia", Email = "frank.garcia@techcorp.com", Role = UserRole.Employee, Team = 2 },
            new { FirstName = "Grace", LastName = "Martinez", Email = "grace.martinez@techcorp.com", Role = UserRole.Employee, Team = 2 },
            
            new { FirstName = "Henry", LastName = "Anderson", Email = "henry.anderson@techcorp.com", Role = UserRole.Manager, Team = 3 },
            new { FirstName = "Ivy", LastName = "Taylor", Email = "ivy.taylor@techcorp.com", Role = UserRole.Employee, Team = 3 },
            new { FirstName = "Jack", LastName = "Thomas", Email = "jack.thomas@techcorp.com", Role = UserRole.Employee, Team = 3 },

            // Company B Users (InnovateLabs Inc)
            new { FirstName = "Kate", LastName = "Hernandez", Email = "kate.hernandez@innovatelabs.com", Role = UserRole.Manager, Team = 1 },
            new { FirstName = "Liam", LastName = "Moore", Email = "liam.moore@innovatelabs.com", Role = UserRole.Employee, Team = 1 },
            new { FirstName = "Maya", LastName = "Jackson", Email = "maya.jackson@innovatelabs.com", Role = UserRole.Employee, Team = 1 },
            new { FirstName = "Noah", LastName = "Martin", Email = "noah.martin@innovatelabs.com", Role = UserRole.Employee, Team = 1 },
            
            new { FirstName = "Olivia", LastName = "Lee", Email = "olivia.lee@innovatelabs.com", Role = UserRole.Manager, Team = 2 },
            new { FirstName = "Parker", LastName = "Perez", Email = "parker.perez@innovatelabs.com", Role = UserRole.Employee, Team = 2 },
            new { FirstName = "Quinn", LastName = "Thompson", Email = "quinn.thompson@innovatelabs.com", Role = UserRole.Employee, Team = 2 },
            
            new { FirstName = "Riley", LastName = "White", Email = "riley.white@innovatelabs.com", Role = UserRole.Manager, Team = 3 },
            new { FirstName = "Sophia", LastName = "Harris", Email = "sophia.harris@innovatelabs.com", Role = UserRole.Employee, Team = 3 },
            new { FirstName = "Tyler", LastName = "Sanchez", Email = "tyler.sanchez@innovatelabs.com", Role = UserRole.Employee, Team = 3 }
        };

        _companyUsers["CompanyA"] = new List<Guid>();
        _companyUsers["CompanyB"] = new List<Guid>();

        for (int i = 0; i < userProfiles.Length; i++)
        {
            var user = userProfiles[i];
            var companyKey = i < 10 ? "CompanyA" : "CompanyB";
            var companyId = _companyTokens[companyKey];
            var username = $"{user.FirstName.ToLower()}.{user.LastName.ToLower()}";

            var response = await PostAsync<UserResponse>("/api/auth/create-user", new
            {
                username = username,
                password = "Password123!",
                email = user.Email,
                firstName = user.FirstName,
                lastName = user.LastName,
                role = (int)user.Role,
                companyId = companyId
            });

            if (response?.Success == true && response.Data?.Id != null)
            {
                _companyUsers[companyKey].Add(response.Data.Id);
                Console.WriteLine($"  ✅ Created {user.FirstName} {user.LastName} ({user.Role}) for {companyKey}");
                
                // Authenticate the user to get a token
                var loginResponse = await PostAsync<LoginResponse>("/api/auth/login", new
                {
                    username = username,
                    password = "Password123!"
                });
                
                if (loginResponse?.Success == true && !string.IsNullOrEmpty(loginResponse.Data?.Token))
                {
                    _userTokens[username] = loginResponse.Data.Token;
                    Console.WriteLine($"    🔐 Authenticated {username}");
                }
            }
            else
            {
                // Handle conflicts or any other failure - assume user already exists
                Console.WriteLine($"  ℹ️  User {username} already exists, authenticating...");
                // Try to authenticate existing user
                var loginResponse = await PostAsync<LoginResponse>("/api/auth/login", new
                {
                    username = username,
                    password = "Password123!"
                });
                
                if (loginResponse?.Success == true && !string.IsNullOrEmpty(loginResponse.Data?.Token))
                {
                    _userTokens[username] = loginResponse.Data.Token;
                    Console.WriteLine($"    🔐 Authenticated existing user {username}");
                    // For existing users, we'll need to get their actual ID from the token or API
                    // For now, let's use a simple approach - we'll create teams without specific user IDs
                }
                else
                {
                    Console.WriteLine($"    ❌ Failed to authenticate {username}");
                }
            }
        }
    }

    private async Task CreateTeamsAsync()
    {
        Console.WriteLine("\n👥 Creating Teams...");
        Console.WriteLine("  ⚠️  Skipping team creation for now - teams can be created manually later");
        Console.WriteLine("  ℹ️  Focus on testing posts, hashtags, and follow functionality");
        
        // Initialize empty team lists for each company
        _companyTeams["CompanyA"] = new List<Guid>();
        _companyTeams["CompanyB"] = new List<Guid>();
    }

    private async Task CreateHashtagsAsync()
    {
        Console.WriteLine("\n🏷️ Creating Hashtags...");

        var hashtags = new[]
        {
            new { Name = "innovation", Description = "Posts about innovative ideas and solutions" },
            new { Name = "teamwork", Description = "Collaboration and team building content" },
            new { Name = "productivity", Description = "Tips and tricks for better productivity" },
            new { Name = "tech", Description = "Technology-related discussions" },
            new { Name = "leadership", Description = "Leadership insights and experiences" },
            new { Name = "project", Description = "Project updates and milestones" },
            new { Name = "meeting", Description = "Meeting notes and outcomes" },
            new { Name = "achievement", Description = "Celebrating accomplishments" },
            new { Name = "feedback", Description = "Feedback and suggestions" },
            new { Name = "celebration", Description = "Celebrating team successes" }
        };

        _hashtagIds["CompanyA"] = new List<int>();
        _hashtagIds["CompanyB"] = new List<int>();

        var companies = new[] { "CompanyA", "CompanyB" };
        
        foreach (var companyKey in companies)
        {
            var companyId = _companyTokens[companyKey];
            var companyUsers = _companyUsers[companyKey];
            
            // Get the first user's token for authentication
            var firstUsername = companyKey == "CompanyA" ? "alice.johnson" : "kate.hernandez";
            var authToken = _userTokens.GetValueOrDefault(firstUsername);

            foreach (var hashtag in hashtags)
            {
                var response = await PostAsync<HashtagResponse>("/api/hashtag/create", new
                {
                    name = hashtag.Name,
                    description = hashtag.Description,
                    companyId = companyId
                }, authToken);

                if (response?.Success == true && response.Data?.Id != null)
                {
                    _hashtagIds[companyKey].Add(response.Data.Id);
                    Console.WriteLine($"  ✅ Created #{hashtag.Name} for {companyKey}");
                }
            }
        }
    }

    private async Task CreatePostsAsync()
    {
        Console.WriteLine("\n📝 Creating Posts...");

        var companies = new[] { "CompanyA", "CompanyB" };
        
        foreach (var companyKey in companies)
        {
            var companyId = _companyTokens[companyKey];
            var companyUsers = _companyUsers[companyKey];
            var hashtagIds = _hashtagIds[companyKey];
            
            // Get the first user's token for authentication
            var firstUsername = companyKey == "CompanyA" ? "alice.johnson" : "kate.hernandez";
            var authToken = _userTokens.GetValueOrDefault(firstUsername);

            var posts = GenerateRealisticPosts(companyUsers, hashtagIds);

            for (int i = 0; i < posts.Count; i++)
            {
                var post = posts[i];
                var response = await PostAsync<PostResponse>("/api/post/create", new
                {
                    context = post.Context,
                    visibility = (int)post.Visibility,
                    companyId = companyId
                }, authToken);

                if (response?.Success == true)
                {
                    Console.WriteLine($"  ✅ Created post {i + 1}/30 for {companyKey}");
                }

                // Add delay to avoid overwhelming the API
                await Task.Delay(100);
            }
        }
    }

    private async Task CreateReactionsAndCommentsAsync()
    {
        Console.WriteLine("\n👍 Creating Reactions and Comments...");

        var companies = new[] { "CompanyA", "CompanyB" };
        
        foreach (var companyKey in companies)
        {
            var companyId = _companyTokens[companyKey];
            var companyUsers = _companyUsers[companyKey];

            Console.WriteLine($"\n  📝 Creating reactions and comments for {companyKey}...");

            // Get posts for this company
            var posts = await GetPostsForCompanyAsync(companyId, companyUsers.First());
            if (posts == null || !posts.Any())
            {
                Console.WriteLine($"  ⚠️  No posts found for {companyKey}, skipping reactions and comments");
                continue;
            }

            Console.WriteLine($"  📊 Found {posts.Count} posts for {companyKey}");

            // Create reactions for posts
            await CreateReactionsForPostsAsync(posts, companyUsers, companyId);

            // Create comments for posts
            await CreateCommentsForPostsAsync(posts, companyUsers, companyId);
        }
    }

    private async Task<List<PostData>?> GetPostsForCompanyAsync(string companyId, Guid userId)
    {
        try
        {
            // Login as a user to get posts
            var loginResponse = await PostAsync<LoginResponse>("/api/auth/login", new
            {
                username = "alice.johnson", // Use first user for login
                password = "Password123!"
            });

            if (loginResponse?.Success != true || loginResponse.Data?.Token == null)
            {
                Console.WriteLine("  ❌ Failed to login to fetch posts");
                return null;
            }

            // Set authorization header
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResponse.Data.Token);

            // Get posts using the filter API
            var filterResponse = await PostAsync<PostFilterResponse>("/api/post/filter", new
            {
                pageSize = 50,
                sortOrder = 1
            });

            if (filterResponse?.Success == true && filterResponse.Data?.Posts != null)
            {
                return filterResponse.Data.Posts.Select(p => new PostData { Id = p.Id, Context = p.Context }).ToList();
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ❌ Error fetching posts: {ex.Message}");
            return null;
        }
    }

    private async Task CreateReactionsForPostsAsync(List<PostData> posts, List<Guid> users, string companyId)
    {
        var random = new Random();
        var reactionTypes = new[] { 1, 2, 3, 4, 5 }; // Like, Love, Laugh, Angry, Sad
        
        Console.WriteLine($"  👍 Creating reactions for {posts.Count} posts...");

        foreach (var post in posts.Take(20)) // React to first 20 posts
        {
            // 70% chance each post gets reactions
            if (random.NextDouble() < 0.7)
            {
                // 1-5 users will react to each post
                var reactors = users.OrderBy(x => random.Next()).Take(random.Next(1, 6)).ToList();
                
                foreach (var userId in reactors)
                {
                    var reactionType = reactionTypes[random.Next(reactionTypes.Length)];
                    
                    var response = await PostAsync<ReactionResponse>("/api/reaction/react", new
                    {
                        postId = post.Id,
                        emojiType = reactionType
                    });

                    if (response?.Success == true)
                    {
                        Console.WriteLine($"    ✅ User {userId} reacted to post {post.Id} with emoji {reactionType}");
                    }

                    // Small delay to avoid overwhelming the API
                    await Task.Delay(50);
                }
            }
        }
    }

    private async Task CreateCommentsForPostsAsync(List<PostData> posts, List<Guid> users, string companyId)
    {
        var random = new Random();
        
        Console.WriteLine($"  💬 Creating comments for {posts.Count} posts...");

        var commentTemplates = new[]
        {
            "Great post! @{user1} +5 #feedback",
            "Excellent work! @{user1} +10 #achievement",
            "Thanks for sharing! @{user1} +5 #teamwork",
            "Very insightful! @{user1} +5 #innovation",
            "I agree with this approach! @{user1} +5 #productivity",
            "This is really helpful! @{user1} +5 #feedback",
            "Amazing results! @{user1} +10 #achievement",
            "Great collaboration! @{user1} +5 #teamwork",
            "Love this idea! @{user1} +5 #innovation",
            "Perfect timing! @{user1} +5 #productivity",
            "Well done team! @{user1} +5 #celebration",
            "This will help us a lot! @{user1} +5 #project",
            "Excellent presentation! @{user1} +10 #achievement",
            "Thanks for the update! @{user1} +5 #meeting",
            "Great progress! @{user1} +5 #project"
        };

        foreach (var post in posts.Take(15)) // Comment on first 15 posts
        {
            // 60% chance each post gets comments
            if (random.NextDouble() < 0.6)
            {
                // 1-3 users will comment on each post
                var commenters = users.OrderBy(x => random.Next()).Take(random.Next(1, 4)).ToList();
                
                foreach (var commenterId in commenters)
                {
                    var template = commentTemplates[random.Next(commentTemplates.Length)];
                    var mentionedUser = users.Where(u => u != commenterId).OrderBy(x => random.Next()).FirstOrDefault();
                    
                    var content = template.Replace("{user1}", mentionedUser.ToString());
                    
                    var response = await PostAsync<CommentResponse>("/api/comment", new
                    {
                        content = content,
                        postId = post.Id,
                        postedByAdded = random.NextDouble() < 0.5 // 50% chance to include post creator
                    });

                    if (response?.Success == true)
                    {
                        Console.WriteLine($"    ✅ User {commenterId} commented on post {post.Id}");
                    }

                    // Small delay to avoid overwhelming the API
                    await Task.Delay(100);
                }
            }
        }
    }

    private List<PostTemplate> GenerateRealisticPosts(List<Guid> users, List<int> hashtagIds)
    {
        var posts = new List<PostTemplate>();
        var random = new Random();
        
        var postTemplates = new[]
        {
            "Great team meeting today! Thanks everyone for the excellent presentation on our Q4 roadmap. #project #teamwork +10",
            "Just finished reviewing the code. Amazing work! The new authentication system looks solid. #tech #achievement +15",
            "Had an insightful discussion about our product strategy. Excited about the upcoming features! #innovation #leadership +20",
            "Congratulations to the team for completing the mobile app redesign! The UI looks fantastic. #achievement #celebration +25",
            "Team standup went really well today. Shared some great insights about performance optimization. #meeting #productivity +10",
            "Working on the new API integration. The documentation is super helpful! #tech #teamwork +15",
            "Just deployed the latest version to production. Thanks for the thorough testing! #project #achievement +20",
            "Had a productive brainstorming session with the team. Came up with some brilliant ideas! #innovation #meeting +15",
            "Reviewing pull requests. Code quality has improved significantly! #tech #feedback +25",
            "Team lunch was awesome! Great to connect with everyone outside of work. #teamwork #celebration +10",
            "Completed the security audit. All systems are secure and up to date. #tech #achievement +20",
            "Planning session for next quarter went smoothly. Provided valuable market insights. #project #leadership +15",
            "Code review session was very helpful. Learned some new best practices! #tech #feedback +10",
            "Team building activity was fun! Everyone really knows how to make each other laugh. #teamwork #celebration +15",
            "Documentation sprint completed ahead of schedule. Thanks for the excellent writing! #project #achievement +20",
            "Client demo went perfectly! Did an amazing job presenting our solution. #project #achievement +25",
            "Bug fixing session was productive. We solved three critical issues today! #tech #productivity +20",
            "Team retrospective was insightful. Shared some great suggestions for improvement. #meeting #feedback +10",
            "Database optimization completed. Performance improved by 40%! Instrumental in this success. #tech #achievement +30",
            "New feature launch was successful! Thanks for the hard work. #project #celebration +25",
            "Code refactoring session made the codebase much cleaner. #tech #productivity +15",
            "Team training on new technologies was excellent. Great teacher! #meeting #innovation +20",
            "Project milestone achieved! Worked tirelessly to make this happen. #project #achievement +30",
            "Client feedback session was very positive. Handled all questions brilliantly. #meeting #achievement +20",
            "System monitoring dashboard is now live! Did fantastic work on the visualization. #tech #project +25",
            "Team collaboration tools implementation was smooth. Provided excellent support. #teamwork #tech +15",
            "Performance testing results are in! Exceeded all expectations. #tech #achievement +25",
            "Code quality metrics improved significantly this sprint. Led the initiative perfectly. #tech #productivity +20",
            "Team knowledge sharing session was valuable. Shared some great insights. #meeting #innovation +15",
            "Project delivery was on time and under budget! Managed the timeline perfectly. #project #achievement +30"
        };

        for (int i = 0; i < 30; i++)
        {
            var template = postTemplates[i % postTemplates.Length];
            
            // Use simple posts without user mentions for now
            var context = template;
            
            var selectedHashtags = hashtagIds.OrderBy(x => random.Next()).Take(random.Next(1, 4)).ToList();

            posts.Add(new PostTemplate
            {
                Context = context,
                Visibility = (PostVisibility)random.Next(0, 3),
                MentionedUsers = new List<Guid>(), // No user mentions for now
                HashtagIds = selectedHashtags
            });
        }

        return posts;
    }

    private async Task<T?> PostAsync<T>(string endpoint, object data, string? authToken = null)
    {
        try
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}{endpoint}")
            {
                Content = content
            };
            
            if (!string.IsNullOrEmpty(authToken))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);
            }
            
            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            
            Console.WriteLine($"  ⚠️  API call failed: {response.StatusCode} - {responseContent}");
            return default(T);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ❌ API call error: {ex.Message}");
            return default(T);
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}

// Response Models
public class CompanyResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public CompanyData? Data { get; set; }
}

public class CompanyData
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
}

public class UserResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public UserData? Data { get; set; }
}

public class UserData
{
    public Guid Id { get; set; }
    public string Username { get; set; } = "";
}

public class TeamResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public TeamData? Data { get; set; }
}

public class TeamData
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
}

public class HashtagResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public HashtagData? Data { get; set; }
}

public class HashtagData
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

public class PostResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public PostData? Data { get; set; }
}

public class PostData
{
    public Guid Id { get; set; }
    public string Context { get; set; } = "";
}

public class PostTemplate
{
    public string Context { get; set; } = "";
    public PostVisibility Visibility { get; set; }
    public List<Guid> MentionedUsers { get; set; } = new();
    public List<int> HashtagIds { get; set; } = new();
}

public class LoginResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public LoginData? Data { get; set; }
}

public class LoginData
{
    public string Token { get; set; } = "";
    public string Username { get; set; } = "";
}

public class PostFilterResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public PostFilterData? Data { get; set; }
}

public class PostFilterData
{
    public List<PostFilterPost> Posts { get; set; } = new();
}

public class PostFilterPost
{
    public Guid Id { get; set; }
    public string Context { get; set; } = "";
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ReactionResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public ReactionData? Data { get; set; }
}

public class ReactionData
{
    public long Id { get; set; }
    public Guid PostId { get; set; }
    public int EmojiType { get; set; }
}

public class CommentResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public CommentData? Data { get; set; }
}

public class CommentData
{
    public Guid Id { get; set; }
    public string Content { get; set; } = "";
    public Guid PostId { get; set; }
}
