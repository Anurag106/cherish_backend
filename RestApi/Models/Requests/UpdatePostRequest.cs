using System.ComponentModel.DataAnnotations;

namespace Cherish.RestApi.Models.Requests;

public class UpdatePostRequest
{
    [Required]
    [StringLength(2000, MinimumLength = 1)]
    public string Context { get; set; } = string.Empty;
}
