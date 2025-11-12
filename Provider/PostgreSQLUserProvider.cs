using Domain.Providers;
using Domain.Models;
using Npgsql;
using Microsoft.Extensions.Configuration;

namespace Provider;

/// <summary>
/// PostgreSQL User Provider - Updated for JWT-based authentication
/// This provider now works with string user IDs (ASP.NET Identity) and NO password storage
/// </summary>
public class PostgreSQLUserProvider : IUserProvider
{
    private readonly NpgsqlDataSource _dataSource;

    public PostgreSQLUserProvider(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            SELECT u.id, u.email, u.first_name, u.last_name, u.profile_picture_url, u.is_active,
                   u.user_mode, u.employee_status, u.team_id, u.department, u.job_title,
                   u.date_hired, u.date_of_birth, u.total_points, u.available_points,
                   u.company_id, u.created_at, u.updated_at, u.last_synced_at, u.sync_source,
                   c.name as company_name
            FROM users u
            LEFT JOIN companies c ON u.company_id = c.id
            WHERE u.email = @email";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@email", email);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapUserFromReader(reader);
        }

        return null;
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            SELECT u.id, u.email, u.first_name, u.last_name, u.profile_picture_url, u.is_active,
                   u.user_mode, u.employee_status, u.team_id, u.department, u.job_title,
                   u.date_hired, u.date_of_birth, u.total_points, u.available_points,
                   u.company_id, u.created_at, u.updated_at, u.last_synced_at, u.sync_source,
                   u.preferred_first_name, c.name as company_name
            FROM users u
            LEFT JOIN companies c ON u.company_id = c.id
            WHERE u.id = @id";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapUserFromReader(reader);
        }

        return null;
    }

    public async Task<List<User>> GetUsersByCompanyIdAsync(Guid companyId)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            SELECT u.id, u.email, u.first_name, u.last_name, u.profile_picture_url, u.is_active,
                   u.user_mode, u.employee_status, u.team_id, u.department, u.job_title,
                   u.date_hired, u.date_of_birth, u.total_points, u.available_points,
                   u.company_id, u.created_at, u.updated_at, u.last_synced_at, u.sync_source,
                   u.preferred_first_name, c.name as company_name
            FROM users u
            LEFT JOIN companies c ON u.company_id = c.id
            WHERE u.company_id = @company_id
            ORDER BY u.first_name, u.last_name";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@company_id", companyId);

        var users = new List<User>();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            users.Add(MapUserFromReader(reader));
        }

        return users;
    }

    public async Task<User> CreateUserAsync(Guid userId, string email, string firstName, string lastName, Guid companyId)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            INSERT INTO users (id, email, first_name, last_name, company_id, is_active, 
                              user_mode, employee_status, sync_source, last_synced_at, created_at, updated_at)
            VALUES (@id, @email, @first_name, @last_name, @company_id, true,
                    0, 0, 'jwt_claims', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)
            RETURNING id, email, first_name, last_name, profile_picture_url, is_active,
                      user_mode, employee_status, team_id, department, job_title,
                      date_hired, date_of_birth, total_points, available_points,
                      company_id, created_at, updated_at, last_synced_at, sync_source, preferred_first_name";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", userId);
        command.Parameters.AddWithValue("@email", email);
        command.Parameters.AddWithValue("@first_name", firstName);
        command.Parameters.AddWithValue("@last_name", lastName);
        command.Parameters.AddWithValue("@company_id", companyId);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapUserFromReader(reader);
        }

        throw new Exception("Failed to create user");
    }

    public async Task<bool> UpdateUserPointsAsync(Guid userId, int newTotalPoints, int newAvailablePoints)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            UPDATE users 
            SET total_points = @total_points, 
                available_points = @available_points, 
                updated_at = CURRENT_TIMESTAMP 
            WHERE id = @user_id";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@user_id", userId);
        command.Parameters.AddWithValue("@total_points", newTotalPoints);
        command.Parameters.AddWithValue("@available_points", newAvailablePoints);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = "UPDATE users SET is_active = false, updated_at = CURRENT_TIMESTAMP WHERE id = @user_id";
        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@user_id", userId);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<List<User>> GetUsersAsync(Guid companyId, List<Guid>? userIds = null, EmployeeStatus? status = null, Guid? teamId = null, int pageNumber = 1, int pageSize = 20)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var whereConditions = new List<string> { "u.company_id = @company_id" };
        var parameters = new List<NpgsqlParameter>
        {
            new("@company_id", companyId),
            new("@offset", (pageNumber - 1) * pageSize),
            new("@limit", pageSize)
        };

        // Add filters
        if (userIds != null && userIds.Any())
        {
            whereConditions.Add("u.id = ANY(@user_ids)");
            parameters.Add(new("@user_ids", userIds.ToArray()));
        }

        if (status.HasValue)
        {
            whereConditions.Add("u.employee_status = @status");
            parameters.Add(new("@status", (int)status.Value));
        }

        if (teamId.HasValue)
        {
            whereConditions.Add("u.team_id = @team_id");
            parameters.Add(new("@team_id", teamId.Value));
        }

        var whereClause = string.Join(" AND ", whereConditions);

        var query = $@"
            SELECT u.id, u.email, u.first_name, u.last_name, u.profile_picture_url, u.is_active,
                   u.user_mode, u.employee_status, u.team_id, u.department, u.job_title,
                   u.date_hired, u.date_of_birth, u.total_points, u.available_points,
                   u.company_id, u.created_at, u.updated_at, u.last_synced_at, u.sync_source,
                   u.preferred_first_name, c.name as company_name
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
            users.Add(MapUserFromReader(reader));
        }

        return users;
    }

    public async Task<List<User>> GetUserAutocompleteAsync(Guid companyId, string searchTerm, int limit = 3)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            SELECT u.id, u.email, u.first_name, u.last_name, u.profile_picture_url, u.is_active,
                   u.user_mode, u.employee_status, u.team_id, u.department, u.job_title,
                   u.date_hired, u.date_of_birth, u.total_points, u.available_points,
                   u.company_id, u.created_at, u.updated_at, u.last_synced_at, u.sync_source,
                   u.preferred_first_name, c.name as company_name
            FROM users u
            LEFT JOIN companies c ON u.company_id = c.id
            WHERE u.company_id = @company_id 
              AND u.is_active = true
              AND u.employee_status = 0
              AND (
                  LOWER(u.first_name) LIKE LOWER(@search_term) || '%' 
                  OR LOWER(u.last_name) LIKE LOWER(@search_term) || '%'
                  OR LOWER(u.email) LIKE LOWER(@search_term) || '%'
                  OR LOWER(u.preferred_first_name) LIKE LOWER(@search_term) || '%'
              )
            ORDER BY 
                CASE 
                    WHEN LOWER(u.first_name) LIKE LOWER(@search_term) || '%' THEN 1
                    WHEN LOWER(u.preferred_first_name) LIKE LOWER(@search_term) || '%' THEN 2
                    WHEN LOWER(u.last_name) LIKE LOWER(@search_term) || '%' THEN 3
                    ELSE 4
                END,
                u.first_name, u.last_name
            LIMIT @limit";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@company_id", companyId);
        command.Parameters.AddWithValue("@search_term", searchTerm);
        command.Parameters.AddWithValue("@limit", limit);

        var users = new List<User>();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            users.Add(MapUserFromReader(reader));
        }

        return users;
    }

    public async Task<List<User>> GetTeammatesAsync(Guid userId, Guid companyId)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        // First get the user's team_id
        var getTeamQuery = "SELECT team_id FROM users WHERE id = @user_id";
        using var getTeamCommand = new NpgsqlCommand(getTeamQuery, connection);
        getTeamCommand.Parameters.AddWithValue("@user_id", userId);

        var teamId = await getTeamCommand.ExecuteScalarAsync() as Guid?;
        
        if (!teamId.HasValue)
        {
            return new List<User>(); // User has no team, return empty list
        }

        // Get all users in the same team except the requesting user
        var query = @"
            SELECT u.id, u.email, u.first_name, u.last_name, u.profile_picture_url, u.is_active,
                   u.user_mode, u.employee_status, u.team_id, u.department, u.job_title,
                   u.date_hired, u.date_of_birth, u.total_points, u.available_points,
                   u.company_id, u.created_at, u.updated_at, u.last_synced_at, u.sync_source,
                   u.preferred_first_name, c.name as company_name
            FROM users u
            LEFT JOIN companies c ON u.company_id = c.id
            WHERE u.company_id = @company_id 
              AND u.team_id = @team_id 
              AND u.id != @user_id
              AND u.is_active = true
              AND u.employee_status = 0
            ORDER BY u.first_name, u.last_name";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@company_id", companyId);
        command.Parameters.AddWithValue("@team_id", teamId.Value);
        command.Parameters.AddWithValue("@user_id", userId);

        var users = new List<User>();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            users.Add(MapUserFromReader(reader));
        }

        return users;
    }

    /// <summary>
    /// Upsert user from JWT claims - creates or updates user based on JWT token data
    /// This is the primary method for populating user data from authentication tokens
    /// </summary>
    public async Task<User?> UpsertUserFromJwtAsync(
        Guid userId, 
        string email, 
        string firstName, 
        string lastName, 
        Guid companyId, 
        string? department, 
        UserMode userMode, 
        EmployeeStatus employeeStatus)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            INSERT INTO users (
                id, email, first_name, last_name, company_id, department,
                user_mode, employee_status, is_active, sync_source, last_synced_at,
                created_at, updated_at, total_points, available_points
            ) VALUES (
                @id, @email, @first_name, @last_name, @company_id, @department,
                @user_mode, @employee_status, true, 'jwt_claims', CURRENT_TIMESTAMP,
                CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 0, 0
            )
            ON CONFLICT (id) DO UPDATE SET
                email = EXCLUDED.email,
                first_name = EXCLUDED.first_name,
                last_name = EXCLUDED.last_name,
                department = COALESCE(EXCLUDED.department, users.department),
                user_mode = EXCLUDED.user_mode,
                employee_status = EXCLUDED.employee_status,
                last_synced_at = CURRENT_TIMESTAMP,
                updated_at = CURRENT_TIMESTAMP
            RETURNING id, email, first_name, last_name, profile_picture_url, is_active,
                      user_mode, employee_status, team_id, department, job_title,
                      date_hired, date_of_birth, total_points, available_points,
                      company_id, created_at, updated_at, last_synced_at, sync_source, preferred_first_name";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", userId);
        command.Parameters.AddWithValue("@email", email);
        command.Parameters.AddWithValue("@first_name", firstName);
        command.Parameters.AddWithValue("@last_name", lastName);
        command.Parameters.AddWithValue("@company_id", companyId);
        command.Parameters.AddWithValue("@department", (object?)department ?? DBNull.Value);
        command.Parameters.AddWithValue("@user_mode", (int)userMode);
        command.Parameters.AddWithValue("@employee_status", (int)employeeStatus);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapUserFromReader(reader);
        }

        return null;
    }

    /// <summary>
    /// Helper method to map database reader to User model
    /// </summary>
    private User MapUserFromReader(NpgsqlDataReader reader)
    {
        return new User
        {
            Id = reader.GetGuid(0),
            Email = reader.GetString(1),
            FirstName = reader.GetString(2),
            LastName = reader.GetString(3),
            ProfilePictureUrl = reader.IsDBNull(4) ? null : reader.GetString(4),
            IsActive = reader.GetBoolean(5),
            UserMode = (UserMode)reader.GetInt32(6),
            EmployeeStatus = (EmployeeStatus)reader.GetInt32(7),
            TeamId = reader.IsDBNull(8) ? null : reader.GetGuid(8),
            Department = reader.IsDBNull(9) ? null : reader.GetString(9),
            JobTitle = reader.IsDBNull(10) ? null : reader.GetString(10),
            DateHired = reader.IsDBNull(11) ? null : reader.GetDateTime(11),
            DateOfBirth = reader.IsDBNull(12) ? null : reader.GetDateTime(12),
            TotalPoints = reader.GetInt32(13),
            AvailablePoints = reader.GetInt32(14),
            CompanyId = reader.GetGuid(15),
            CreatedAt = reader.GetDateTime(16),
            UpdatedAt = reader.GetDateTime(17),
            LastSyncedAt = reader.IsDBNull(18) ? null : reader.GetDateTime(18),
            SyncSource = reader.IsDBNull(19) ? "jwt_claims" : reader.GetString(19),
            PreferredFirstName = reader.IsDBNull(20) ? null : reader.GetString(20),
            CompanyName = reader.IsDBNull(21) ? string.Empty : reader.GetString(21)
        };
    }
}
