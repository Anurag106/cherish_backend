using System.ComponentModel.DataAnnotations;
using Domain.Models;

namespace Cherish.RestApi.Models.Requests;

public class PostFilterRequest
{
    [Range(1, 50, ErrorMessage = "PageSize must be between 1 and 50")]
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
