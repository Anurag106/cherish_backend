using Domain.Models;

namespace Domain.Providers;

public interface ICommentProvider
{
    Task<Comment?> GetCommentByIdAsync(Guid id);
    Task<List<Comment>> GetCommentsByPostIdAsync(Guid postId);
    Task<List<Comment>> GetCommentsByUserIdAsync(Guid userId);
    Task<List<Comment>> GetCommentsByCompanyIdAsync(Guid companyId);
    Task<Comment> CreateCommentAsync(Guid userId, bool postedByAdded, Guid companyId, string content, int points, Guid postId, List<int> hashtags, string metadata);
    Task<Comment?> UpdateCommentAsync(Guid id, string content, int points, List<int> hashtags, string metadata);
    Task<bool> SoftDeleteCommentAsync(Guid id);
    Task<Comment?> UpdateCommentContentAsync(Guid id, string content, List<int> hashtags);
    Task<List<Comment>> GetCommentsByPostIdWithPaginationAsync(Guid postId, int page, int pageSize);
}
