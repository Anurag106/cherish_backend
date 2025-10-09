using Domain.Models;

namespace Cherish.RestApi.Models.Requests;

public class CreatePostRequest
{
    public string Context { get; set; } = string.Empty;
    public PostVisibility Visibility { get; set; } = PostVisibility.Public;
}
