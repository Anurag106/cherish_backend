using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public class Comment
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; } // created by
    public bool PostedByAdded { get; set; } // is user including post by user too
    public Guid CompanyId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Points { get; set; }
    public Guid PostId { get; set; }
    public List<int> Hashtags { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public string Metadata { get; set; } = string.Empty; // JSONB as string
    public bool Deleted { get; set; } = false;
}

public class CommentWithUserInfo
{
    public Comment Comment { get; set; } = new();
    public string UserFullName { get; set; } = string.Empty;
}

public class CommentParseResult
{
    public List<Guid> UserMentioned { get; set; } = new();
    public List<int> Hashtags { get; set; } = new();
    public int TotalPoints { get; set; }
    public string ParsedContent { get; set; } = string.Empty;
}
