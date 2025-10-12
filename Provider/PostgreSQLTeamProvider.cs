using Domain.Providers;
using Domain.Models;
using Npgsql;
using Microsoft.Extensions.Configuration;

namespace Provider;

public class PostgreSQLTeamProvider : ITeamProvider
{
    private readonly NpgsqlDataSource _dataSource;

    public PostgreSQLTeamProvider(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }


    public async Task<Team?> GetTeamByIdAsync(Guid id)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            SELECT t.id, t.name, t.manager_id, t.company_id, t.employee_ids, t.created_at, t.updated_at
            FROM teams t
            WHERE t.id = @id";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            var team = new Team
            {
                Id = reader.GetGuid(0),
                Name = reader.GetString(1),
                ManagerId = reader.GetGuid(2),
                CompanyId = reader.GetGuid(3),
                EmployeeIds = reader.GetFieldValue<List<Guid>>(4),
                CreatedAt = reader.GetDateTime(5),
                UpdatedAt = reader.GetDateTime(6)
            };

            return team;
        }

        return null;
    }

    public async Task<Team?> GetTeamByNameAsync(string name)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            SELECT t.id, t.name, t.manager_id, t.company_id, t.employee_ids, t.created_at, t.updated_at
            FROM teams t
            WHERE t.name = @name";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@name", name);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            var team = new Team
            {
                Id = reader.GetGuid(0),
                Name = reader.GetString(1),
                ManagerId = reader.GetGuid(2),
                CompanyId = reader.GetGuid(3),
                EmployeeIds = reader.GetFieldValue<List<Guid>>(4),
                CreatedAt = reader.GetDateTime(5),
                UpdatedAt = reader.GetDateTime(6)
            };

            return team;
        }

        return null;
    }

    public async Task<List<Team>> GetTeamsByCompanyIdAsync(Guid companyId)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            SELECT t.id, t.name, t.manager_id, t.company_id, t.employee_ids, t.created_at, t.updated_at
            FROM teams t
            WHERE t.company_id = @company_id
            ORDER BY t.name";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@company_id", companyId);

        var teams = new List<Team>();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var team = new Team
            {
                Id = reader.GetGuid(0),
                Name = reader.GetString(1),
                ManagerId = reader.GetGuid(2),
                CompanyId = reader.GetGuid(3),
                EmployeeIds = reader.GetFieldValue<List<Guid>>(4),
                CreatedAt = reader.GetDateTime(5),
                UpdatedAt = reader.GetDateTime(6)
            };

            teams.Add(team);
        }

        return teams;
    }

    public async Task<List<Team>> GetTeamsByManagerIdAsync(Guid managerId)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            SELECT t.id, t.name, t.manager_id, t.company_id, t.employee_ids, t.created_at, t.updated_at
            FROM teams t
            WHERE t.manager_id = @manager_id
            ORDER BY t.name";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@manager_id", managerId);

        var teams = new List<Team>();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var team = new Team
            {
                Id = reader.GetGuid(0),
                Name = reader.GetString(1),
                ManagerId = reader.GetGuid(2),
                CompanyId = reader.GetGuid(3),
                EmployeeIds = reader.GetFieldValue<List<Guid>>(4),
                CreatedAt = reader.GetDateTime(5),
                UpdatedAt = reader.GetDateTime(6)
            };

            teams.Add(team);
        }

        return teams;
    }

    public async Task<Team> CreateTeamAsync(string name, Guid managerId, Guid companyId)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            INSERT INTO teams (name, manager_id, company_id, employee_ids, created_at, updated_at) 
            VALUES (@name, @manager_id, @company_id, @employee_ids, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP) 
            RETURNING id, name, manager_id, company_id, employee_ids, created_at, updated_at";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@name", name);
        command.Parameters.AddWithValue("@manager_id", managerId);
        command.Parameters.AddWithValue("@company_id", companyId);
        command.Parameters.AddWithValue("@employee_ids", System.Text.Json.JsonSerializer.Serialize(new List<Guid>()));

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            var team = new Team
            {
                Id = reader.GetGuid(0),
                Name = reader.GetString(1),
                ManagerId = reader.GetGuid(2),
                CompanyId = reader.GetGuid(3),
                EmployeeIds = reader.GetFieldValue<List<Guid>>(4),
                CreatedAt = reader.GetDateTime(5),
                UpdatedAt = reader.GetDateTime(6)
            };

            return team;
        }

        throw new Exception("Failed to create team");
    }

    public async Task<Team> UpdateTeamAsync(Guid id, string name, Guid managerId)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            UPDATE teams 
            SET name = @name, manager_id = @manager_id, updated_at = CURRENT_TIMESTAMP 
            WHERE id = @id 
            RETURNING id, name, manager_id, company_id, employee_ids, created_at, updated_at";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@name", name);
        command.Parameters.AddWithValue("@manager_id", managerId);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            var team = new Team
            {
                Id = reader.GetGuid(0),
                Name = reader.GetString(1),
                ManagerId = reader.GetGuid(2),
                CompanyId = reader.GetGuid(3),
                EmployeeIds = reader.GetFieldValue<List<Guid>>(4),
                CreatedAt = reader.GetDateTime(5),
                UpdatedAt = reader.GetDateTime(6)
            };

            return team;
        }

        throw new Exception("Team not found or update failed");
    }

    public async Task<bool> DeleteTeamAsync(Guid id)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = "DELETE FROM teams WHERE id = @id";
        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> AddEmployeeToTeamAsync(Guid teamId, Guid employeeId)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        // First get the current team to check if employee already exists
        var getTeamQuery = "SELECT employee_ids FROM teams WHERE id = @team_id";
        using var getCommand = new NpgsqlCommand(getTeamQuery, connection);
        getCommand.Parameters.AddWithValue("@team_id", teamId);

        using var reader = await getCommand.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return false; // Team not found
        }

        var currentEmployeeIds = reader.GetFieldValue<List<Guid>>(0);
        
        // Check if employee already exists
        if (currentEmployeeIds.Contains(employeeId))
        {
            return false; // Employee already in team
        }

        await reader.CloseAsync();

        // Add employee to the JSON array
        currentEmployeeIds.Add(employeeId);
        var updatedEmployeeIdsJson = System.Text.Json.JsonSerializer.Serialize(currentEmployeeIds);

        var updateQuery = @"
            UPDATE teams 
            SET employee_ids = @employee_ids, updated_at = CURRENT_TIMESTAMP 
            WHERE id = @team_id";

        using var updateCommand = new NpgsqlCommand(updateQuery, connection);
        updateCommand.Parameters.AddWithValue("@team_id", teamId);
        updateCommand.Parameters.AddWithValue("@employee_ids", updatedEmployeeIdsJson);

        var rowsAffected = await updateCommand.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> RemoveEmployeeFromTeamAsync(Guid teamId, Guid employeeId)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        // First get the current team to check if employee exists
        var getTeamQuery = "SELECT employee_ids FROM teams WHERE id = @team_id";
        using var getCommand = new NpgsqlCommand(getTeamQuery, connection);
        getCommand.Parameters.AddWithValue("@team_id", teamId);

        using var reader = await getCommand.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return false; // Team not found
        }

        var currentEmployeeIds = reader.GetFieldValue<List<Guid>>(0);
        
        // Check if employee exists and remove it
        if (!currentEmployeeIds.Remove(employeeId))
        {
            return false; // Employee not in team
        }

        await reader.CloseAsync();

        var updatedEmployeeIdsJson = System.Text.Json.JsonSerializer.Serialize(currentEmployeeIds);

        var updateQuery = @"
            UPDATE teams 
            SET employee_ids = @employee_ids, updated_at = CURRENT_TIMESTAMP 
            WHERE id = @team_id";

        using var updateCommand = new NpgsqlCommand(updateQuery, connection);
        updateCommand.Parameters.AddWithValue("@team_id", teamId);
        updateCommand.Parameters.AddWithValue("@employee_ids", updatedEmployeeIdsJson);

        var rowsAffected = await updateCommand.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<List<Team>> GetAllTeamsAsync()
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            SELECT t.id, t.name, t.manager_id, t.company_id, t.employee_ids, t.created_at, t.updated_at
            FROM teams t
            ORDER BY t.name";

        using var command = new NpgsqlCommand(query, connection);

        var teams = new List<Team>();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var team = new Team
            {
                Id = reader.GetGuid(0),
                Name = reader.GetString(1),
                ManagerId = reader.GetGuid(2),
                CompanyId = reader.GetGuid(3),
                EmployeeIds = reader.GetFieldValue<List<Guid>>(4),
                CreatedAt = reader.GetDateTime(5),
                UpdatedAt = reader.GetDateTime(6)
            };

            teams.Add(team);
        }

        return teams;
    }

}
