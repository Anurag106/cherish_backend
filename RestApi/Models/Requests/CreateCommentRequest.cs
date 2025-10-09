using System.ComponentModel.DataAnnotations;

namespace Cherish.RestApi.Models.Requests;

public class CreateCommentRequest
{
    [Required]
    public Guid PostId { get; set; }

    [Required]
    [StringLength(2000, MinimumLength = 1)]
    public string Content { get; set; } = string.Empty;

    [Required]
    public bool PostedByAdded { get; set; }
}
