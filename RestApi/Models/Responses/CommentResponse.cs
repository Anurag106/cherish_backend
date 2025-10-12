namespace Cherish.RestApi.Models.Responses;

public class CommentResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public bool PostedByAdded { get; set; }
    public Guid CompanyId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Points { get; set; }
    public Guid PostId { get; set; }
    public List<int> Hashtags { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public string Metadata { get; set; } = string.Empty;
    public bool Deleted { get; set; }
}
