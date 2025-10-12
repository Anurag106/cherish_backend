using Domain.Models;

namespace Cherish.RestApi.Models.Responses;

public class ReactionResponse
{
    public long Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid UserId { get; set; }
    public Guid PostId { get; set; }
    public string EmojiType { get; set; } = string.Empty;
    public DateTime LastModifiedAt { get; set; }
}

public class ReactionCountsResponse
{
    public Guid PostId { get; set; }
    public Dictionary<ReactionType, int> Counts { get; set; } = new();
}

