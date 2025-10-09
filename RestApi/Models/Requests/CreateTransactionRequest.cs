namespace Cherish.RestApi.Models.Requests;

public class CreateTransactionRequest
{
    public Guid ToUserId { get; set; }
    public int Points { get; set; }
    public string? Description { get; set; }
}
