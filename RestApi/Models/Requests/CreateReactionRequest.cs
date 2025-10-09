using System.ComponentModel.DataAnnotations;
using Domain.Models;

namespace Cherish.RestApi.Models.Requests;

public class CreateReactionRequest
{
    [Required]
    public Guid PostId { get; set; }

    [Required]
    [Range(1, 5, ErrorMessage = "ReactionType must be between 1 and 5")]
    public ReactionType EmojiType { get; set; }
}
