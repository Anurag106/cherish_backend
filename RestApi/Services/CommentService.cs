using Domain.Services;
using Domain.Providers;
using Domain.Models;

namespace Cherish.RestApi.Services;

public class CommentService : ICommentService
{
    private readonly ICommentProvider _commentProvider;
    private readonly IUserProvider _userProvider;
    private readonly ICompanyProvider _companyProvider;
    private readonly IPostProvider _postProvider;
    private readonly IContentParsingService _contentParsingService;
    private readonly ILogger<CommentService> _logger;

    public CommentService(
        ICommentProvider commentProvider,
        IUserProvider userProvider,
        ICompanyProvider companyProvider,
        IPostProvider postProvider,
        IContentParsingService contentParsingService,
        ILogger<CommentService> logger)
    {
        _commentProvider = commentProvider;
        _userProvider = userProvider;
        _companyProvider = companyProvider;
        _postProvider = postProvider;
        _contentParsingService = contentParsingService;
        _logger = logger;
    }

    public async Task<Comment?> GetCommentByIdAsync(Guid id)
    {
        try
        {
            return await _commentProvider.GetCommentByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting comment by ID: {CommentId}", id);
            return null;
        }
    }

    public async Task<List<Comment>> GetCommentsByPostIdAsync(Guid postId)
    {
        try
        {
            return await _commentProvider.GetCommentsByPostIdAsync(postId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting comments by post ID: {PostId}", postId);
            return new List<Comment>();
        }
    }

    public async Task<List<Comment>> GetCommentsByUserIdAsync(Guid userId)
    {
        try
        {
            return await _commentProvider.GetCommentsByUserIdAsync(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting comments by user ID: {UserId}", userId);
            return new List<Comment>();
        }
    }

    public async Task<List<Comment>> GetCommentsByCompanyIdAsync(Guid companyId)
    {
        try
        {
            return await _commentProvider.GetCommentsByCompanyIdAsync(companyId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting comments by company ID: {CompanyId}", companyId);
            return new List<Comment>();
        }
    }

    public async Task<Comment?> CreateCommentAsync(Guid userId, bool postedByAdded, Guid companyId, string content, Guid postId)
    {
        try
        {
            // Validate user exists
            var user = await _userProvider.GetUserByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", userId);
                return null;
            }

            // Validate company exists
            var company = await _companyProvider.GetCompanyByIdAsync(companyId);
            if (company == null)
            {
                _logger.LogWarning("Company not found: {CompanyId}", companyId);
                return null;
            }

            // Validate post exists
            var post = await _postProvider.GetPostByIdAsync(postId);
            if (post == null)
            {
                _logger.LogWarning("Post not found: {PostId}", postId);
                return null;
            }

            // Validate post belongs to same company
            if (post.CompanyId != companyId)
            {
                _logger.LogWarning("Post does not belong to company: {PostId}, {CompanyId}", postId, companyId);
                return null;
            }

            // Parse content for mentions, hashtags, and points
            var parseResult = await _contentParsingService.ParseCommentContentAsync(content, companyId, postId, postedByAdded);

            // Validate user has enough points
            if (parseResult.TotalPoints > 0)
            {
                var hasEnoughPoints = await _contentParsingService.ValidateUserHasEnoughPointsAsync(userId, parseResult.TotalPoints);
                if (!hasEnoughPoints)
                {
                    _logger.LogWarning("User does not have enough points: {UserId}, Required: {RequiredPoints}", userId, parseResult.TotalPoints);
                    return null;
                }

                // Create transactions for point allocation
                if (parseResult.UserMentioned.Any())
                {
                    var pointsPerUser = parseResult.TotalPoints / parseResult.UserMentioned.Count;
                    await _contentParsingService.CreatePointTransactionsAsync(
                        userId,
                        parseResult.UserMentioned,
                        pointsPerUser,
                        companyId,
                        $"Points from comment on post {postId}"
                    );
                }
            }

            // Create the comment
            var comment = await _commentProvider.CreateCommentAsync(
                userId,
                postedByAdded,
                companyId,
                parseResult.ParsedContent,
                parseResult.TotalPoints,
                postId,
                parseResult.Hashtags,
                "{}" // Default empty metadata
            );

            _logger.LogInformation("Comment created successfully: {CommentId} by user {UserId}", comment.Id, userId);
            return comment;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating comment for user: {UserId}", userId);
            return null;
        }
    }

    public async Task<Comment?> UpdateCommentContentAsync(Guid id, string content, Guid userId)
    {
        try
        {
            // Get existing comment
            var existingComment = await _commentProvider.GetCommentByIdAsync(id);
            if (existingComment == null)
            {
                _logger.LogWarning("Comment not found: {CommentId}", id);
                return null;
            }

            // Validate user owns the comment
            if (existingComment.UserId != userId)
            {
                _logger.LogWarning("User {UserId} does not own comment {CommentId}", userId, id);
                return null;
            }

            // Validation: Only content can be modified, mentions and points remain the same
            if (string.IsNullOrEmpty(content))
            {
                _logger.LogWarning("Content cannot be empty for comment: {CommentId}", id);
                return null;
            }

            // Parse the new content to extract mentions and points
            var newContentParse = await _contentParsingService.ParseCommentContentAsync(
                content, 
                existingComment.CompanyId, 
                existingComment.PostId, 
                existingComment.PostedByAdded
            );

            // Validate that points haven't changed
            if (newContentParse.TotalPoints != existingComment.Points)
            {
                _logger.LogWarning("Points cannot be changed for comment: {CommentId}. Original: {OriginalPoints}, New: {NewPoints}", 
                    id, existingComment.Points, newContentParse.TotalPoints);
                return null;
            }

            // Note: Hashtags can be modified, so we don't validate them

            // Note: For comments, we don't validate UserMentioned because it includes users from the original post
            // and the PostedByAdded logic, which is more complex. The points validation ensures the overall
            // point allocation remains the same.

            // Update content and hashtags, mentions and points remain unchanged
            return await _commentProvider.UpdateCommentContentAsync(id, content, newContentParse.Hashtags);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating comment content: {CommentId}", id);
            return null;
        }
    }

    public async Task<bool> SoftDeleteCommentAsync(Guid id, Guid userId)
    {
        try
        {
            // Get existing comment
            var existingComment = await _commentProvider.GetCommentByIdAsync(id);
            if (existingComment == null)
            {
                _logger.LogWarning("Comment not found: {CommentId}", id);
                return false;
            }

            // Validate user owns the comment
            if (existingComment.UserId != userId)
            {
                _logger.LogWarning("User {UserId} does not own comment {CommentId}", userId, id);
                return false;
            }

            // Note: No need to revert transactions as per requirements
            return await _commentProvider.SoftDeleteCommentAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error soft deleting comment: {CommentId}", id);
            return false;
        }
    }

    public async Task<List<Comment>> GetCommentsByPostIdWithPaginationAsync(Guid postId, int page, int pageSize)
    {
        try
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20; // Default page size

            return await _commentProvider.GetCommentsByPostIdWithPaginationAsync(postId, page, pageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paginated comments for post: {PostId}", postId);
            return new List<Comment>();
        }
    }
}
