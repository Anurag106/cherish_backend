using Domain.Models;

namespace Domain.Providers;

public interface IReactionProvider
{
    Task<Reaction?> GetReactionByIdAsync(long id);
    Task<Reaction?> GetUserReactionForPostAsync(Guid userId, Guid postId);
    Task<List<Reaction>> GetReactionsByPostIdAsync(Guid postId);
    Task<Dictionary<ReactionType, int>> GetReactionCountsByPostIdAsync(Guid postId);
    Task<List<Reaction>> GetReactionsByCompanyIdAsync(Guid companyId);
    Task<List<Reaction>> GetReactionsByUserIdAsync(Guid userId);
    Task<Reaction> CreateOrUpdateReactionAsync(Guid companyId, Guid userId, Guid postId, ReactionType emojiType);
    Task<bool> DeleteReactionAsync(long id);
    Task<bool> DeleteUserReactionForPostAsync(Guid userId, Guid postId);
}
