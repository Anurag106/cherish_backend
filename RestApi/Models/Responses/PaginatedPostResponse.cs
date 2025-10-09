using Domain.Models;

namespace Cherish.RestApi.Models.Responses;

public class PaginatedPostResponse
{
    public List<PostWithDetailsResponse> Posts { get; set; } = new();
    public PaginationInfo Pagination { get; set; } = new();
    public FilterInfo AppliedFilters { get; set; } = new();
}

public class PaginationInfo
{
    public int PageSize { get; set; }
    public string? NextCursor { get; set; }
    public string? PreviousCursor { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}

public class FilterInfo
{
    public bool FilterByTeam { get; set; }
    public Guid? FilterByUserId { get; set; }
    public List<int> HashtagIds { get; set; } = new();
    public List<string> HashtagNames { get; set; } = new();
    public PostSortOrder SortOrder { get; set; }
}

public class PostWithDetailsResponse : PostResponse
{
    public List<CommentResponse> LatestComments { get; set; } = new();
    public ReactionCountsResponse ReactionCounts { get; set; } = new();
    public ReactionResponse? UserReaction { get; set; }
}
