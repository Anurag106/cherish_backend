namespace Cherish.RestApi.Models.Requests;

public class CreateUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Guid CompanyId { get; set; }
}
