using Domain.Services;
using Domain.Providers;
using Domain.Models;

namespace Cherish.RestApi.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionProvider _transactionProvider;
    private readonly IUserProvider _userProvider;
    private readonly ICompanyProvider _companyProvider;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(
        ITransactionProvider transactionProvider,
        IUserProvider userProvider,
        ICompanyProvider companyProvider,
        ILogger<TransactionService> logger)
    {
        _transactionProvider = transactionProvider;
        _userProvider = userProvider;
        _companyProvider = companyProvider;
        _logger = logger;
    }

    public async Task<Transaction?> GetTransactionByIdAsync(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Attempted to get transaction with empty GUID");
                return null;
            }

            return await _transactionProvider.GetTransactionByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transaction with ID: {Id}", id);
            throw;
        }
    }

    public async Task<List<Transaction>> GetTransactionsByCompanyIdAsync(Guid companyId)
    {
        try
        {
            if (companyId == Guid.Empty)
            {
                _logger.LogWarning("Attempted to get transactions with empty company GUID");
                return new List<Transaction>();
            }

            // Verify company exists
            var company = await _companyProvider.GetCompanyByIdAsync(companyId);
            if (company == null)
            {
                _logger.LogWarning("Attempted to get transactions for non-existent company: {CompanyId}", companyId);
                return new List<Transaction>();
            }

            return await _transactionProvider.GetTransactionsByCompanyIdAsync(companyId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transactions for company: {CompanyId}", companyId);
            throw;
        }
    }

    public async Task<List<Transaction>> GetTransactionsByFromUserIdAsync(Guid fromUserId)
    {
        try
        {
            if (fromUserId == Guid.Empty)
            {
                _logger.LogWarning("Attempted to get transactions with empty from user GUID");
                return new List<Transaction>();
            }

            return await _transactionProvider.GetTransactionsByFromUserIdAsync(fromUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transactions for from user: {FromUserId}", fromUserId);
            throw;
        }
    }

    public async Task<List<Transaction>> GetTransactionsByToUserIdAsync(Guid toUserId)
    {
        try
        {
            if (toUserId == Guid.Empty)
            {
                _logger.LogWarning("Attempted to get transactions with empty to user GUID");
                return new List<Transaction>();
            }

            return await _transactionProvider.GetTransactionsByToUserIdAsync(toUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transactions for to user: {ToUserId}", toUserId);
            throw;
        }
    }

    public async Task<List<Transaction>> GetTransactionsByUserIdAsync(Guid userId)
    {
        try
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("Attempted to get transactions with empty user GUID");
                return new List<Transaction>();
            }

            return await _transactionProvider.GetTransactionsByUserIdAsync(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transactions for user: {UserId}", userId);
            throw;
        }
    }

    public async Task<Transaction?> CreateTransactionAsync(Guid companyId, Guid fromUserId, Guid toUserId, int points, string? description = null)
    {
        try
        {
            if (companyId == Guid.Empty)
            {
                _logger.LogWarning("Attempted to create transaction with empty company GUID");
                return null;
            }

            if (fromUserId == Guid.Empty)
            {
                _logger.LogWarning("Attempted to create transaction with empty from user GUID");
                return null;
            }

            if (toUserId == Guid.Empty)
            {
                _logger.LogWarning("Attempted to create transaction with empty to user GUID");
                return null;
            }

            if (points <= 0)
            {
                _logger.LogWarning("Attempted to create transaction with invalid points: {Points}", points);
                return null;
            }

            if (fromUserId == toUserId)
            {
                _logger.LogWarning("Attempted to create transaction where from and to user are the same");
                return null;
            }

            // Verify company exists
            var company = await _companyProvider.GetCompanyByIdAsync(companyId);
            if (company == null)
            {
                _logger.LogWarning("Attempted to create transaction for non-existent company: {CompanyId}", companyId);
                return null;
            }

            // Verify both users exist and are in the same company
            var fromUser = await _userProvider.GetUserByIdAsync(fromUserId);
            if (fromUser == null)
            {
                _logger.LogWarning("Attempted to create transaction with non-existent from user: {FromUserId}", fromUserId);
                return null;
            }

            var toUser = await _userProvider.GetUserByIdAsync(toUserId);
            if (toUser == null)
            {
                _logger.LogWarning("Attempted to create transaction with non-existent to user: {ToUserId}", toUserId);
                return null;
            }

            if (fromUser.CompanyId != toUser.CompanyId || fromUser.CompanyId != companyId)
            {
                _logger.LogWarning("Attempted to create transaction between users from different companies");
                return null;
            }

            // Check if from user has enough available points
            if (fromUser.AvailablePoints < points)
            {
                _logger.LogWarning("User {FromUserId} does not have enough points. Available: {Available}, Required: {Points}", 
                    fromUserId, fromUser.AvailablePoints, points);
                return null;
            }

            // Create transaction in database
            var transaction = await _transactionProvider.CreateTransactionAsync(companyId, fromUserId, toUserId, points, description);

            // Update user points
            var fromUserNewAvailable = fromUser.AvailablePoints - points;
            var toUserNewTotal = toUser.TotalPoints + points;
            var toUserNewAvailable = toUser.AvailablePoints + points;

            await _userProvider.UpdateUserPointsAsync(fromUserId, fromUser.TotalPoints, fromUserNewAvailable);
            await _userProvider.UpdateUserPointsAsync(toUserId, toUserNewTotal, toUserNewAvailable);

            _logger.LogInformation("Transaction created successfully. From user {FromUserId} sent {Points} points to {ToUserId}", 
                fromUserId, points, toUserId);

            return transaction;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating transaction from {FromUserId} to {ToUserId} for {Points} points", 
                fromUserId, toUserId, points);
            throw;
        }
    }

    public async Task<List<Transaction>> GetAllTransactionsAsync()
    {
        try
        {
            return await _transactionProvider.GetAllTransactionsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all transactions");
            throw;
        }
    }
}
