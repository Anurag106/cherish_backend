using System.ComponentModel.DataAnnotations;

namespace Cherish.RestApi.Models.Requests;

public class UpdateCommentRequest
{
    [Required]
    [StringLength(2000, MinimumLength = 1)]
    public string Content { get; set; } = string.Empty;
}
