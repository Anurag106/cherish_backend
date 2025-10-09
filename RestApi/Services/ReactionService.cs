using Domain.Services;
using Domain.Providers;
using Domain.Models;

namespace Cherish.RestApi.Services;

public class ReactionService : IReactionService
{
    private readonly IReactionProvider _reactionProvider;
    private readonly IUserProvider _userProvider;
    private readonly ICompanyProvider _companyProvider;
    private readonly IPostProvider _postProvider;
    private readonly ILogger<ReactionService> _logger;

    public ReactionService(
        IReactionProvider reactionProvider,
        IUserProvider userProvider,
        ICompanyProvider companyProvider,
        IPostProvider postProvider,
        ILogger<ReactionService> logger)
    {
        _reactionProvider = reactionProvider;
        _userProvider = userProvider;
        _companyProvider = companyProvider;
        _postProvider = postProvider;
        _logger = logger;
    }

    public async Task<Reaction?> GetUserReactionForPostAsync(Guid userId, Guid postId)
    {
        try
        {
            return await _reactionProvider.GetUserReactionForPostAsync(userId, postId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user reaction for post: {UserId}, {PostId}", userId, postId);
            return null;
        }
    }

    public async Task<List<Reaction>> GetReactionsByPostIdAsync(Guid postId)
    {
        try
        {
            return await _reactionProvider.GetReactionsByPostIdAsync(postId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting reactions for post: {PostId}", postId);
            return new List<Reaction>();
        }
    }

    public async Task<Dictionary<ReactionType, int>> GetReactionCountsByPostIdAsync(Guid postId)
    {
        try
        {
            return await _reactionProvider.GetReactionCountsByPostIdAsync(postId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting reaction counts for post: {PostId}", postId);
            return new Dictionary<ReactionType, int>();
        }
    }

    public async Task<List<Reaction>> GetReactionsByUserIdAsync(Guid userId)
    {
        try
        {
            return await _reactionProvider.GetReactionsByUserIdAsync(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting reactions for user: {UserId}", userId);
            return new List<Reaction>();
        }
    }

    public async Task<Reaction?> CreateOrUpdateReactionAsync(Guid userId, Guid postId, ReactionType emojiType)
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

            // Validate post exists
            var post = await _postProvider.GetPostByIdAsync(postId);
            if (post == null)
            {
                _logger.LogWarning("Post not found: {PostId}", postId);
                return null;
            }

            // Validate post belongs to same company as user
            if (post.CompanyId != user.CompanyId)
            {
                _logger.LogWarning("Post does not belong to user's company: {PostId}, {UserId}", postId, userId);
                return null;
            }

            // Create or update the reaction (one user can only have one reaction per post)
            var reaction = await _reactionProvider.CreateOrUpdateReactionAsync(
                user.CompanyId, 
                userId, 
                postId, 
                emojiType
            );

            _logger.LogInformation("Reaction created/updated successfully: {ReactionId} by user {UserId} for post {PostId}", 
                reaction.Id, userId, postId);
            
            return reaction;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating/updating reaction for user: {UserId}, post: {PostId}", userId, postId);
            return null;
        }
    }

    public async Task<bool> RemoveReactionAsync(Guid userId, Guid postId)
    {
        try
        {
            // Validate user exists
            var user = await _userProvider.GetUserByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", userId);
                return false;
            }

            // Validate post exists
            var post = await _postProvider.GetPostByIdAsync(postId);
            if (post == null)
            {
                _logger.LogWarning("Post not found: {PostId}", postId);
                return false;
            }

            // Validate post belongs to same company as user
            if (post.CompanyId != user.CompanyId)
            {
                _logger.LogWarning("Post does not belong to user's company: {PostId}, {UserId}", postId, userId);
                return false;
            }

            var success = await _reactionProvider.DeleteUserReactionForPostAsync(userId, postId);
            
            if (success)
            {
                _logger.LogInformation("Reaction removed successfully by user {UserId} for post {PostId}", userId, postId);
            }
            
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing reaction for user: {UserId}, post: {PostId}", userId, postId);
            return false;
        }
    }
}
