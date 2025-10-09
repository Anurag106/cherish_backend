using Domain.Providers;
using Domain.Models;
using Npgsql;
using Microsoft.Extensions.Configuration;
using System.Web;

namespace Provider;

public class PostgreSQLUserProvider : IUserProvider
{
    private readonly string _connectionString;

    public PostgreSQLUserProvider(IConfiguration configuration)
    {
        _connectionString = DatabaseConnectionManager.GetConnectionString(configuration);
    }



    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT id, username, password, email, first_name, last_name, role, status, team_id, 
                   department, job_title, date_hired, date_of_birth, total_points, available_points,
                   company_id, created_at, updated_at 
            FROM users 
            WHERE username = @username";
        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@username", username);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new User
            {
                Id = reader.GetGuid(0),
                Username = reader.GetString(1),
                Password = reader.GetString(2),
                Email = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                FirstName = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                LastName = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                Role = (UserRole)reader.GetInt32(6),
                Status = (UserStatus)reader.GetInt32(7),
                TeamId = reader.IsDBNull(8) ? null : reader.GetGuid(8),
                Department = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                JobTitle = reader.IsDBNull(10) ? string.Empty : reader.GetString(10),
                DateHired = reader.IsDBNull(11) ? null : reader.GetDateTime(11),
                DateOfBirth = reader.IsDBNull(12) ? null : reader.GetDateTime(12),
                TotalPoints = reader.GetInt32(13),
                AvailablePoints = reader.GetInt32(14),
                CompanyId = reader.GetGuid(15),
                CreatedAt = reader.GetDateTime(16),
                UpdatedAt = reader.GetDateTime(17)
            };
        }

        return null;
    }

    public async Task<User> CreateUserAsync(string username, string password, Guid companyId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            INSERT INTO users (username, password, email, first_name, last_name, role, status, team_id, 
                              department, job_title, date_hired, date_of_birth, total_points, available_points,
                              company_id, created_at, updated_at)
            VALUES (@username, @password, @email, @first_name, @last_name, @role, @status, @team_id,
                    @department, @job_title, @date_hired, @date_of_birth, @total_points, @available_points,
                    @company_id, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)
            RETURNING id, username, password, email, first_name, last_name, role, status, team_id, 
                      department, job_title, date_hired, date_of_birth, total_points, available_points,
                      company_id, created_at, updated_at";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@username", username);
        command.Parameters.AddWithValue("@password", password);
        command.Parameters.AddWithValue("@email", DBNull.Value);
        command.Parameters.AddWithValue("@first_name", DBNull.Value);
        command.Parameters.AddWithValue("@last_name", DBNull.Value);
        command.Parameters.AddWithValue("@role", (int)UserRole.Employee);
        command.Parameters.AddWithValue("@status", (int)UserStatus.Active);
        command.Parameters.AddWithValue("@team_id", DBNull.Value);
        command.Parameters.AddWithValue("@department", DBNull.Value);
        command.Parameters.AddWithValue("@job_title", DBNull.Value);
        command.Parameters.AddWithValue("@date_hired", DBNull.Value);
        command.Parameters.AddWithValue("@date_of_birth", DBNull.Value);
        command.Parameters.AddWithValue("@total_points", 0);
        command.Parameters.AddWithValue("@available_points", 0);
        command.Parameters.AddWithValue("@company_id", companyId);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new User
            {
                Id = reader.GetGuid(0),
                Username = reader.GetString(1),
                Password = reader.GetString(2),
                Email = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                FirstName = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                LastName = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                Role = (UserRole)reader.GetInt32(6),
                Status = (UserStatus)reader.GetInt32(7),
                TeamId = reader.IsDBNull(8) ? null : reader.GetGuid(8),
                Department = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                JobTitle = reader.IsDBNull(10) ? string.Empty : reader.GetString(10),
                DateHired = reader.IsDBNull(11) ? null : reader.GetDateTime(11),
                DateOfBirth = reader.IsDBNull(12) ? null : reader.GetDateTime(12),
                TotalPoints = reader.GetInt32(13),
                AvailablePoints = reader.GetInt32(14),
                CompanyId = reader.GetGuid(15),
                CreatedAt = reader.GetDateTime(16),
                UpdatedAt = reader.GetDateTime(17)
            };
        }

        throw new Exception("Failed to create user");
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT u.id, u.username, u.password, u.email, u.first_name, u.last_name, u.role, u.status, u.team_id, 
                   u.department, u.job_title, u.date_hired, u.date_of_birth, u.total_points, u.available_points,
                   u.company_id, u.created_at, u.updated_at, c.name as company_name
            FROM users u
            LEFT JOIN companies c ON u.company_id = c.id
            WHERE u.id = @id";
        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new User
            {
                Id = reader.GetGuid(0),
                Username = reader.GetString(1),
                Password = reader.GetString(2),
                Email = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                FirstName = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                LastName = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                Role = (UserRole)reader.GetInt32(6),
                Status = (UserStatus)reader.GetInt32(7),
                TeamId = reader.IsDBNull(8) ? null : reader.GetGuid(8),
                Department = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                JobTitle = reader.IsDBNull(10) ? string.Empty : reader.GetString(10),
                DateHired = reader.IsDBNull(11) ? null : reader.GetDateTime(11),
                DateOfBirth = reader.IsDBNull(12) ? null : reader.GetDateTime(12),
                TotalPoints = reader.GetInt32(13),
                AvailablePoints = reader.GetInt32(14),
                CompanyId = reader.GetGuid(15),
                CreatedAt = reader.GetDateTime(16),
                UpdatedAt = reader.GetDateTime(17),
                CompanyName = reader.IsDBNull(18) ? string.Empty : reader.GetString(18)
            };
        }

        return null;
    }

    public async Task<bool> UpdateUserPasswordAsync(string username, string newPassword)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = "UPDATE users SET password = @password, updated_at = CURRENT_TIMESTAMP WHERE username = @username";
        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@password", newPassword);
        command.Parameters.AddWithValue("@username", username);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteUserAsync(string username)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = "DELETE FROM users WHERE username = @username";
        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@username", username);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<List<User>> GetUsersByCompanyIdAsync(Guid companyId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT id, username, password, email, first_name, last_name, role, status, team_id, 
                   department, job_title, date_hired, date_of_birth, total_points, available_points,
                   company_id, created_at, updated_at 
            FROM users 
            WHERE company_id = @company_id";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@company_id", companyId);

        var users = new List<User>();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            users.Add(new User
            {
                Id = reader.GetGuid(0),
                Username = reader.GetString(1),
                Password = reader.GetString(2),
                Email = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                FirstName = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                LastName = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                Role = (UserRole)reader.GetInt32(6),
                Status = (UserStatus)reader.GetInt32(7),
                TeamId = reader.IsDBNull(8) ? null : reader.GetGuid(8),
                Department = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                JobTitle = reader.IsDBNull(10) ? string.Empty : reader.GetString(10),
                DateHired = reader.IsDBNull(11) ? null : reader.GetDateTime(11),
                DateOfBirth = reader.IsDBNull(12) ? null : reader.GetDateTime(12),
                TotalPoints = reader.GetInt32(13),
                AvailablePoints = reader.GetInt32(14),
                CompanyId = reader.GetGuid(15),
                CreatedAt = reader.GetDateTime(16),
                UpdatedAt = reader.GetDateTime(17),
                CompanyName = reader.IsDBNull(18) ? string.Empty : reader.GetString(18)
            });
        }

        return users;
    }

    public async Task<bool> UpdateUserPointsAsync(Guid userId, int newTotalPoints, int newAvailablePoints)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            UPDATE users 
            SET total_points = @total_points, available_points = @available_points, updated_at = CURRENT_TIMESTAMP 
            WHERE id = @user_id";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@user_id", userId);
        command.Parameters.AddWithValue("@total_points", newTotalPoints);
        command.Parameters.AddWithValue("@available_points", newAvailablePoints);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<List<User>> GetUsersAsync(Guid companyId, List<Guid>? userIds = null, UserStatus? status = null, Guid? teamId = null, UserRole? role = null, int pageNumber = 1, int pageSize = 20)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var whereConditions = new List<string> { "company_id = @company_id" };
        var parameters = new List<NpgsqlParameter>
        {
            new("@company_id", companyId),
            new("@offset", (pageNumber - 1) * pageSize),
            new("@limit", pageSize)
        };

        // Add filters
        if (userIds != null && userIds.Any())
        {
            whereConditions.Add("id = ANY(@user_ids)");
            parameters.Add(new("@user_ids", userIds.ToArray()));
        }

        if (status.HasValue)
        {
            whereConditions.Add("status = @status");
            parameters.Add(new("@status", (int)status.Value));
        }

        if (teamId.HasValue)
        {
            whereConditions.Add("team_id = @team_id");
            parameters.Add(new("@team_id", teamId.Value));
        }

        if (role.HasValue)
        {
            whereConditions.Add("role = @role");
            parameters.Add(new("@role", (int)role.Value));
        }

        var whereClause = string.Join(" AND ", whereConditions);

        var query = $@"
            SELECT u.id, u.username, u.password, u.email, u.first_name, u.last_name, u.role, u.status, u.team_id, 
                   u.department, u.job_title, u.date_hired, u.date_of_birth, u.total_points, u.available_points,
                   u.company_id, u.created_at, u.updated_at, c.name as company_name
            FROM users u
            LEFT JOIN companies c ON u.company_id = c.id
            WHERE {whereClause}
            ORDER BY u.created_at DESC
            OFFSET @offset LIMIT @limit";

        using var command = new NpgsqlCommand(query, connection);
        foreach (var param in parameters)
        {
            command.Parameters.Add(param);
        }

        var users = new List<User>();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            users.Add(new User
            {
                Id = reader.GetGuid(0),
                Username = reader.GetString(1),
                Password = reader.GetString(2),
                Email = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                FirstName = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                LastName = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                Role = (UserRole)reader.GetInt32(6),
                Status = (UserStatus)reader.GetInt32(7),
                TeamId = reader.IsDBNull(8) ? null : reader.GetGuid(8),
                Department = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                JobTitle = reader.IsDBNull(10) ? string.Empty : reader.GetString(10),
                DateHired = reader.IsDBNull(11) ? null : reader.GetDateTime(11),
                DateOfBirth = reader.IsDBNull(12) ? null : reader.GetDateTime(12),
                TotalPoints = reader.GetInt32(13),
                AvailablePoints = reader.GetInt32(14),
                CompanyId = reader.GetGuid(15),
                CreatedAt = reader.GetDateTime(16),
                UpdatedAt = reader.GetDateTime(17),
                CompanyName = reader.IsDBNull(18) ? string.Empty : reader.GetString(18)
            });
        }

        return users;
    }
}
