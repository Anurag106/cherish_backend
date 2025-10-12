namespace Domain.Models;

public class Post
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CompanyId { get; set; }
    public string Context { get; set; } = string.Empty;
    public List<Guid> UserMentioned { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public List<int> Hashtags { get; set; } = new();
    public string Metadata { get; set; } = string.Empty; // JSONB as string
    public int TotalPoints { get; set; }
    public PostVisibility Visibility { get; set; }
    public bool Deleted { get; set; } = false;
}

public enum PostVisibility
{
    Public = 0,
    Team = 1,
    Private = 2
}

public enum PostSortOrder
{
    CreatedAtAsc = 0,
    CreatedAtDesc = 1
}

public class PostWithDetails
{
    public Post Post { get; set; } = new();
    public string UserFullName { get; set; } = string.Empty;
    public List<CommentWithUserInfo> LatestComments { get; set; } = new();
    public Dictionary<ReactionType, int> ReactionCounts { get; set; } = new();
    public Reaction? UserReaction { get; set; }
}

public class PaginatedPostResult
{
    public List<PostWithDetails> Posts { get; set; } = new();
    public int PageSize { get; set; }
    public string? NextCursor { get; set; }
    public string? PreviousCursor { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}
