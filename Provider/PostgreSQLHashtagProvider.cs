using Domain.Providers;
using Domain.Models;
using Npgsql;
using Microsoft.Extensions.Configuration;

namespace Provider;

public class PostgreSQLHashtagProvider : IHashtagProvider
{
    private readonly string _connectionString;

    public PostgreSQLHashtagProvider(IConfiguration configuration)
    {
        _connectionString = DatabaseConnectionManager.GetConnectionString(configuration);

    }


    public async Task<Hashtag?> GetHashtagByIdAsync(int id)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT id, name, description, company_id, created_by, created_date, modified_by, modified_date
            FROM hashtags
            WHERE id = @id";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Hashtag
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                CompanyId = reader.GetGuid(3),
                CreatedBy = reader.GetGuid(4),
                CreatedDate = reader.GetDateTime(5),
                ModifiedBy = reader.IsDBNull(6) ? null : reader.GetGuid(6),
                ModifiedDate = reader.IsDBNull(7) ? null : reader.GetDateTime(7)
            };
        }

        return null;
    }

    public async Task<Hashtag?> GetHashtagByNameAsync(string name, Guid companyId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT id, name, description, company_id, created_by, created_date, modified_by, modified_date
            FROM hashtags
            WHERE name = @name AND company_id = @company_id";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@name", name);
        command.Parameters.AddWithValue("@company_id", companyId);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Hashtag
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                CompanyId = reader.GetGuid(3),
                CreatedBy = reader.GetGuid(4),
                CreatedDate = reader.GetDateTime(5),
                ModifiedBy = reader.IsDBNull(6) ? null : reader.GetGuid(6),
                ModifiedDate = reader.IsDBNull(7) ? null : reader.GetDateTime(7)
            };
        }

        return null;
    }

    public async Task<List<Hashtag>> GetAllHashtagsAsync()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT id, name, description, company_id, created_by, created_date, modified_by, modified_date
            FROM hashtags
            ORDER BY created_date DESC";

        using var command = new NpgsqlCommand(query, connection);

        var hashtags = new List<Hashtag>();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            hashtags.Add(new Hashtag
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                CompanyId = reader.GetGuid(3),
                CreatedBy = reader.GetGuid(4),
                CreatedDate = reader.GetDateTime(5),
                ModifiedBy = reader.IsDBNull(6) ? null : reader.GetGuid(6),
                ModifiedDate = reader.IsDBNull(7) ? null : reader.GetDateTime(7)
            });
        }

        return hashtags;
    }

    public async Task<List<Hashtag>> GetHashtagsByCompanyIdAsync(Guid companyId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT id, name, description, company_id, created_by, created_date, modified_by, modified_date
            FROM hashtags
            WHERE company_id = @company_id
            ORDER BY created_date DESC";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@company_id", companyId);

        var hashtags = new List<Hashtag>();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            hashtags.Add(new Hashtag
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                CompanyId = reader.GetGuid(3),
                CreatedBy = reader.GetGuid(4),
                CreatedDate = reader.GetDateTime(5),
                ModifiedBy = reader.IsDBNull(6) ? null : reader.GetGuid(6),
                ModifiedDate = reader.IsDBNull(7) ? null : reader.GetDateTime(7)
            });
        }

        return hashtags;
    }

    public async Task<List<Hashtag>> GetHashtagsByCreatedByAsync(Guid createdBy)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT id, name, description, company_id, created_by, created_date, modified_by, modified_date
            FROM hashtags
            WHERE created_by = @created_by
            ORDER BY created_date DESC";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@created_by", createdBy);

        var hashtags = new List<Hashtag>();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            hashtags.Add(new Hashtag
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                CompanyId = reader.GetGuid(3),
                CreatedBy = reader.GetGuid(4),
                CreatedDate = reader.GetDateTime(5),
                ModifiedBy = reader.IsDBNull(6) ? null : reader.GetGuid(6),
                ModifiedDate = reader.IsDBNull(7) ? null : reader.GetDateTime(7)
            });
        }

        return hashtags;
    }

    public async Task<Hashtag> CreateHashtagAsync(string name, string description, Guid companyId, Guid createdBy)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            INSERT INTO hashtags (name, description, company_id, created_by, created_date)
            VALUES (@name, @description, @company_id, @created_by, CURRENT_TIMESTAMP)
            RETURNING id, name, description, company_id, created_by, created_date, modified_by, modified_date";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@name", name);
        command.Parameters.AddWithValue("@description", description);
        command.Parameters.AddWithValue("@company_id", companyId);
        command.Parameters.AddWithValue("@created_by", createdBy);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Hashtag
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                CompanyId = reader.GetGuid(3),
                CreatedBy = reader.GetGuid(4),
                CreatedDate = reader.GetDateTime(5),
                ModifiedBy = reader.IsDBNull(6) ? null : reader.GetGuid(6),
                ModifiedDate = reader.IsDBNull(7) ? null : reader.GetDateTime(7)
            };
        }

        throw new Exception("Failed to create hashtag");
    }

    public async Task<Hashtag?> UpdateHashtagAsync(int id, string name, string description, Guid modifiedBy)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            UPDATE hashtags 
            SET name = @name, description = @description, modified_by = @modified_by, modified_date = CURRENT_TIMESTAMP
            WHERE id = @id
            RETURNING id, name, description, company_id, created_by, created_date, modified_by, modified_date";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@name", name);
        command.Parameters.AddWithValue("@description", description);
        command.Parameters.AddWithValue("@modified_by", modifiedBy);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Hashtag
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                CompanyId = reader.GetGuid(3),
                CreatedBy = reader.GetGuid(4),
                CreatedDate = reader.GetDateTime(5),
                ModifiedBy = reader.IsDBNull(6) ? null : reader.GetGuid(6),
                ModifiedDate = reader.IsDBNull(7) ? null : reader.GetDateTime(7)
            };
        }

        return null;
    }

    public async Task<bool> DeleteHashtagAsync(int id)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = "DELETE FROM hashtags WHERE id = @id";
        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }
}
