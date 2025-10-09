using Domain.Models;

namespace Cherish.RestApi.Models.Requests;

public class AddPointsRequest
{
    public int Points { get; set; }
}

public class GetUsersRequest
{
    public List<Guid>? UserIds { get; set; }
    public UserStatus? Status { get; set; }
    public Guid? TeamId { get; set; }
    public UserRole? Role { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
