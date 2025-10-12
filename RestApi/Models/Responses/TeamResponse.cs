namespace Cherish.RestApi.Models.Responses;

public class TeamResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid ManagerId { get; set; }
    public Guid CompanyId { get; set; }
    public List<Guid> EmployeeIds { get; set; } = new();
    public List<EmployeeInfo> Employees { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
