namespace Cherish.RestApi.Models.Responses;

public class TransactionResponse
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid FromUserId { get; set; }
    public Guid ToUserId { get; set; }
    public int Points { get; set; }
    public DateTime DateAndTime { get; set; }
    public string? Description { get; set; }
}
