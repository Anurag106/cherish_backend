using Domain.Providers;
using Domain.Models;
using Npgsql;
using NpgsqlTypes;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Provider;

public class PostgreSQLPostProvider : IPostProvider
{
    private readonly NpgsqlDataSource _dataSource;

    public PostgreSQLPostProvider(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;

    }


    public async Task<Post?> GetPostByIdAsync(Guid id)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            SELECT id, user_id, company_id, context, user_mentioned, created_at, hashtags, metadata, total_points, visibility, deleted
            FROM posts
            WHERE id = @id AND deleted = false";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            var userMentionedJson = reader.GetString(4);
            var hashtagsJson = reader.GetString(6);
            var metadataJson = reader.GetString(7);

            return new Post
            {
                Id = reader.GetGuid(0),
                UserId = reader.GetGuid(1),
                CompanyId = reader.GetGuid(2),
                Context = reader.GetString(3),
                UserMentioned = JsonSerializer.Deserialize<List<Guid>>(userMentionedJson) ?? new List<Guid>(),
                CreatedAt = reader.GetDateTime(5),
                Hashtags = JsonSerializer.Deserialize<List<int>>(hashtagsJson) ?? new List<int>(),
                Metadata = metadataJson,
                TotalPoints = reader.GetInt32(8),
                Visibility = (PostVisibility)reader.GetInt32(9)
            };
        }

        return null;
    }

    public async Task<List<Post>> GetPostsByCompanyIdAsync(Guid companyId)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            SELECT id, user_id, company_id, context, user_mentioned, created_at, hashtags, metadata, total_points, visibility
            FROM posts
            WHERE company_id = @company_id AND deleted = false
            ORDER BY created_at DESC";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@company_id", companyId);

        var posts = new List<Post>();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var userMentionedJson = reader.GetString(4);
            var hashtagsJson = reader.GetString(6);
            var metadataJson = reader.GetString(7);

            posts.Add(new Post
            {
                Id = reader.GetGuid(0),
                UserId = reader.GetGuid(1),
                CompanyId = reader.GetGuid(2),
                Context = reader.GetString(3),
                UserMentioned = JsonSerializer.Deserialize<List<Guid>>(userMentionedJson) ?? new List<Guid>(),
                CreatedAt = reader.GetDateTime(5),
                Hashtags = JsonSerializer.Deserialize<List<int>>(hashtagsJson) ?? new List<int>(),
                Metadata = metadataJson,
                TotalPoints = reader.GetInt32(8),
                Visibility = (PostVisibility)reader.GetInt32(9)
            });
        }

        return posts;
    }

    public async Task<List<Post>> GetPostsByUserIdAsync(Guid userId)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            SELECT id, user_id, company_id, context, user_mentioned, created_at, hashtags, metadata, total_points, visibility, deleted
            FROM posts
            WHERE user_id = @user_id AND deleted = false
            ORDER BY created_at DESC";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@user_id", userId);

        var posts = new List<Post>();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var userMentionedJson = reader.GetString(4);
            var hashtagsJson = reader.GetString(6);
            var metadataJson = reader.GetString(7);

            posts.Add(new Post
            {
                Id = reader.GetGuid(0),
                UserId = reader.GetGuid(1),
                CompanyId = reader.GetGuid(2),
                Context = reader.GetString(3),
                UserMentioned = JsonSerializer.Deserialize<List<Guid>>(userMentionedJson) ?? new List<Guid>(),
                CreatedAt = reader.GetDateTime(5),
                Hashtags = JsonSerializer.Deserialize<List<int>>(hashtagsJson) ?? new List<int>(),
                Metadata = metadataJson,
                TotalPoints = reader.GetInt32(8),
                Visibility = (PostVisibility)reader.GetInt32(9)
            });
        }

        return posts;
    }

    public async Task<List<Post>> GetPostsByMentionedUserAsync(Guid userId)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            SELECT id, user_id, company_id, context, user_mentioned, created_at, hashtags, metadata, total_points, visibility
            FROM posts
            WHERE user_mentioned @> @user_id::text::jsonb
            ORDER BY created_at DESC";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@user_id", userId.ToString());

        var posts = new List<Post>();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var userMentionedJson = reader.GetString(4);
            var hashtagsJson = reader.GetString(6);
            var metadataJson = reader.GetString(7);

            posts.Add(new Post
            {
                Id = reader.GetGuid(0),
                UserId = reader.GetGuid(1),
                CompanyId = reader.GetGuid(2),
                Context = reader.GetString(3),
                UserMentioned = JsonSerializer.Deserialize<List<Guid>>(userMentionedJson) ?? new List<Guid>(),
                CreatedAt = reader.GetDateTime(5),
                Hashtags = JsonSerializer.Deserialize<List<int>>(hashtagsJson) ?? new List<int>(),
                Metadata = metadataJson,
                TotalPoints = reader.GetInt32(8),
                Visibility = (PostVisibility)reader.GetInt32(9)
            });
        }

        return posts;
    }

    public async Task<List<Post>> GetPostsByHashtagIdAsync(int hashtagId)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            SELECT id, user_id, company_id, context, user_mentioned, created_at, hashtags, metadata, total_points, visibility
            FROM posts
            WHERE hashtags @> @hashtag_id::text::jsonb
            ORDER BY created_at DESC";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@hashtag_id", hashtagId.ToString());

        var posts = new List<Post>();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var userMentionedJson = reader.GetString(4);
            var hashtagsJson = reader.GetString(6);
            var metadataJson = reader.GetString(7);

            posts.Add(new Post
            {
                Id = reader.GetGuid(0),
                UserId = reader.GetGuid(1),
                CompanyId = reader.GetGuid(2),
                Context = reader.GetString(3),
                UserMentioned = JsonSerializer.Deserialize<List<Guid>>(userMentionedJson) ?? new List<Guid>(),
                CreatedAt = reader.GetDateTime(5),
                Hashtags = JsonSerializer.Deserialize<List<int>>(hashtagsJson) ?? new List<int>(),
                Metadata = metadataJson,
                TotalPoints = reader.GetInt32(8),
                Visibility = (PostVisibility)reader.GetInt32(9)
            });
        }

        return posts;
    }

    public async Task<List<Post>> GetAllPostsAsync()
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            SELECT id, user_id, company_id, context, user_mentioned, created_at, hashtags, metadata, total_points, visibility
            FROM posts
            ORDER BY created_at DESC";

        using var command = new NpgsqlCommand(query, connection);

        var posts = new List<Post>();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var userMentionedJson = reader.GetString(4);
            var hashtagsJson = reader.GetString(6);
            var metadataJson = reader.GetString(7);

            posts.Add(new Post
            {
                Id = reader.GetGuid(0),
                UserId = reader.GetGuid(1),
                CompanyId = reader.GetGuid(2),
                Context = reader.GetString(3),
                UserMentioned = JsonSerializer.Deserialize<List<Guid>>(userMentionedJson) ?? new List<Guid>(),
                CreatedAt = reader.GetDateTime(5),
                Hashtags = JsonSerializer.Deserialize<List<int>>(hashtagsJson) ?? new List<int>(),
                Metadata = metadataJson,
                TotalPoints = reader.GetInt32(8),
                Visibility = (PostVisibility)reader.GetInt32(9)
            });
        }

        return posts;
    }

    public async Task<Post> CreatePostAsync(Guid userId, Guid companyId, string context, List<Guid> userMentioned, List<int> hashtags, string metadata, int totalPoints, PostVisibility visibility)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            INSERT INTO posts (user_id, company_id, context, user_mentioned, hashtags, metadata, total_points, visibility, created_at)
            VALUES (@user_id, @company_id, @context, @user_mentioned, @hashtags, @metadata, @total_points, @visibility, CURRENT_TIMESTAMP)
            RETURNING id, user_id, company_id, context, user_mentioned, created_at, hashtags, metadata, total_points, visibility";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@user_id", userId);
        command.Parameters.AddWithValue("@company_id", companyId);
        command.Parameters.AddWithValue("@context", context);
        command.Parameters.Add("@user_mentioned", NpgsqlTypes.NpgsqlDbType.Jsonb).Value = JsonSerializer.Serialize(userMentioned);
        command.Parameters.Add("@hashtags", NpgsqlTypes.NpgsqlDbType.Jsonb).Value = JsonSerializer.Serialize(hashtags);
        command.Parameters.Add("@metadata", NpgsqlTypes.NpgsqlDbType.Jsonb).Value = string.IsNullOrEmpty(metadata) ? "{}" : metadata;
        command.Parameters.AddWithValue("@total_points", totalPoints);
        command.Parameters.AddWithValue("@visibility", (int)visibility);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            var userMentionedJson = reader.GetString(4);
            var hashtagsJson = reader.GetString(6);
            var metadataJson = reader.GetString(7);

            return new Post
            {
                Id = reader.GetGuid(0),
                UserId = reader.GetGuid(1),
                CompanyId = reader.GetGuid(2),
                Context = reader.GetString(3),
                UserMentioned = JsonSerializer.Deserialize<List<Guid>>(userMentionedJson) ?? new List<Guid>(),
                CreatedAt = reader.GetDateTime(5),
                Hashtags = JsonSerializer.Deserialize<List<int>>(hashtagsJson) ?? new List<int>(),
                Metadata = metadataJson,
                TotalPoints = reader.GetInt32(8),
                Visibility = (PostVisibility)reader.GetInt32(9)
            };
        }

        throw new Exception("Failed to create post");
    }

    public async Task<bool> SoftDeletePostAsync(Guid id)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = "UPDATE posts SET deleted = true WHERE id = @id";
        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<Post?> UpdatePostContentAsync(Guid id, string content, List<int> hashtags)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            UPDATE posts 
            SET context = @content, hashtags = @hashtags
            WHERE id = @id AND deleted = false
            RETURNING id, user_id, company_id, context, user_mentioned, created_at, hashtags, metadata, total_points, visibility, deleted";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@content", content);
        command.Parameters.Add("@hashtags", NpgsqlDbType.Jsonb).Value = JsonSerializer.Serialize(hashtags);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            var userMentionedJson = reader.GetString(4);
            var hashtagsJson = reader.GetString(6);
            var metadataJson = reader.GetString(7);

            return new Post
            {
                Id = reader.GetGuid(0),
                UserId = reader.GetGuid(1),
                CompanyId = reader.GetGuid(2),
                Context = reader.GetString(3),
                UserMentioned = JsonSerializer.Deserialize<List<Guid>>(userMentionedJson) ?? new List<Guid>(),
                CreatedAt = reader.GetDateTime(5),
                Hashtags = JsonSerializer.Deserialize<List<int>>(hashtagsJson) ?? new List<int>(),
                Metadata = metadataJson,
                TotalPoints = reader.GetInt32(8),
                Visibility = (PostVisibility)reader.GetInt32(9),
                Deleted = reader.GetBoolean(10)
            };
        }

        return null;
    }

    public async Task<(List<Post> Posts, bool HasNextPage)> GetFilteredPostsAsync(
        Guid companyId,
        int pageSize,
        CursorInfo? cursor = null,
        bool? filterByTeam = null,
        Guid? filterByUserId = null,
        List<int>? hashtagIds = null,
        PostSortOrder sortOrder = PostSortOrder.CreatedAtDesc)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var parameters = new List<NpgsqlParameter>
        {
            new("@company_id", companyId),
            new("@page_size_plus_one", pageSize + 1)
        };

        // Build cursor conditions
        var cursorCondition = "";
        if (cursor != null)
        {
            if (sortOrder == PostSortOrder.CreatedAtDesc)
            {
                cursorCondition = @"
                    AND (p.created_at < @cursor_created_at 
                         OR (p.created_at = @cursor_created_at AND p.id < @cursor_post_id))";
            }
            else if (sortOrder == PostSortOrder.CreatedAtAsc)
            {
                cursorCondition = @"
                    AND (p.created_at > @cursor_created_at 
                         OR (p.created_at = @cursor_created_at AND p.id > @cursor_post_id))";
            }

            parameters.Add(new("@cursor_created_at", cursor.CreatedAt));
            parameters.Add(new("@cursor_post_id", cursor.PostId));
        }

        // Build hashtag condition
        var hashtagCondition = "";
        if (hashtagIds != null && hashtagIds.Any())
        {
            hashtagCondition = @"
                AND EXISTS (
                    SELECT 1 FROM jsonb_array_elements_text(p.hashtags) AS hashtag_id
                    WHERE hashtag_id::int = ANY(@hashtag_ids)
                )";
            parameters.Add(new("@hashtag_ids", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Integer)
            {
                Value = hashtagIds.ToArray()
            });
        }

        // Build sort order
        var orderByClause = sortOrder switch
        {
            PostSortOrder.CreatedAtAsc => "ORDER BY p.created_at ASC, p.id ASC",
            PostSortOrder.CreatedAtDesc => "ORDER BY p.created_at DESC, p.id DESC",
            _ => "ORDER BY p.created_at DESC, p.id DESC"
        };

        string postsQuery;

        if (filterByTeam == true)
        {
            // Optimized team filter using CTE
            parameters.Add(new("@current_user_id", filterByUserId ?? Guid.Empty));
            
            postsQuery = $@"
                WITH team_members AS (
                    SELECT u1.id as user_id
                    FROM users u1 
                    JOIN users u2 ON u1.team_id = u2.team_id 
                    WHERE u2.id = @current_user_id
                ),
                relevant_posts AS (
                    SELECT DISTINCT p.id
                    FROM posts p
                    WHERE p.company_id = @company_id 
                      AND p.deleted = false
                      AND (
                          p.user_id IN (SELECT user_id FROM team_members)
                          OR EXISTS (
                              SELECT 1 FROM jsonb_array_elements_text(p.user_mentioned) AS mentioned_user_id
                              WHERE mentioned_user_id::uuid IN (SELECT user_id FROM team_members)
                          )
                          OR EXISTS (
                              SELECT 1 FROM reactions r 
                              WHERE r.post_id = p.id AND r.user_id IN (SELECT user_id FROM team_members)
                          )
                          OR EXISTS (
                              SELECT 1 FROM comments c 
                              WHERE c.post_id = p.id AND c.user_id IN (SELECT user_id FROM team_members)
                          )
                      )
                      {cursorCondition}
                      {hashtagCondition}
                )
                SELECT p.id, p.user_id, p.company_id, p.context, p.user_mentioned, 
                       p.created_at, p.hashtags, p.metadata, p.total_points, p.visibility, p.deleted
                FROM posts p
                INNER JOIN relevant_posts rp ON p.id = rp.id
                {orderByClause}
                LIMIT @page_size_plus_one";
        }
        else if (filterByUserId.HasValue)
        {
            // Optimized user filter
            parameters.Add(new("@user_id", filterByUserId.Value));
            
            postsQuery = $@"
                WITH relevant_posts AS (
                    SELECT DISTINCT p.id
                    FROM posts p
                    WHERE p.company_id = @company_id 
                      AND p.deleted = false
                      AND (
                          p.user_id = @user_id
                          OR EXISTS (
                              SELECT 1 FROM jsonb_array_elements_text(p.user_mentioned) AS mentioned_user_id
                              WHERE mentioned_user_id::uuid = @user_id
                          )
                          OR EXISTS (
                              SELECT 1 FROM reactions r 
                              WHERE r.post_id = p.id AND r.user_id = @user_id
                          )
                          OR EXISTS (
                              SELECT 1 FROM comments c 
                              WHERE c.post_id = p.id AND c.user_id = @user_id
                          )
                      )
                      {cursorCondition}
                      {hashtagCondition}
                )
                SELECT p.id, p.user_id, p.company_id, p.context, p.user_mentioned, 
                       p.created_at, p.hashtags, p.metadata, p.total_points, p.visibility, p.deleted
                FROM posts p
                INNER JOIN relevant_posts rp ON p.id = rp.id
                {orderByClause}
                LIMIT @page_size_plus_one";
        }
        else
        {
            // Simple filter without team/user filtering
            postsQuery = $@"
                SELECT p.id, p.user_id, p.company_id, p.context, p.user_mentioned, 
                       p.created_at, p.hashtags, p.metadata, p.total_points, p.visibility, p.deleted
                FROM posts p
                WHERE p.company_id = @company_id 
                  AND p.deleted = false
                  {cursorCondition}
                  {hashtagCondition}
                {orderByClause}
                LIMIT @page_size_plus_one";
        }

        using var postsCommand = new NpgsqlCommand(postsQuery, connection);
        foreach (var param in parameters)
        {
            postsCommand.Parameters.Add(param);
        }

        var posts = new List<Post>();
        using var reader = await postsCommand.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
        {
            posts.Add(new Post
            {
                Id = reader.GetGuid(0),
                UserId = reader.GetGuid(1),
                CompanyId = reader.GetGuid(2),
                Context = reader.GetString(3),
                UserMentioned = reader.GetFieldValue<List<Guid>>(4),
                CreatedAt = reader.GetDateTime(5),
                Hashtags = reader.GetFieldValue<List<int>>(6),
                Metadata = reader.GetFieldValue<string>(7),
                TotalPoints = reader.GetInt32(8),
                Visibility = (PostVisibility)reader.GetInt32(9),
                Deleted = reader.GetBoolean(10)
            });
        }

        // Check if there's a next page
        var hasNextPage = posts.Count > pageSize;
        if (hasNextPage)
        {
            posts.RemoveAt(posts.Count - 1); // Remove the extra record
        }

        return (posts, hasNextPage);
    }
}
