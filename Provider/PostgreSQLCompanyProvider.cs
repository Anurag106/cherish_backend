using Domain.Providers;
using Domain.Models;
using Npgsql;
using Microsoft.Extensions.Configuration;

namespace Provider;

public class PostgreSQLCompanyProvider : ICompanyProvider
{
    private readonly NpgsqlDataSource _dataSource;

    public PostgreSQLCompanyProvider(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }



    public async Task<Company?> GetCompanyByIdAsync(Guid id)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = "SELECT id, name, created_at, updated_at FROM companies WHERE id = @id";
        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Company
            {
                Id = reader.GetGuid(0),
                Name = reader.GetString(1),
                CreatedAt = reader.GetDateTime(2),
                UpdatedAt = reader.GetDateTime(3)
            };
        }

        return null;
    }

    public async Task<Company?> GetCompanyByNameAsync(string name)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = "SELECT id, name, created_at, updated_at FROM companies WHERE name = @name";
        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@name", name);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Company
            {
                Id = reader.GetGuid(0),
                Name = reader.GetString(1),
                CreatedAt = reader.GetDateTime(2),
                UpdatedAt = reader.GetDateTime(3)
            };
        }

        return null;
    }

    public async Task<Company> CreateCompanyAsync(string name)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            INSERT INTO companies (name, created_at, updated_at) 
            VALUES (@name, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP) 
            RETURNING id, name, created_at, updated_at";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@name", name);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Company
            {
                Id = reader.GetGuid(0),
                Name = reader.GetString(1),
                CreatedAt = reader.GetDateTime(2),
                UpdatedAt = reader.GetDateTime(3)
            };
        }

        throw new Exception("Failed to create company");
    }

    public async Task<Company> UpdateCompanyAsync(Guid id, string name)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            UPDATE companies 
            SET name = @name, updated_at = CURRENT_TIMESTAMP 
            WHERE id = @id 
            RETURNING id, name, created_at, updated_at";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@name", name);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Company
            {
                Id = reader.GetGuid(0),
                Name = reader.GetString(1),
                CreatedAt = reader.GetDateTime(2),
                UpdatedAt = reader.GetDateTime(3)
            };
        }

        throw new Exception("Company not found or update failed");
    }

    public async Task<bool> DeleteCompanyAsync(Guid id)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = "DELETE FROM companies WHERE id = @id";
        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<List<Company>> GetAllCompaniesAsync()
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = "SELECT id, name, created_at, updated_at FROM companies ORDER BY name";
        using var command = new NpgsqlCommand(query, connection);

        var companies = new List<Company>();
        using var reader = await command.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
        {
            companies.Add(new Company
            {
                Id = reader.GetGuid(0),
                Name = reader.GetString(1),
                CreatedAt = reader.GetDateTime(2),
                UpdatedAt = reader.GetDateTime(3)
            });
        }

        return companies;
    }
}
