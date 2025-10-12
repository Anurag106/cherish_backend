using Domain.Models;

namespace Cherish.RestApi.Models.Responses;

public class PostResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public Guid CompanyId { get; set; }
    public string Context { get; set; } = string.Empty;
    public List<Guid> UserMentioned { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public List<int> Hashtags { get; set; } = new();
    public string Metadata { get; set; } = string.Empty;
    public int TotalPoints { get; set; }
    public PostVisibility Visibility { get; set; }
    public bool Deleted { get; set; }
}
