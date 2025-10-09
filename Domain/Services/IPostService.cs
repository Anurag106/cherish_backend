using Domain.Models;

namespace Domain.Services;

public interface IPostService
{
    Task<Post?> GetPostByIdAsync(Guid id);
    Task<List<Post>> GetPostsByCompanyIdAsync(Guid companyId);
    Task<List<Post>> GetPostsByUserIdAsync(Guid userId);
    Task<List<Post>> GetPostsByMentionedUserAsync(Guid userId);
    Task<List<Post>> GetPostsByHashtagIdAsync(int hashtagId);
    Task<List<Post>> GetAllPostsAsync();
    Task<PostValidationResult> ValidateAndCreatePostAsync(Guid userId, Guid companyId, string context, PostVisibility visibility);
    Task<bool> SoftDeletePostAsync(Guid id, Guid userId);
    Task<Post?> UpdatePostContentAsync(Guid id, string content, Guid userId);
    Task<PostWithDetails?> GetPostWithDetailsAsync(Guid postId, Guid? userId = null);
    Task<PaginatedPostResult> GetFilteredPostsWithDetailsAsync(
        Guid userId,
        PostFilterRequest filterRequest);
}

public class PostValidationResult
{
    public bool IsValid { get; set; }
    public List<string> MissingItems { get; set; } = new();
    public List<string> InvalidItems { get; set; } = new();
    public List<Guid> UserMentioned { get; set; } = new();
    public List<int> Hashtags { get; set; } = new();
    public int TotalPoints { get; set; }
    public string ParsedContext { get; set; } = string.Empty;
    public Post? Post { get; set; }
}
