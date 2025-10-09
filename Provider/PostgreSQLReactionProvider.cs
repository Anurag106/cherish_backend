using Domain.Providers;
using Domain.Models;
using Npgsql;
using Microsoft.Extensions.Configuration;

namespace Provider;

public class PostgreSQLReactionProvider : IReactionProvider
{
    private readonly string _connectionString;

    public PostgreSQLReactionProvider(IConfiguration configuration)
    {
        _connectionString = DatabaseConnectionManager.GetConnectionString(configuration);
    }

    public async Task<Reaction?> GetReactionByIdAsync(long id)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT id, company_id, user_id, post_id, emoji_type, last_modified_at
            FROM reactions
            WHERE id = @id";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Reaction
            {
                Id = reader.GetInt64(0),
                CompanyId = reader.GetGuid(1),
                UserId = reader.GetGuid(2),
                PostId = reader.GetGuid(3),
                EmojiType = (ReactionType)reader.GetInt32(4),
                LastModifiedAt = reader.GetDateTime(5)
            };
        }

        return null;
    }

    public async Task<Reaction?> GetUserReactionForPostAsync(Guid userId, Guid postId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT id, company_id, user_id, post_id, emoji_type, last_modified_at
            FROM reactions
            WHERE user_id = @user_id AND post_id = @post_id";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@user_id", userId);
        command.Parameters.AddWithValue("@post_id", postId);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Reaction
            {
                Id = reader.GetInt64(0),
                CompanyId = reader.GetGuid(1),
                UserId = reader.GetGuid(2),
                PostId = reader.GetGuid(3),
                EmojiType = (ReactionType)reader.GetInt32(4),
                LastModifiedAt = reader.GetDateTime(5)
            };
        }

        return null;
    }

    public async Task<List<Reaction>> GetReactionsByPostIdAsync(Guid postId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT id, company_id, user_id, post_id, emoji_type, last_modified_at
            FROM reactions
            WHERE post_id = @post_id
            ORDER BY last_modified_at DESC";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@post_id", postId);

        var reactions = new List<Reaction>();
        using var reader = await command.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
        {
            reactions.Add(new Reaction
            {
                Id = reader.GetInt64(0),
                CompanyId = reader.GetGuid(1),
                UserId = reader.GetGuid(2),
                PostId = reader.GetGuid(3),
                EmojiType = (ReactionType)reader.GetInt32(4),
                LastModifiedAt = reader.GetDateTime(5)
            });
        }

        return reactions;
    }

    public async Task<Dictionary<ReactionType, int>> GetReactionCountsByPostIdAsync(Guid postId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT emoji_type, COUNT(*) as count
            FROM reactions
            WHERE post_id = @post_id
            GROUP BY emoji_type";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@post_id", postId);

        var counts = new Dictionary<ReactionType, int>();
        using var reader = await command.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
        {
            var emojiType = (ReactionType)reader.GetInt32(0);
            var count = reader.GetInt32(1);
            counts[emojiType] = count;
        }

        // Ensure all reaction types are present with count 0
        foreach (ReactionType reactionType in Enum.GetValues<ReactionType>())
        {
            if (!counts.ContainsKey(reactionType))
            {
                counts[reactionType] = 0;
            }
        }

        return counts;
    }

    public async Task<List<Reaction>> GetReactionsByCompanyIdAsync(Guid companyId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT id, company_id, user_id, post_id, emoji_type, last_modified_at
            FROM reactions
            WHERE company_id = @company_id
            ORDER BY last_modified_at DESC";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@company_id", companyId);

        var reactions = new List<Reaction>();
        using var reader = await command.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
        {
            reactions.Add(new Reaction
            {
                Id = reader.GetInt64(0),
                CompanyId = reader.GetGuid(1),
                UserId = reader.GetGuid(2),
                PostId = reader.GetGuid(3),
                EmojiType = (ReactionType)reader.GetInt32(4),
                LastModifiedAt = reader.GetDateTime(5)
            });
        }

        return reactions;
    }

    public async Task<List<Reaction>> GetReactionsByUserIdAsync(Guid userId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT id, company_id, user_id, post_id, emoji_type, last_modified_at
            FROM reactions
            WHERE user_id = @user_id
            ORDER BY last_modified_at DESC";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@user_id", userId);

        var reactions = new List<Reaction>();
        using var reader = await command.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
        {
            reactions.Add(new Reaction
            {
                Id = reader.GetInt64(0),
                CompanyId = reader.GetGuid(1),
                UserId = reader.GetGuid(2),
                PostId = reader.GetGuid(3),
                EmojiType = (ReactionType)reader.GetInt32(4),
                LastModifiedAt = reader.GetDateTime(5)
            });
        }

        return reactions;
    }

    public async Task<Reaction> CreateOrUpdateReactionAsync(Guid companyId, Guid userId, Guid postId, ReactionType emojiType)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        // Use UPSERT (INSERT ... ON CONFLICT ... DO UPDATE)
        var query = @"
            INSERT INTO reactions (company_id, user_id, post_id, emoji_type, last_modified_at)
            VALUES (@company_id, @user_id, @post_id, @emoji_type, @last_modified_at)
            ON CONFLICT (user_id, post_id)
            DO UPDATE SET 
                emoji_type = @emoji_type,
                last_modified_at = @last_modified_at
            RETURNING id, company_id, user_id, post_id, emoji_type, last_modified_at";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@company_id", companyId);
        command.Parameters.AddWithValue("@user_id", userId);
        command.Parameters.AddWithValue("@post_id", postId);
        command.Parameters.AddWithValue("@emoji_type", (int)emojiType);
        command.Parameters.AddWithValue("@last_modified_at", DateTime.UtcNow);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Reaction
            {
                Id = reader.GetInt64(0),
                CompanyId = reader.GetGuid(1),
                UserId = reader.GetGuid(2),
                PostId = reader.GetGuid(3),
                EmojiType = (ReactionType)reader.GetInt32(4),
                LastModifiedAt = reader.GetDateTime(5)
            };
        }

        throw new Exception("Failed to create or update reaction");
    }

    public async Task<bool> DeleteReactionAsync(long id)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = "DELETE FROM reactions WHERE id = @id";
        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteUserReactionForPostAsync(Guid userId, Guid postId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = "DELETE FROM reactions WHERE user_id = @user_id AND post_id = @post_id";
        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@user_id", userId);
        command.Parameters.AddWithValue("@post_id", postId);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }
}
