using System.ComponentModel.DataAnnotations;
using Domain.Models;

namespace Cherish.RestApi.Models.Requests;

public class CreateReactionRequest
{
    [Required]
    public Guid PostId { get; set; }

    [Required]
    public string ReactionType { get; set; } = string.Empty;
}
