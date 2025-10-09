namespace Domain.Models;

public class PostFilterRequest
{
    public int PageSize { get; set; } = 15;

    // Cursor for pagination (encoded timestamp + post ID)
    public string? Cursor { get; set; }

    // Filter by team (only fetch posts from user's team)
    public bool? FilterByTeam { get; set; }

    // Filter by specific user ID (user can follow)
    public Guid? FilterByUserId { get; set; }

    // Filter by specific hashtags
    public List<int>? HashtagIds { get; set; }

    // Filter by hashtag names (alternative to hashtag IDs)
    public List<string>? HashtagNames { get; set; }

    // Sort order
    public PostSortOrder SortOrder { get; set; } = PostSortOrder.CreatedAtDesc;
}

public class CursorInfo
{
    public DateTime CreatedAt { get; set; }
    public Guid PostId { get; set; }
}
