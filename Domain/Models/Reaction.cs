namespace Domain.Models;

public class Reaction
{
    public long Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid UserId { get; set; }
    public Guid PostId { get; set; }
    public ReactionType EmojiType { get; set; }
    public DateTime LastModifiedAt { get; set; }
}

public enum ReactionType
{
    Like = 1,
    Love = 2,
    Laugh = 3,
    Angry = 4,
    Sad = 5
}
