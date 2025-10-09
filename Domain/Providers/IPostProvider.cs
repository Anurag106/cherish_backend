using Domain.Models;

namespace Domain.Providers;

public interface IPostProvider
{
    Task<Post?> GetPostByIdAsync(Guid id);
    Task<List<Post>> GetPostsByCompanyIdAsync(Guid companyId);
    Task<List<Post>> GetPostsByUserIdAsync(Guid userId);
    Task<List<Post>> GetPostsByMentionedUserAsync(Guid userId);
    Task<List<Post>> GetPostsByHashtagIdAsync(int hashtagId);
    Task<List<Post>> GetAllPostsAsync();
    Task<Post> CreatePostAsync(Guid userId, Guid companyId, string context, List<Guid> userMentioned, List<int> hashtags, string metadata, int totalPoints, PostVisibility visibility);
    Task<bool> SoftDeletePostAsync(Guid id);
    Task<Post?> UpdatePostContentAsync(Guid id, string content, List<int> hashtags);
    Task<(List<Post> Posts, bool HasNextPage)> GetFilteredPostsAsync(
        Guid companyId,
        int pageSize,
        CursorInfo? cursor = null,
        bool? filterByTeam = null,
        Guid? filterByUserId = null,
        List<int>? hashtagIds = null,
        PostSortOrder sortOrder = PostSortOrder.CreatedAtDesc);
}
