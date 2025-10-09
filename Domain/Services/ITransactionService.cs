using Domain.Models;

namespace Domain.Services;

public interface ITransactionService
{
    Task<Transaction?> GetTransactionByIdAsync(Guid id);
    Task<List<Transaction>> GetTransactionsByCompanyIdAsync(Guid companyId);
    Task<List<Transaction>> GetTransactionsByFromUserIdAsync(Guid fromUserId);
    Task<List<Transaction>> GetTransactionsByToUserIdAsync(Guid toUserId);
    Task<List<Transaction>> GetTransactionsByUserIdAsync(Guid userId);
    Task<Transaction?> CreateTransactionAsync(Guid companyId, Guid fromUserId, Guid toUserId, int points, string? description = null);
    Task<List<Transaction>> GetAllTransactionsAsync();
}
