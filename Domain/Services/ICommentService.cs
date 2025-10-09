using Domain.Models;

namespace Domain.Services;

public interface ICommentService
{
    Task<Comment?> GetCommentByIdAsync(Guid id);
    Task<List<Comment>> GetCommentsByPostIdAsync(Guid postId);
    Task<List<Comment>> GetCommentsByUserIdAsync(Guid userId);
    Task<List<Comment>> GetCommentsByCompanyIdAsync(Guid companyId);
    Task<Comment?> CreateCommentAsync(Guid userId, bool postedByAdded, Guid companyId, string content, Guid postId);
    Task<Comment?> UpdateCommentContentAsync(Guid id, string content, Guid userId);
    Task<bool> SoftDeleteCommentAsync(Guid id, Guid userId);
    Task<List<Comment>> GetCommentsByPostIdWithPaginationAsync(Guid postId, int page, int pageSize);
}
