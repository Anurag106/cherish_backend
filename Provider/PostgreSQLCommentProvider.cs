using Domain.Providers;
using Domain.Models;
using Npgsql;
using NpgsqlTypes;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Provider;

public class PostgreSQLCommentProvider : ICommentProvider
{
    private readonly NpgsqlDataSource _dataSource;

    public PostgreSQLCommentProvider(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<Comment?> GetCommentByIdAsync(Guid id)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            SELECT id, user_id, posted_by_added, company_id, content, points, post_id, hashtags, created_at, metadata
            FROM comments
            WHERE id = @id";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Comment
            {
                Id = reader.GetGuid(0),
                UserId = reader.GetGuid(1),
                PostedByAdded = reader.GetBoolean(2),
                CompanyId = reader.GetGuid(3),
                Content = reader.GetString(4),
                Points = reader.GetInt32(5),
                PostId = reader.GetGuid(6),
                Hashtags = JsonSerializer.Deserialize<List<int>>(reader.GetString(7)) ?? new List<int>(),
                CreatedAt = reader.GetDateTime(8),
                Metadata = reader.GetString(9)
            };
        }

        return null;
    }

    public async Task<List<Comment>> GetCommentsByPostIdAsync(Guid postId)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            SELECT id, user_id, posted_by_added, company_id, content, points, post_id, hashtags, created_at, metadata
            FROM comments
            WHERE post_id = @post_id
            ORDER BY created_at ASC";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@post_id", postId);

        var comments = new List<Comment>();
        using var reader = await command.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
        {
            comments.Add(new Comment
            {
                Id = reader.GetGuid(0),
                UserId = reader.GetGuid(1),
                PostedByAdded = reader.GetBoolean(2),
                CompanyId = reader.GetGuid(3),
                Content = reader.GetString(4),
                Points = reader.GetInt32(5),
                PostId = reader.GetGuid(6),
                Hashtags = JsonSerializer.Deserialize<List<int>>(reader.GetString(7)) ?? new List<int>(),
                CreatedAt = reader.GetDateTime(8),
                Metadata = reader.GetString(9)
            });
        }

        return comments;
    }

    public async Task<List<Comment>> GetCommentsByUserIdAsync(Guid userId)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            SELECT id, user_id, posted_by_added, company_id, content, points, post_id, hashtags, created_at, metadata
            FROM comments
            WHERE user_id = @user_id
            ORDER BY created_at DESC";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@user_id", userId);

        var comments = new List<Comment>();
        using var reader = await command.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
        {
            comments.Add(new Comment
            {
                Id = reader.GetGuid(0),
                UserId = reader.GetGuid(1),
                PostedByAdded = reader.GetBoolean(2),
                CompanyId = reader.GetGuid(3),
                Content = reader.GetString(4),
                Points = reader.GetInt32(5),
                PostId = reader.GetGuid(6),
                Hashtags = JsonSerializer.Deserialize<List<int>>(reader.GetString(7)) ?? new List<int>(),
                CreatedAt = reader.GetDateTime(8),
                Metadata = reader.GetString(9)
            });
        }

        return comments;
    }

    public async Task<List<Comment>> GetCommentsByCompanyIdAsync(Guid companyId)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            SELECT id, user_id, posted_by_added, company_id, content, points, post_id, hashtags, created_at, metadata
            FROM comments
            WHERE company_id = @company_id
            ORDER BY created_at DESC";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@company_id", companyId);

        var comments = new List<Comment>();
        using var reader = await command.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
        {
            comments.Add(new Comment
            {
                Id = reader.GetGuid(0),
                UserId = reader.GetGuid(1),
                PostedByAdded = reader.GetBoolean(2),
                CompanyId = reader.GetGuid(3),
                Content = reader.GetString(4),
                Points = reader.GetInt32(5),
                PostId = reader.GetGuid(6),
                Hashtags = JsonSerializer.Deserialize<List<int>>(reader.GetString(7)) ?? new List<int>(),
                CreatedAt = reader.GetDateTime(8),
                Metadata = reader.GetString(9)
            });
        }

        return comments;
    }

    public async Task<Comment> CreateCommentAsync(Guid userId, bool postedByAdded, Guid companyId, string content, int points, Guid postId, List<int> hashtags, string metadata)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            INSERT INTO comments (user_id, posted_by_added, company_id, content, points, post_id, hashtags, metadata)
            VALUES (@user_id, @posted_by_added, @company_id, @content, @points, @post_id, @hashtags, @metadata)
            RETURNING id, user_id, posted_by_added, company_id, content, points, post_id, hashtags, created_at, metadata";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@user_id", userId);
        command.Parameters.AddWithValue("@posted_by_added", postedByAdded);
        command.Parameters.AddWithValue("@company_id", companyId);
        command.Parameters.AddWithValue("@content", content);
        command.Parameters.AddWithValue("@points", points);
        command.Parameters.AddWithValue("@post_id", postId);
        command.Parameters.Add("@hashtags", NpgsqlDbType.Jsonb).Value = JsonSerializer.Serialize(hashtags);
        command.Parameters.Add("@metadata", NpgsqlDbType.Jsonb).Value = metadata;

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Comment
            {
                Id = reader.GetGuid(0),
                UserId = reader.GetGuid(1),
                PostedByAdded = reader.GetBoolean(2),
                CompanyId = reader.GetGuid(3),
                Content = reader.GetString(4),
                Points = reader.GetInt32(5),
                PostId = reader.GetGuid(6),
                Hashtags = JsonSerializer.Deserialize<List<int>>(reader.GetString(7)) ?? new List<int>(),
                CreatedAt = reader.GetDateTime(8),
                Metadata = reader.GetString(9)
            };
        }

        throw new Exception("Failed to create comment");
    }

    public async Task<Comment?> UpdateCommentAsync(Guid id, string content, int points, List<int> hashtags, string metadata)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            UPDATE comments 
            SET content = @content, points = @points, hashtags = @hashtags, metadata = @metadata
            WHERE id = @id
            RETURNING id, user_id, posted_by_added, company_id, content, points, post_id, hashtags, created_at, metadata";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@content", content);
        command.Parameters.AddWithValue("@points", points);
        command.Parameters.Add("@hashtags", NpgsqlDbType.Jsonb).Value = JsonSerializer.Serialize(hashtags);
        command.Parameters.Add("@metadata", NpgsqlDbType.Jsonb).Value = metadata;

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Comment
            {
                Id = reader.GetGuid(0),
                UserId = reader.GetGuid(1),
                PostedByAdded = reader.GetBoolean(2),
                CompanyId = reader.GetGuid(3),
                Content = reader.GetString(4),
                Points = reader.GetInt32(5),
                PostId = reader.GetGuid(6),
                Hashtags = JsonSerializer.Deserialize<List<int>>(reader.GetString(7)) ?? new List<int>(),
                CreatedAt = reader.GetDateTime(8),
                Metadata = reader.GetString(9)
            };
        }

        return null;
    }

    public async Task<bool> DeleteCommentAsync(Guid id)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = "DELETE FROM comments WHERE id = @id";
        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<List<Comment>> GetCommentsByPostIdWithPaginationAsync(Guid postId, int page, int pageSize)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var offset = (page - 1) * pageSize;
        var query = @"
            SELECT id, user_id, posted_by_added, company_id, content, points, post_id, hashtags, created_at, metadata
            FROM comments
            WHERE post_id = @post_id
            ORDER BY created_at ASC
            LIMIT @limit OFFSET @offset";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@post_id", postId);
        command.Parameters.AddWithValue("@limit", pageSize);
        command.Parameters.AddWithValue("@offset", offset);

        var comments = new List<Comment>();
        using var reader = await command.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
        {
            comments.Add(new Comment
            {
                Id = reader.GetGuid(0),
                UserId = reader.GetGuid(1),
                PostedByAdded = reader.GetBoolean(2),
                CompanyId = reader.GetGuid(3),
                Content = reader.GetString(4),
                Points = reader.GetInt32(5),
                PostId = reader.GetGuid(6),
                Hashtags = JsonSerializer.Deserialize<List<int>>(reader.GetString(7)) ?? new List<int>(),
                CreatedAt = reader.GetDateTime(8),
                Metadata = reader.GetString(9)
            });
        }

        return comments;
    }

    public async Task<bool> SoftDeleteCommentAsync(Guid id)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = "UPDATE comments SET deleted = true WHERE id = @id";
        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<Comment?> UpdateCommentContentAsync(Guid id, string content, List<int> hashtags)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            UPDATE comments 
            SET content = @content, hashtags = @hashtags
            WHERE id = @id AND deleted = false
            RETURNING id, user_id, posted_by_added, company_id, content, points, post_id, hashtags, created_at, metadata, deleted";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@content", content);
        command.Parameters.Add("@hashtags", NpgsqlDbType.Jsonb).Value = JsonSerializer.Serialize(hashtags);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Comment
            {
                Id = reader.GetGuid(0),
                UserId = reader.GetGuid(1),
                PostedByAdded = reader.GetBoolean(2),
                CompanyId = reader.GetGuid(3),
                Content = reader.GetString(4),
                Points = reader.GetInt32(5),
                PostId = reader.GetGuid(6),
                Hashtags = JsonSerializer.Deserialize<List<int>>(reader.GetString(7)) ?? new List<int>(),
                CreatedAt = reader.GetDateTime(8),
                Metadata = reader.GetString(9),
                Deleted = reader.GetBoolean(10)
            };
        }

        return null;
    }
}
