using Domain.Services;
using Domain.Providers;
using Domain.Models;
using System.Text.RegularExpressions;

namespace Cherish.RestApi.Services;

public class ContentParsingService : IContentParsingService
{
    private readonly IUserProvider _userProvider;
    private readonly IHashtagProvider _hashtagProvider;
    private readonly IPostProvider _postProvider;
    private readonly ITransactionProvider _transactionProvider;
    private readonly ILogger<ContentParsingService> _logger;

    public ContentParsingService(
        IUserProvider userProvider,
        IHashtagProvider hashtagProvider,
        IPostProvider postProvider,
        ITransactionProvider transactionProvider,
        ILogger<ContentParsingService> logger)
    {
        _userProvider = userProvider;
        _hashtagProvider = hashtagProvider;
        _postProvider = postProvider;
        _transactionProvider = transactionProvider;
        _logger = logger;
    }

    public async Task<CommentParseResult> ParseCommentContentAsync(string content, Guid companyId, Guid postId, bool postedByAdded)
    {
        var userMentioned = new List<Guid>();
        var hashtags = new List<int>();
        var totalPoints = 0;
        var parsedContent = content;

        // Get the original post to find mentioned users and post creator
        var originalPost = await _postProvider.GetPostByIdAsync(postId);
        if (originalPost == null)
        {
            throw new ArgumentException("Original post not found");
        }

        // Parse user mentions (@username) from comment content
        // TODO: Update mention system to use email or user ID instead of username
        // For now, mentions are disabled until proper implementation
        /* TEMPORARILY DISABLED - Username field removed
        var mentionMatches = Regex.Matches(content, @"@(\w+)");
        foreach (Match match in mentionMatches)
        {
            var username = match.Groups[1].Value;
            var user = await _userProvider.GetUserByEmailAsync(username);
            if (user != null && user.CompanyId == companyId)
            {
                userMentioned.Add(user.Id);
            }
        }
        */

        // If postedByAdded is true, add the post creator if not already mentioned
        if (postedByAdded && !userMentioned.Contains(originalPost.UserId))
        {
            userMentioned.Add(originalPost.UserId);
        }

        // Add users mentioned in the original post (excluding commenter)
        foreach (var mentionedUserId in originalPost.UserMentioned)
        {
            if (!userMentioned.Contains(mentionedUserId))
            {
                userMentioned.Add(mentionedUserId);
            }
        }

        // Parse hashtags (#hashtagname) from comment content
        var hashtagMatches = Regex.Matches(content, @"#([\w-]+)");
        foreach (Match match in hashtagMatches)
        {
            var hashtagName = match.Groups[1].Value;
            var hashtag = await _hashtagProvider.GetHashtagByNameAsync(hashtagName, companyId);
            if (hashtag != null)
            {
                hashtags.Add(hashtag.Id);
            }
        }

        // Parse points (+number) from comment content
        var pointMatches = Regex.Matches(content, @"\+(\d+)");
        if (pointMatches.Count == 1)
        {
            if (int.TryParse(pointMatches[0].Groups[1].Value, out var points))
            {
                totalPoints = points * userMentioned.Count;
            }
        }

        return new CommentParseResult
        {
            UserMentioned = userMentioned,
            Hashtags = hashtags,
            TotalPoints = totalPoints,
            ParsedContent = parsedContent
        };
    }

    public async Task<(List<Guid> UserMentioned, List<int> Hashtags, int TotalPoints, string ParsedContent, List<string> NotFoundUsernames)> ParsePostContentAsync(string content, Guid companyId, Guid userId)
    {
        var userMentioned = new List<Guid>();
        var hashtags = new List<int>();
        var totalPoints = 0;
        var parsedContent = content;
        var notFoundUsernames = new List<string>();

        // Parse user mentions (@username) and convert to user IDs
        // Pattern allows alphanumeric, dots, hyphens, and underscores in usernames
        // TODO: Update mention system to use email or user ID instead of username
        /* TEMPORARILY DISABLED - Username field removed
        var mentionMatches = Regex.Matches(content, @"@([\w.-]+)");
        foreach (Match match in mentionMatches)
        {
            var username = match.Groups[1].Value;
            var user = await _userProvider.GetUserByEmailAsync(username);
            if (user != null && user.CompanyId == companyId)
            {
                userMentioned.Add(user.Id);
            }
            else
            {
                _logger.LogWarning("User mention not found or not in company: @{Username}", username);
                notFoundUsernames.Add(username);
            }
        }
        */

        // Parse hashtags (#hashtagname) and convert to hashtag IDs
        // Create hashtag if it doesn't exist
        var hashtagMatches = Regex.Matches(content, @"#([\w-]+)");
        foreach (Match match in hashtagMatches)
        {
            var hashtagName = match.Groups[1].Value;
            var hashtag = await _hashtagProvider.GetHashtagByNameAsync(hashtagName, companyId);
            
            // If hashtag doesn't exist, create it
            if (hashtag == null)
            {
                try
                {
                    _logger.LogInformation("Creating new hashtag: {HashtagName} for company: {CompanyId}", hashtagName, companyId);
                    hashtag = await _hashtagProvider.CreateHashtagAsync(
                        hashtagName, 
                        $"Auto-created from post", 
                        companyId, 
                        userId);
                    _logger.LogInformation("Successfully created hashtag: {HashtagName} with ID: {HashtagId}", hashtagName, hashtag.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating hashtag: {HashtagName}", hashtagName);
                    continue; // Skip this hashtag if creation fails
                }
            }
            
            if (hashtag != null)
            {
                hashtags.Add(hashtag.Id);
            }
        }

        // Parse points (+number)
        var pointMatches = Regex.Matches(content, @"\+(\d+)");
        if (pointMatches.Count == 1)
        {
            if (int.TryParse(pointMatches[0].Groups[1].Value, out var points))
            {
                totalPoints = points * userMentioned.Count;
            }
        }

        return (userMentioned, hashtags, totalPoints, parsedContent, notFoundUsernames);
    }

    public async Task<bool> ValidateUserHasEnoughPointsAsync(Guid userId, int requiredPoints)
    {
        var user = await _userProvider.GetUserByIdAsync(userId);
        return user != null && user.AvailablePoints >= requiredPoints;
    }

    public async Task<List<Transaction>> CreatePointTransactionsAsync(Guid fromUserId, List<Guid> toUserIds, int pointsPerUser, Guid companyId, string description)
    {
        var transactions = new List<Transaction>();

        foreach (var toUserId in toUserIds)
        {
            if (fromUserId == toUserId) continue; // Skip self-transactions

            var transaction = await _transactionProvider.CreateTransactionAsync(
                companyId,
                fromUserId,
                toUserId,
                pointsPerUser,
                description
            );

            if (transaction != null)
            {
                transactions.Add(transaction);
            }
        }

        return transactions;
    }
}
