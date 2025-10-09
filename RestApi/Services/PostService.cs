using Domain.Services;
using Domain.Providers;
using Domain.Models;
using System.Text.RegularExpressions;

namespace Cherish.RestApi.Services;

public class PostService : IPostService
{
    private readonly IPostProvider _postProvider;
    private readonly IUserProvider _userProvider;
    private readonly ICompanyProvider _companyProvider;
    private readonly ICommentProvider _commentProvider;
    private readonly IReactionService _reactionService;
    private readonly IContentParsingService _contentParsingService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PostService> _logger;

    public PostService(
        IPostProvider postProvider,
        IUserProvider userProvider,
        ICompanyProvider companyProvider,
        ICommentProvider commentProvider,
        IReactionService reactionService,
        IContentParsingService contentParsingService,
        IServiceProvider serviceProvider,
        ILogger<PostService> logger)
    {
        _postProvider = postProvider;
        _userProvider = userProvider;
        _companyProvider = companyProvider;
        _commentProvider = commentProvider;
        _reactionService = reactionService;
        _contentParsingService = contentParsingService;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<Post?> GetPostByIdAsync(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Attempted to get post with empty GUID");
                return null;
            }

            return await _postProvider.GetPostByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving post with ID: {Id}", id);
            throw;
        }
    }

    public async Task<List<Post>> GetPostsByCompanyIdAsync(Guid companyId)
    {
        try
        {
            if (companyId == Guid.Empty)
            {
                _logger.LogWarning("Attempted to get posts with empty company GUID");
                return new List<Post>();
            }

            return await _postProvider.GetPostsByCompanyIdAsync(companyId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving posts for company: {CompanyId}", companyId);
            throw;
        }
    }

    public async Task<List<Post>> GetPostsByUserIdAsync(Guid userId)
    {
        try
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("Attempted to get posts with empty user GUID");
                return new List<Post>();
            }

            return await _postProvider.GetPostsByUserIdAsync(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving posts for user: {UserId}", userId);
            throw;
        }
    }

    public async Task<List<Post>> GetPostsByMentionedUserAsync(Guid userId)
    {
        try
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("Attempted to get posts with empty user GUID");
                return new List<Post>();
            }

            return await _postProvider.GetPostsByMentionedUserAsync(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving posts mentioning user: {UserId}", userId);
            throw;
        }
    }

    public async Task<List<Post>> GetPostsByHashtagIdAsync(int hashtagId)
    {
        try
        {
            if (hashtagId <= 0)
            {
                _logger.LogWarning("Attempted to get posts with invalid hashtag ID: {HashtagId}", hashtagId);
                return new List<Post>();
            }

            return await _postProvider.GetPostsByHashtagIdAsync(hashtagId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving posts for hashtag: {HashtagId}", hashtagId);
            throw;
        }
    }

    public async Task<List<Post>> GetAllPostsAsync()
    {
        try
        {
            return await _postProvider.GetAllPostsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all posts");
            throw;
        }
    }

    public async Task<PostValidationResult> ValidateAndCreatePostAsync(Guid userId, Guid companyId, string context, PostVisibility visibility)
    {
        var result = new PostValidationResult();

        try
        {
            if (string.IsNullOrEmpty(context))
            {
                result.MissingItems.Add("Context cannot be empty");
                return result;
            }

            // Parse mentions, hashtags, and points from context
            var parsedData = await _contentParsingService.ParsePostContentAsync(context, companyId);
            
            result.UserMentioned = parsedData.UserMentioned;
            result.Hashtags = parsedData.Hashtags;
            result.TotalPoints = parsedData.TotalPoints;
            result.ParsedContext = parsedData.ParsedContent;

            // Validate that poster is not in mentioned users
            if (result.UserMentioned.Contains(userId))
            {
                result.InvalidItems.Add("Poster cannot mention themselves");
            }

            // Validate mentioned users exist and are in the same company
            var missingUsers = new List<string>();
            var invalidUsers = new List<string>();
            foreach (var mentionedUserId in result.UserMentioned)
            {
                var user = await _userProvider.GetUserByIdAsync(mentionedUserId);
                if (user == null)
                {
                    missingUsers.Add(mentionedUserId.ToString());
                }
                else if (user.CompanyId != companyId)
                {
                    invalidUsers.Add(user.Username);
                }
            }

            if (missingUsers.Any())
            {
                result.MissingItems.Add($"Users not found: {string.Join(", ", missingUsers)}");
            }

            if (invalidUsers.Any())
            {
                result.InvalidItems.Add($"Users from different company: {string.Join(", ", invalidUsers)}");
            }

            // Note: Hashtags are already validated during parsing in ContentParsingService

            // Validate points format (only one +points mention allowed)
            var pointMatches = Regex.Matches(context, @"\+\d+");
            if (pointMatches.Count > 1)
            {
                result.InvalidItems.Add("Only one +points mention is allowed");
            }
            else if (pointMatches.Count == 0)
            {
                result.MissingItems.Add("Must include +points (e.g., +10)");
            }

            // Check if user has enough points
            var poster = await _userProvider.GetUserByIdAsync(userId);
            if (poster == null)
            {
                result.MissingItems.Add("Poster user not found");
                return result;
            }

            if (poster.AvailablePoints < result.TotalPoints)
            {
                result.InvalidItems.Add($"Insufficient points. Available: {poster.AvailablePoints}, Required: {result.TotalPoints}");
            }

            // Check if there are any validation errors
            if (result.MissingItems.Any() || result.InvalidItems.Any())
            {
                result.IsValid = false;
                return result;
            }

            // Create the post
            var metadata = CreateMetadata(parsedData);
            var post = await _postProvider.CreatePostAsync(
                userId, companyId, result.ParsedContext, result.UserMentioned, 
                result.Hashtags, metadata, result.TotalPoints, visibility);

            // Create transactions for mentioned users
            if (result.UserMentioned.Any())
            {
                var pointsPerUser = result.TotalPoints / result.UserMentioned.Count;
                await _contentParsingService.CreatePointTransactionsAsync(
                    userId,
                    result.UserMentioned,
                    pointsPerUser,
                    companyId,
                    $"Points from post: {post.Id}"
                );
            }

            result.IsValid = true;
            result.Post = post;
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating and creating post for user: {UserId}", userId);
            result.MissingItems.Add("An error occurred during post creation");
            return result;
        }
    }


    private string CreateMetadata((List<Guid> UserMentioned, List<int> Hashtags, int TotalPoints, string ParsedContext) parsedData)
    {
        var metadata = new
        {
            ParsedUserMentions = parsedData.UserMentioned.Count,
            ParsedHashtags = parsedData.Hashtags.Count,
            TotalPointsCalculated = parsedData.TotalPoints,
            OriginalContextLength = parsedData.ParsedContext.Length
        };

        return System.Text.Json.JsonSerializer.Serialize(metadata);
    }

    public async Task<bool> SoftDeletePostAsync(Guid id, Guid userId)
    {
        try
        {
            // Get existing post to verify ownership
            var existingPost = await _postProvider.GetPostByIdAsync(id);
            if (existingPost == null)
            {
                _logger.LogWarning("Post not found: {PostId}", id);
                return false;
            }

            // Validate user owns the post
            if (existingPost.UserId != userId)
            {
                _logger.LogWarning("User {UserId} does not own post {PostId}", userId, id);
                return false;
            }

            // Note: No need to revert transactions as per requirements
            return await _postProvider.SoftDeletePostAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error soft deleting post: {PostId}", id);
            return false;
        }
    }

    public async Task<Post?> UpdatePostContentAsync(Guid id, string content, Guid userId)
    {
        try
        {
            // Get existing post to verify ownership
            var existingPost = await _postProvider.GetPostByIdAsync(id);
            if (existingPost == null)
            {
                _logger.LogWarning("Post not found: {PostId}", id);
                return null;
            }

            // Validate user owns the post
            if (existingPost.UserId != userId)
            {
                _logger.LogWarning("User {UserId} does not own post {PostId}", userId, id);
                return null;
            }

            // Validation: Only content can be modified, mentions and points remain the same
            if (string.IsNullOrEmpty(content))
            {
                _logger.LogWarning("Content cannot be empty for post: {PostId}", id);
                return null;
            }

            // Parse the new content to extract mentions and points
            var newContentParse = await _contentParsingService.ParsePostContentAsync(content, existingPost.CompanyId);

            // Validate that mentions haven't changed (order doesn't matter)
            var originalMentionsSorted = existingPost.UserMentioned.OrderBy(x => x).ToList();
            var newMentionsSorted = newContentParse.UserMentioned.OrderBy(x => x).ToList();
            if (!originalMentionsSorted.SequenceEqual(newMentionsSorted))
            {
                _logger.LogWarning("User mentions cannot be changed for post: {PostId}. Original: {OriginalMentions}, New: {NewMentions}", 
                    id, string.Join(",", existingPost.UserMentioned), string.Join(",", newContentParse.UserMentioned));
                return null;
            }

            // Validate that total points haven't changed
            if (newContentParse.TotalPoints != existingPost.TotalPoints)
            {
                _logger.LogWarning("Points cannot be changed for post: {PostId}. Original: {OriginalPoints}, New: {NewPoints}", 
                    id, existingPost.TotalPoints, newContentParse.TotalPoints);
                return null;
            }

            // Note: Hashtags can be modified, so we don't validate them

            // Update content and hashtags, mentions and points remain unchanged
            return await _postProvider.UpdatePostContentAsync(id, content, newContentParse.Hashtags);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating post content: {PostId}", id);
            return null;
        }
    }

    public async Task<PostWithDetails?> GetPostWithDetailsAsync(Guid postId, Guid? userId = null)
    {
        try
        {
            // Get the post first (required for validation)
            var post = await _postProvider.GetPostByIdAsync(postId);
            if (post == null)
            {
                _logger.LogWarning("Post not found: {PostId}", postId);
                return null;
            }

            // Create tasks for parallel execution of independent operations
            var commentsTask = _commentProvider.GetCommentsByPostIdWithPaginationAsync(postId, 1, 2);
            var reactionCountsTask = _reactionService.GetReactionCountsByPostIdAsync(postId);
            
            // Create user reaction task only if userId is provided
            var userReactionTask = userId.HasValue 
                ? _reactionService.GetUserReactionForPostAsync(userId.Value, postId)
                : Task.FromResult<Reaction?>(null);

            // Wait for all tasks to complete in parallel
            await Task.WhenAll(commentsTask, reactionCountsTask, userReactionTask);

            // Extract results
            var latestComments = await commentsTask;
            var reactionCounts = await reactionCountsTask;
            var userReaction = await userReactionTask;

            return new PostWithDetails
            {
                Post = post,
                LatestComments = latestComments,
                ReactionCounts = reactionCounts,
                UserReaction = userReaction
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting post with details: {PostId}", postId);
            return null;
        }
    }

    public async Task<PaginatedPostResult> GetFilteredPostsWithDetailsAsync(
        Guid userId,
        PostFilterRequest filterRequest)
    {
        try
        {
            // Get user to validate company
            var user = await _userProvider.GetUserByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", userId);
                return new PaginatedPostResult();
            }

            // Resolve hashtag names to IDs if provided
            List<int>? hashtagIds = null;
            if (filterRequest.HashtagNames != null && filterRequest.HashtagNames.Any())
            {
                hashtagIds = await ResolveHashtagNamesToIds(filterRequest.HashtagNames, user.CompanyId);
            }
            else if (filterRequest.HashtagIds != null && filterRequest.HashtagIds.Any())
            {
                hashtagIds = filterRequest.HashtagIds;
            }

            // Decode cursor if provided
            CursorInfo? cursor = null;
            if (!string.IsNullOrEmpty(filterRequest.Cursor))
            {
                cursor = DecodeCursor(filterRequest.Cursor);
            }

            // Get filtered posts from provider
            var (posts, hasNextPage) = await _postProvider.GetFilteredPostsAsync(
                user.CompanyId,
                filterRequest.PageSize,
                cursor,
                filterRequest.FilterByTeam,
                filterRequest.FilterByUserId ?? userId,
                hashtagIds,
                filterRequest.SortOrder
            );

            // Calculate pagination info
            var hasPreviousPage = cursor != null;

            // Get details for each post in parallel
            var postDetailsTasks = posts.Select(async post =>
            {
                // Create tasks for parallel execution
                var commentsTask = _commentProvider.GetCommentsByPostIdWithPaginationAsync(post.Id, 1, 2);
                var reactionCountsTask = _reactionService.GetReactionCountsByPostIdAsync(post.Id);
                var userReactionTask = _reactionService.GetUserReactionForPostAsync(userId, post.Id);

                // Wait for all tasks to complete
                await Task.WhenAll(commentsTask, reactionCountsTask, userReactionTask);

                // Extract results
                var latestComments = await commentsTask;
                var reactionCounts = await reactionCountsTask;
                var userReaction = await userReactionTask;

                return new PostWithDetails
                {
                    Post = post,
                    LatestComments = latestComments,
                    ReactionCounts = reactionCounts,
                    UserReaction = userReaction
                };
            });

            // Wait for all post details to complete
            var postDetails = await Task.WhenAll(postDetailsTasks);

            // Generate cursors for next and previous pages
            string? nextCursor = null;
            string? previousCursor = null;

            if (hasNextPage && postDetails.Any())
            {
                var lastPost = postDetails.Last();
                nextCursor = EncodeCursor(lastPost.Post.CreatedAt, lastPost.Post.Id);
            }

            if (hasPreviousPage && posts.Any())
            {
                var firstPost = posts.First();
                previousCursor = EncodeCursor(firstPost.CreatedAt, firstPost.Id);
            }

            return new PaginatedPostResult
            {
                Posts = postDetails.ToList(),
                PageSize = filterRequest.PageSize,
                NextCursor = nextCursor,
                PreviousCursor = previousCursor,
                HasNextPage = hasNextPage,
                HasPreviousPage = hasPreviousPage
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting filtered posts with details for user: {UserId}", userId);
            return new PaginatedPostResult();
        }
    }

    private async Task<List<int>> ResolveHashtagNamesToIds(List<string> hashtagNames, Guid companyId)
    {
        try
        {
            var hashtagProvider = _serviceProvider.GetService<IHashtagProvider>();
            if (hashtagProvider == null)
            {
                _logger.LogWarning("HashtagProvider not available");
                return new List<int>();
            }

            var hashtagIds = new List<int>();
            foreach (var hashtagName in hashtagNames)
            {
                var hashtag = await hashtagProvider.GetHashtagByNameAsync(hashtagName, companyId);
                if (hashtag != null)
                {
                    hashtagIds.Add(hashtag.Id);
                }
            }
            return hashtagIds;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resolving hashtag names to IDs: {HashtagNames}", string.Join(", ", hashtagNames));
            return new List<int>();
        }
    }

    private string EncodeCursor(DateTime createdAt, Guid postId)
    {
        try
        {
            var cursorData = new CursorInfo
            {
                CreatedAt = createdAt,
                PostId = postId
            };
            
            var json = System.Text.Json.JsonSerializer.Serialize(cursorData);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            return Convert.ToBase64String(bytes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error encoding cursor");
            throw new ArgumentException("Invalid cursor data");
        }
    }

    private CursorInfo? DecodeCursor(string cursor)
    {
        try
        {
            var bytes = Convert.FromBase64String(cursor);
            var json = System.Text.Encoding.UTF8.GetString(bytes);
            return System.Text.Json.JsonSerializer.Deserialize<CursorInfo>(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error decoding cursor: {Cursor}", cursor);
            throw new ArgumentException("Invalid cursor format");
        }
    }
}
