using Domain.Providers;
using Domain.Models;
using Npgsql;
using Microsoft.Extensions.Configuration;

namespace Provider;

public class PostgreSQLTransactionProvider : ITransactionProvider
{
    private readonly string _connectionString;

    public PostgreSQLTransactionProvider(IConfiguration configuration)
    {
        _connectionString = DatabaseConnectionManager.GetConnectionString(configuration);

    }


    public async Task<Transaction?> GetTransactionByIdAsync(Guid id)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT id, company_id, from_user_id, to_user_id, points, date_and_time, description
            FROM transactions
            WHERE id = @id";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Transaction
            {
                Id = reader.GetGuid(0),
                CompanyId = reader.GetGuid(1),
                FromUserId = reader.GetGuid(2),
                ToUserId = reader.GetGuid(3),
                Points = reader.GetInt32(4),
                DateAndTime = reader.GetDateTime(5),
                Description = reader.IsDBNull(6) ? null : reader.GetString(6)
            };
        }

        return null;
    }

    public async Task<List<Transaction>> GetTransactionsByCompanyIdAsync(Guid companyId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT id, company_id, from_user_id, to_user_id, points, date_and_time, description
            FROM transactions
            WHERE company_id = @company_id
            ORDER BY date_and_time DESC";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@company_id", companyId);

        var transactions = new List<Transaction>();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            transactions.Add(new Transaction
            {
                Id = reader.GetGuid(0),
                CompanyId = reader.GetGuid(1),
                FromUserId = reader.GetGuid(2),
                ToUserId = reader.GetGuid(3),
                Points = reader.GetInt32(4),
                DateAndTime = reader.GetDateTime(5),
                Description = reader.IsDBNull(6) ? null : reader.GetString(6)
            });
        }

        return transactions;
    }

    public async Task<List<Transaction>> GetTransactionsByFromUserIdAsync(Guid fromUserId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT id, company_id, from_user_id, to_user_id, points, date_and_time, description
            FROM transactions
            WHERE from_user_id = @from_user_id
            ORDER BY date_and_time DESC";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@from_user_id", fromUserId);

        var transactions = new List<Transaction>();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            transactions.Add(new Transaction
            {
                Id = reader.GetGuid(0),
                CompanyId = reader.GetGuid(1),
                FromUserId = reader.GetGuid(2),
                ToUserId = reader.GetGuid(3),
                Points = reader.GetInt32(4),
                DateAndTime = reader.GetDateTime(5),
                Description = reader.IsDBNull(6) ? null : reader.GetString(6)
            });
        }

        return transactions;
    }

    public async Task<List<Transaction>> GetTransactionsByToUserIdAsync(Guid toUserId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT id, company_id, from_user_id, to_user_id, points, date_and_time, description
            FROM transactions
            WHERE to_user_id = @to_user_id
            ORDER BY date_and_time DESC";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@to_user_id", toUserId);

        var transactions = new List<Transaction>();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            transactions.Add(new Transaction
            {
                Id = reader.GetGuid(0),
                CompanyId = reader.GetGuid(1),
                FromUserId = reader.GetGuid(2),
                ToUserId = reader.GetGuid(3),
                Points = reader.GetInt32(4),
                DateAndTime = reader.GetDateTime(5),
                Description = reader.IsDBNull(6) ? null : reader.GetString(6)
            });
        }

        return transactions;
    }

    public async Task<List<Transaction>> GetTransactionsByUserIdAsync(Guid userId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT id, company_id, from_user_id, to_user_id, points, date_and_time, description
            FROM transactions
            WHERE from_user_id = @user_id OR to_user_id = @user_id
            ORDER BY date_and_time DESC";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@user_id", userId);

        var transactions = new List<Transaction>();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            transactions.Add(new Transaction
            {
                Id = reader.GetGuid(0),
                CompanyId = reader.GetGuid(1),
                FromUserId = reader.GetGuid(2),
                ToUserId = reader.GetGuid(3),
                Points = reader.GetInt32(4),
                DateAndTime = reader.GetDateTime(5),
                Description = reader.IsDBNull(6) ? null : reader.GetString(6)
            });
        }

        return transactions;
    }

    public async Task<Transaction> CreateTransactionAsync(Guid companyId, Guid fromUserId, Guid toUserId, int points, string? description = null)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            INSERT INTO transactions (company_id, from_user_id, to_user_id, points, date_and_time, description)
            VALUES (@company_id, @from_user_id, @to_user_id, @points, CURRENT_TIMESTAMP, @description)
            RETURNING id, company_id, from_user_id, to_user_id, points, date_and_time, description";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@company_id", companyId);
        command.Parameters.AddWithValue("@from_user_id", fromUserId);
        command.Parameters.AddWithValue("@to_user_id", toUserId);
        command.Parameters.AddWithValue("@points", points);
        command.Parameters.AddWithValue("@description", description ?? (object)DBNull.Value);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Transaction
            {
                Id = reader.GetGuid(0),
                CompanyId = reader.GetGuid(1),
                FromUserId = reader.GetGuid(2),
                ToUserId = reader.GetGuid(3),
                Points = reader.GetInt32(4),
                DateAndTime = reader.GetDateTime(5),
                Description = reader.IsDBNull(6) ? null : reader.GetString(6)
            };
        }

        throw new Exception("Failed to create transaction");
    }

    public async Task<List<Transaction>> GetAllTransactionsAsync()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT id, company_id, from_user_id, to_user_id, points, date_and_time, description
            FROM transactions
            ORDER BY date_and_time DESC";

        using var command = new NpgsqlCommand(query, connection);

        var transactions = new List<Transaction>();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            transactions.Add(new Transaction
            {
                Id = reader.GetGuid(0),
                CompanyId = reader.GetGuid(1),
                FromUserId = reader.GetGuid(2),
                ToUserId = reader.GetGuid(3),
                Points = reader.GetInt32(4),
                DateAndTime = reader.GetDateTime(5),
                Description = reader.IsDBNull(6) ? null : reader.GetString(6)
            });
        }

        return transactions;
    }
}
