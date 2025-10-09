using Domain.Models;

namespace Domain.Services;

public interface IContentParsingService
{
    Task<CommentParseResult> ParseCommentContentAsync(string content, Guid companyId, Guid postId, bool postedByAdded);
    Task<(List<Guid> UserMentioned, List<int> Hashtags, int TotalPoints, string ParsedContent)> ParsePostContentAsync(string content, Guid companyId);
    Task<bool> ValidateUserHasEnoughPointsAsync(Guid userId, int requiredPoints);
    Task<List<Transaction>> CreatePointTransactionsAsync(Guid fromUserId, List<Guid> toUserIds, int pointsPerUser, Guid companyId, string description);
}
