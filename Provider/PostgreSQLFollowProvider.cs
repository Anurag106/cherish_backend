using Domain.Models;
using Domain.Providers;
using Npgsql;
using NpgsqlTypes;
using Microsoft.Extensions.Configuration;

namespace Provider;

public class PostgreSQLFollowProvider : IFollowProvider
{
    private readonly NpgsqlDataSource _dataSource;

    public PostgreSQLFollowProvider(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    // User-to-User Follow Operations
    public async Task<UserFollowUser?> GetUserFollowUserAsync(Guid companyId, Guid followerUserId, Guid followeeUserId)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            SELECT company_id, follower_user_id, followee_user_id, last_modified, is_followed
            FROM user_follow_user
            WHERE company_id = @company_id 
              AND follower_user_id = @follower_user_id 
              AND followee_user_id = @followee_user_id";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@company_id", companyId);
        command.Parameters.AddWithValue("@follower_user_id", followerUserId);
        command.Parameters.AddWithValue("@followee_user_id", followeeUserId);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new UserFollowUser
            {
                CompanyId = reader.GetGuid(0),
                FollowerUserId = reader.GetGuid(1),
                FolloweeUserId = reader.GetGuid(2),
                LastModified = reader.GetDateTime(3),
                IsFollowed = reader.GetBoolean(4)
            };
        }
        return null;
    }

    public async Task<UserFollowUser> CreateOrUpdateUserFollowUserAsync(Guid companyId, Guid followerUserId, Guid followeeUserId, bool isFollowing)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            INSERT INTO user_follow_user (company_id, follower_user_id, followee_user_id, last_modified, is_followed)
            VALUES (@company_id, @follower_user_id, @followee_user_id, CURRENT_TIMESTAMP, @is_followed)
            ON CONFLICT (follower_user_id, followee_user_id, company_id) 
            DO UPDATE SET 
                is_followed = @is_followed,
                last_modified = CURRENT_TIMESTAMP
            RETURNING company_id, follower_user_id, followee_user_id, last_modified, is_followed";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@company_id", companyId);
        command.Parameters.AddWithValue("@follower_user_id", followerUserId);
        command.Parameters.AddWithValue("@followee_user_id", followeeUserId);
        command.Parameters.AddWithValue("@is_followed", isFollowing);

        using var reader = await command.ExecuteReaderAsync();
        await reader.ReadAsync();

        return new UserFollowUser
        {
            CompanyId = reader.GetGuid(0),
            FollowerUserId = reader.GetGuid(1),
            FolloweeUserId = reader.GetGuid(2),
            LastModified = reader.GetDateTime(3),
            IsFollowed = reader.GetBoolean(4)
        };
    }

    public async Task<List<UserFollowInfo>> GetUserFollowingUsersAsync(Guid companyId, Guid followerUserId)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            SELECT u.id, u.first_name, u.last_name, u.email, u.department, uf.last_modified
            FROM user_follow_user uf
            JOIN users u ON u.id = uf.followee_user_id
            WHERE uf.company_id = @company_id 
              AND uf.follower_user_id = @follower_user_id 
              AND uf.is_followed = true
            ORDER BY uf.last_modified DESC";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@company_id", companyId);
        command.Parameters.AddWithValue("@follower_user_id", followerUserId);

        var following = new List<UserFollowInfo>();
        using var reader = await command.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
        {
            following.Add(new UserFollowInfo
            {
                UserId = reader.GetGuid(0),
                FirstName = reader.GetString(1),
                LastName = reader.GetString(2),
                Email = reader.GetString(3),
                Department = reader.IsDBNull(4) ? null : reader.GetString(4),
                LastModified = reader.GetDateTime(5)
            });
        }
        return following;
    }

    public async Task<List<UserFollowInfo>> GetUserFollowersAsync(Guid companyId, Guid followeeUserId)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            SELECT u.id, u.first_name, u.last_name, u.email, u.department, uf.last_modified
            FROM user_follow_user uf
            JOIN users u ON u.id = uf.follower_user_id
            WHERE uf.company_id = @company_id 
              AND uf.followee_user_id = @followee_user_id 
              AND uf.is_followed = true
            ORDER BY uf.last_modified DESC";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@company_id", companyId);
        command.Parameters.AddWithValue("@followee_user_id", followeeUserId);

        var followers = new List<UserFollowInfo>();
        using var reader = await command.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
        {
            followers.Add(new UserFollowInfo
            {
                UserId = reader.GetGuid(0),
                FirstName = reader.GetString(1),
                LastName = reader.GetString(2),
                Email = reader.GetString(3),
                Department = reader.IsDBNull(4) ? null : reader.GetString(4),
                LastModified = reader.GetDateTime(5)
            });
        }
        return followers;
    }

    // User-to-Team Follow Operations
    public async Task<UserFollowTeam?> GetUserFollowTeamAsync(Guid companyId, Guid followerUserId, Guid followeeTeamId)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            SELECT company_id, follower_user_id, followee_team_id, last_modified, is_followed
            FROM user_follow_team
            WHERE company_id = @company_id 
              AND follower_user_id = @follower_user_id 
              AND followee_team_id = @followee_team_id";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@company_id", companyId);
        command.Parameters.AddWithValue("@follower_user_id", followerUserId);
        command.Parameters.AddWithValue("@followee_team_id", followeeTeamId);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new UserFollowTeam
            {
                CompanyId = reader.GetGuid(0),
                FollowerUserId = reader.GetGuid(1),
                FolloweeTeamId = reader.GetGuid(2),
                LastModified = reader.GetDateTime(3),
                IsFollowed = reader.GetBoolean(4)
            };
        }
        return null;
    }

    public async Task<UserFollowTeam> CreateOrUpdateUserFollowTeamAsync(Guid companyId, Guid followerUserId, Guid followeeTeamId, bool isFollowing)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            INSERT INTO user_follow_team (company_id, follower_user_id, followee_team_id, last_modified, is_followed)
            VALUES (@company_id, @follower_user_id, @followee_team_id, CURRENT_TIMESTAMP, @is_followed)
            ON CONFLICT (follower_user_id, followee_team_id, company_id) 
            DO UPDATE SET 
                is_followed = @is_followed,
                last_modified = CURRENT_TIMESTAMP
            RETURNING company_id, follower_user_id, followee_team_id, last_modified, is_followed";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@company_id", companyId);
        command.Parameters.AddWithValue("@follower_user_id", followerUserId);
        command.Parameters.AddWithValue("@followee_team_id", followeeTeamId);
        command.Parameters.AddWithValue("@is_followed", isFollowing);

        using var reader = await command.ExecuteReaderAsync();
        await reader.ReadAsync();

        return new UserFollowTeam
        {
            CompanyId = reader.GetGuid(0),
            FollowerUserId = reader.GetGuid(1),
            FolloweeTeamId = reader.GetGuid(2),
            LastModified = reader.GetDateTime(3),
            IsFollowed = reader.GetBoolean(4)
        };
    }

    public async Task<List<TeamFollowInfo>> GetUserFollowingTeamsAsync(Guid companyId, Guid followerUserId)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            SELECT t.id, t.name, t.manager_id, 
                   CONCAT(u.first_name, ' ', u.last_name) as manager_name,
                   (SELECT COUNT(*) FROM users WHERE team_id = t.id) as member_count,
                   utf.last_modified
            FROM user_follow_team utf
            JOIN teams t ON t.id = utf.followee_team_id
            JOIN users u ON u.id = t.manager_id
            WHERE utf.company_id = @company_id 
              AND utf.follower_user_id = @follower_user_id 
              AND utf.is_followed = true
            ORDER BY utf.last_modified DESC";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@company_id", companyId);
        command.Parameters.AddWithValue("@follower_user_id", followerUserId);

        var following = new List<TeamFollowInfo>();
        using var reader = await command.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
        {
            following.Add(new TeamFollowInfo
            {
                TeamId = reader.GetGuid(0),
                TeamName = reader.GetString(1),
                ManagerId = reader.GetGuid(2),
                ManagerName = reader.GetString(3),
                MemberCount = reader.GetInt32(4),
                LastModified = reader.GetDateTime(5)
            });
        }
        return following;
    }

    public async Task<List<UserFollowInfo>> GetTeamFollowersAsync(Guid companyId, Guid followeeTeamId)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            SELECT u.id, u.first_name, u.last_name, u.email, u.department, utf.last_modified
            FROM user_follow_team utf
            JOIN users u ON u.id = utf.follower_user_id
            WHERE utf.company_id = @company_id 
              AND utf.followee_team_id = @followee_team_id 
              AND utf.is_followed = true
            ORDER BY utf.last_modified DESC";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@company_id", companyId);
        command.Parameters.AddWithValue("@followee_team_id", followeeTeamId);

        var followers = new List<UserFollowInfo>();
        using var reader = await command.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
        {
            followers.Add(new UserFollowInfo
            {
                UserId = reader.GetGuid(0),
                FirstName = reader.GetString(1),
                LastName = reader.GetString(2),
                Email = reader.GetString(3),
                Department = reader.IsDBNull(4) ? null : reader.GetString(4),
                LastModified = reader.GetDateTime(5)
            });
        }
        return followers;
    }

    // Validation Methods
    public async Task<bool> IsUserInSameCompanyAsync(Guid userId1, Guid userId2)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = @"
            SELECT COUNT(*) FROM users u1
            JOIN users u2 ON u1.company_id = u2.company_id
            WHERE u1.id = @user_id_1 AND u2.id = @user_id_2";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@user_id_1", userId1);
        command.Parameters.AddWithValue("@user_id_2", userId2);

        var count = await command.ExecuteScalarAsync();
        return Convert.ToInt32(count) > 0;
    }

    public async Task<bool> IsUserInCompanyAsync(Guid userId, Guid companyId)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = "SELECT COUNT(*) FROM users WHERE id = @user_id AND company_id = @company_id";
        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@user_id", userId);
        command.Parameters.AddWithValue("@company_id", companyId);

        var count = await command.ExecuteScalarAsync();
        return Convert.ToInt32(count) > 0;
    }

    public async Task<bool> IsTeamInCompanyAsync(Guid teamId, Guid companyId)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = "SELECT COUNT(*) FROM teams WHERE id = @team_id AND company_id = @company_id";
        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@team_id", teamId);
        command.Parameters.AddWithValue("@company_id", companyId);

        var count = await command.ExecuteScalarAsync();
        return Convert.ToInt32(count) > 0;
    }

    public async Task<bool> IsUserInTeamAsync(Guid userId, Guid teamId)
    {
        using var connection = _dataSource.CreateConnection();
        await connection.OpenAsync();

        var query = "SELECT COUNT(*) FROM users WHERE id = @user_id AND team_id = @team_id";
        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@user_id", userId);
        command.Parameters.AddWithValue("@team_id", teamId);

        var count = await command.ExecuteScalarAsync();
        return Convert.ToInt32(count) > 0;
    }
}
