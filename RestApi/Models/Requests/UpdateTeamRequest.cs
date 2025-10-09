namespace Cherish.RestApi.Models.Requests;

public class UpdateTeamRequest
{
    public string Name { get; set; } = string.Empty;
    public Guid ManagerId { get; set; }
}
