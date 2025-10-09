using Domain.Models;

namespace Domain.Services;

public interface IReactionService
{
    Task<Reaction?> GetUserReactionForPostAsync(Guid userId, Guid postId);
    Task<List<Reaction>> GetReactionsByPostIdAsync(Guid postId);
    Task<Dictionary<ReactionType, int>> GetReactionCountsByPostIdAsync(Guid postId);
    Task<List<Reaction>> GetReactionsByUserIdAsync(Guid userId);
    Task<Reaction?> CreateOrUpdateReactionAsync(Guid userId, Guid postId, ReactionType emojiType);
    Task<bool> RemoveReactionAsync(Guid userId, Guid postId);
}
