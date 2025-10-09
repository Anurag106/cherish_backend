namespace Cherish.RestApi.Models.Requests;

public class CreateTeamRequest
{
    public string Name { get; set; } = string.Empty;
    public Guid ManagerId { get; set; }
    public Guid CompanyId { get; set; }
}
