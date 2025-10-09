using Npgsql;
using Microsoft.Extensions.Configuration;
using System.Web;

namespace Database;

public class DatabaseSchemaManager
{
    private readonly string _connectionString;

    public DatabaseSchemaManager(IConfiguration configuration)
    {
        _connectionString = Provider.DatabaseConnectionManager.GetConnectionString(configuration);
    }

    public async Task InitializeDatabaseAsync()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        Console.WriteLine("🚀 Initializing Cherish Database Schema...");
        Console.WriteLine("==========================================");

        try
        {
            // Execute schema creation in proper dependency order
            await CreateCompaniesTableAsync(connection);
            await CreateUsersTableAsync(connection);
            await CreateTeamsTableAsync(connection);
            await CreateHashtagsTableAsync(connection);
            await CreateTransactionsTableAsync(connection);
            await CreatePostsTableAsync(connection);
            await CreateCommentsTableAsync(connection);
            await CreateReactionsTableAsync(connection);
            await CreateUserFollowUserTableAsync(connection);
            await CreateUserFollowTeamTableAsync(connection);
            await CreateOptimizedIndexesAsync(connection);

            Console.WriteLine("✅ Database schema initialization completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Database schema initialization failed: {ex.Message}");
            throw;
        }
    }


    private async Task CreateCompaniesTableAsync(NpgsqlConnection connection)
    {
        Console.WriteLine("📋 Creating companies table...");
        
        var createTableCommand = @"
            CREATE TABLE IF NOT EXISTS companies (
                id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                name VARCHAR(100) UNIQUE NOT NULL,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );";

        using var command = new NpgsqlCommand(createTableCommand, connection);
        await command.ExecuteNonQueryAsync();
        
        Console.WriteLine("✅ Companies table created/verified");
    }

    private async Task CreateUsersTableAsync(NpgsqlConnection connection)
    {
        Console.WriteLine("👥 Creating users table...");
        
        // Check if table exists and needs migration
        var checkTableCommand = @"
            SELECT column_name, data_type
            FROM information_schema.columns
            WHERE table_name = 'users' AND column_name = 'id'";

        using var checkCommand = new NpgsqlCommand(checkTableCommand, connection);
        using var reader = await checkCommand.ExecuteReaderAsync();

        bool needsMigration = false;
        if (await reader.ReadAsync())
        {
            var dataType = reader.GetString(1);
            if (dataType == "integer")
            {
                needsMigration = true;
                Console.WriteLine("🔄 Migrating users table from integer to UUID...");
            }
        }
        await reader.CloseAsync();

        // Check if new columns exist
        var checkNewColumnsCommand = @"
            SELECT COUNT(*) 
            FROM information_schema.columns 
            WHERE table_name = 'users' AND column_name = 'email'";

        using var checkNewColumnsCmd = new NpgsqlCommand(checkNewColumnsCommand, connection);
        var newColumnsExist = await checkNewColumnsCmd.ExecuteScalarAsync();
        var hasNewColumns = Convert.ToInt32(newColumnsExist) > 0;

        if (needsMigration || !hasNewColumns)
        {
            Console.WriteLine("🔄 Dropping and recreating users table with new schema...");
            var dropTableCommand = "DROP TABLE IF EXISTS users CASCADE;";
            using var dropCommand = new NpgsqlCommand(dropTableCommand, connection);
            await dropCommand.ExecuteNonQueryAsync();
        }

        var createTableCommand = @"
            CREATE TABLE IF NOT EXISTS users (
                id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                username VARCHAR(50) UNIQUE NOT NULL,
                password VARCHAR(255) NOT NULL,
                email VARCHAR(255),
                first_name VARCHAR(100),
                last_name VARCHAR(100),
                role INTEGER DEFAULT 0,
                status INTEGER DEFAULT 0,
                team_id UUID,
                department VARCHAR(100),
                job_title VARCHAR(100),
                date_hired DATE,
                date_of_birth DATE,
                total_points INTEGER DEFAULT 0,
                available_points INTEGER DEFAULT 0,
                company_id UUID NOT NULL,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (company_id) REFERENCES companies(id)
            );";

        using var command = new NpgsqlCommand(createTableCommand, connection);
        await command.ExecuteNonQueryAsync();
        
        Console.WriteLine("✅ Users table created/verified");
    }

    private async Task CreateTeamsTableAsync(NpgsqlConnection connection)
    {
        Console.WriteLine("👥 Creating teams table...");
        
        var createTableCommand = @"
            CREATE TABLE IF NOT EXISTS teams (
                id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                name VARCHAR(100) NOT NULL,
                manager_id UUID NOT NULL,
                company_id UUID NOT NULL,
                employee_ids JSONB DEFAULT '[]'::jsonb,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (manager_id) REFERENCES users(id),
                FOREIGN KEY (company_id) REFERENCES companies(id),
                UNIQUE(name, company_id)
            );";

        using var command = new NpgsqlCommand(createTableCommand, connection);
        await command.ExecuteNonQueryAsync();
        
        Console.WriteLine("✅ Teams table created/verified");
    }

    private async Task CreateHashtagsTableAsync(NpgsqlConnection connection)
    {
        Console.WriteLine("🏷️ Creating hashtags table...");
        
        // Check if company_id column exists
        var checkColumnCommand = @"
            SELECT COUNT(*)
            FROM information_schema.columns
            WHERE table_name = 'hashtags' AND column_name = 'company_id'";

        using var checkCommand = new NpgsqlCommand(checkColumnCommand, connection);
        var columnExists = await checkCommand.ExecuteScalarAsync();
        var hasCompanyIdColumn = Convert.ToInt32(columnExists) > 0;

        if (!hasCompanyIdColumn)
        {
            Console.WriteLine("🔄 Migrating hashtags table to include company_id...");
            var dropTableCommand = "DROP TABLE IF EXISTS hashtags CASCADE;";
            using var dropCommand = new NpgsqlCommand(dropTableCommand, connection);
            await dropCommand.ExecuteNonQueryAsync();
        }

        var createTableCommand = @"
            CREATE TABLE IF NOT EXISTS hashtags (
                id SERIAL PRIMARY KEY,
                name VARCHAR(100) NOT NULL,
                description TEXT,
                company_id UUID NOT NULL,
                created_by UUID NOT NULL,
                created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                modified_by UUID,
                modified_date TIMESTAMP,
                FOREIGN KEY (company_id) REFERENCES companies(id),
                FOREIGN KEY (created_by) REFERENCES users(id),
                FOREIGN KEY (modified_by) REFERENCES users(id),
                UNIQUE(name, company_id)
            );";

        using var command = new NpgsqlCommand(createTableCommand, connection);
        await command.ExecuteNonQueryAsync();
        
        Console.WriteLine("✅ Hashtags table created/verified");
    }

    private async Task CreateTransactionsTableAsync(NpgsqlConnection connection)
    {
        Console.WriteLine("💰 Creating transactions table...");
        
        var createTableCommand = @"
            CREATE TABLE IF NOT EXISTS transactions (
                id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                company_id UUID NOT NULL,
                from_user_id UUID NOT NULL,
                to_user_id UUID NOT NULL,
                points INTEGER NOT NULL,
                description TEXT,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (company_id) REFERENCES companies(id),
                FOREIGN KEY (from_user_id) REFERENCES users(id),
                FOREIGN KEY (to_user_id) REFERENCES users(id)
            );";

        using var command = new NpgsqlCommand(createTableCommand, connection);
        await command.ExecuteNonQueryAsync();
        
        Console.WriteLine("✅ Transactions table created/verified");
    }

    private async Task CreatePostsTableAsync(NpgsqlConnection connection)
    {
        Console.WriteLine("📝 Creating posts table...");
        
        var createTableCommand = @"
            CREATE TABLE IF NOT EXISTS posts (
                id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                user_id UUID NOT NULL,
                company_id UUID NOT NULL,
                context TEXT NOT NULL,
                user_mentioned JSONB DEFAULT '[]'::jsonb,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                hashtags JSONB DEFAULT '[]'::jsonb,
                metadata JSONB DEFAULT '{}'::jsonb,
                total_points INTEGER DEFAULT 0,
                visibility INTEGER DEFAULT 0,
                deleted BOOLEAN DEFAULT false,
                FOREIGN KEY (user_id) REFERENCES users(id),
                FOREIGN KEY (company_id) REFERENCES companies(id)
            );";

        using var command = new NpgsqlCommand(createTableCommand, connection);
        await command.ExecuteNonQueryAsync();
        
        Console.WriteLine("✅ Posts table created/verified");
    }

    private async Task CreateCommentsTableAsync(NpgsqlConnection connection)
    {
        Console.WriteLine("💬 Creating comments table...");
        
        var createTableCommand = @"
            CREATE TABLE IF NOT EXISTS comments (
                id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                user_id UUID NOT NULL,
                posted_by_added BOOLEAN NOT NULL DEFAULT false,
                company_id UUID NOT NULL,
                content TEXT NOT NULL,
                points INTEGER DEFAULT 0,
                post_id UUID NOT NULL,
                hashtags JSONB DEFAULT '[]'::jsonb,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                metadata JSONB DEFAULT '{}'::jsonb,
                deleted BOOLEAN DEFAULT false,
                FOREIGN KEY (user_id) REFERENCES users(id),
                FOREIGN KEY (company_id) REFERENCES companies(id),
                FOREIGN KEY (post_id) REFERENCES posts(id)
            );";

        using var command = new NpgsqlCommand(createTableCommand, connection);
        await command.ExecuteNonQueryAsync();
        
        Console.WriteLine("✅ Comments table created/verified");
    }

    private async Task CreateReactionsTableAsync(NpgsqlConnection connection)
    {
        Console.WriteLine("👍 Creating reactions table...");
        
        var createTableCommand = @"
            CREATE TABLE IF NOT EXISTS reactions (
                id BIGSERIAL PRIMARY KEY,
                company_id UUID NOT NULL,
                user_id UUID NOT NULL,
                post_id UUID NOT NULL,
                emoji_type INTEGER NOT NULL CHECK (emoji_type >= 1 AND emoji_type <= 5),
                last_modified_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (company_id) REFERENCES companies(id),
                FOREIGN KEY (user_id) REFERENCES users(id),
                FOREIGN KEY (post_id) REFERENCES posts(id),
                UNIQUE(user_id, post_id)
            );";

        using var command = new NpgsqlCommand(createTableCommand, connection);
        await command.ExecuteNonQueryAsync();
        
        Console.WriteLine("✅ Reactions table created/verified");
    }

    private async Task CreateOptimizedIndexesAsync(NpgsqlConnection connection)
    {
        Console.WriteLine("🚀 Creating optimized indexes...");
        
        // Create indexes one by one to handle errors gracefully
        var indexCommands = new[]
        {
            // Core post indexes
            ("idx_posts_company_created_deleted", @"CREATE INDEX IF NOT EXISTS idx_posts_company_created_deleted 
              ON posts(company_id, created_at DESC, id DESC) 
              WHERE deleted = false"),
            
            ("idx_posts_user_created", @"CREATE INDEX IF NOT EXISTS idx_posts_user_created 
              ON posts(user_id, created_at DESC, id DESC) 
              WHERE deleted = false"),
            
            // JSONB indexes for fast searches
            ("idx_posts_user_mentioned_gin", @"CREATE INDEX IF NOT EXISTS idx_posts_user_mentioned_gin 
              ON posts USING GIN (user_mentioned) 
              WHERE deleted = false"),
            
            ("idx_posts_hashtags_gin", @"CREATE INDEX IF NOT EXISTS idx_posts_hashtags_gin 
              ON posts USING GIN (hashtags) 
              WHERE deleted = false"),
            
            // Reaction indexes
            ("idx_reactions_user_post", @"CREATE INDEX IF NOT EXISTS idx_reactions_user_post 
              ON reactions(user_id, post_id, last_modified_at DESC)"),
            
            ("idx_reactions_post_user", @"CREATE INDEX IF NOT EXISTS idx_reactions_post_user 
              ON reactions(post_id, user_id)"),
            
            // Comment indexes
            ("idx_comments_user_post", @"CREATE INDEX IF NOT EXISTS idx_comments_user_post 
              ON comments(user_id, post_id, created_at DESC) 
              WHERE deleted = false"),
            
            ("idx_comments_post_user", @"CREATE INDEX IF NOT EXISTS idx_comments_post_user 
              ON comments(post_id, user_id) 
              WHERE deleted = false"),
            
            // User team indexes
            ("idx_users_team_id", @"CREATE INDEX IF NOT EXISTS idx_users_team_id 
              ON users(team_id)"),
            
            ("idx_users_company_team", @"CREATE INDEX IF NOT EXISTS idx_users_company_team 
              ON users(company_id, team_id)"),
            
            // Cursor pagination index
            ("idx_posts_cursor_pagination", @"CREATE INDEX IF NOT EXISTS idx_posts_cursor_pagination 
              ON posts(company_id, created_at DESC, id DESC) 
              WHERE deleted = false")
        };

        foreach (var (name, command) in indexCommands)
        {
            try
            {
                using var cmd = new NpgsqlCommand(command, connection);
                await cmd.ExecuteNonQueryAsync();
                Console.WriteLine($"  ✅ {name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ⚠️  {name} - {ex.Message}");
            }
        }
        
        Console.WriteLine("✅ Optimized indexes creation completed");
    }

    private async Task CreateUserFollowUserTableAsync(NpgsqlConnection connection)
    {
        Console.WriteLine("👥 Creating user_follow_user table...");
        
        var createTableCommand = @"
            CREATE TABLE IF NOT EXISTS user_follow_user (
                company_id UUID NOT NULL,
                follower_user_id UUID NOT NULL,
                followee_user_id UUID NOT NULL,
                last_modified TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                is_followed BOOLEAN DEFAULT true,
                
                PRIMARY KEY (follower_user_id, followee_user_id, company_id),
                FOREIGN KEY (company_id) REFERENCES companies(id),
                FOREIGN KEY (follower_user_id) REFERENCES users(id),
                FOREIGN KEY (followee_user_id) REFERENCES users(id),
                CHECK (follower_user_id <> followee_user_id)
            );";

        using var command = new NpgsqlCommand(createTableCommand, connection);
        await command.ExecuteNonQueryAsync();
        
        Console.WriteLine("✅ User follow user table created/verified");
    }

    private async Task CreateUserFollowTeamTableAsync(NpgsqlConnection connection)
    {
        Console.WriteLine("👥 Creating user_follow_team table...");
        
        var createTableCommand = @"
            CREATE TABLE IF NOT EXISTS user_follow_team (
                company_id UUID NOT NULL,
                follower_user_id UUID NOT NULL,
                followee_team_id UUID NOT NULL,
                last_modified TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                is_followed BOOLEAN DEFAULT true,
                
                PRIMARY KEY (follower_user_id, followee_team_id, company_id),
                FOREIGN KEY (company_id) REFERENCES companies(id),
                FOREIGN KEY (follower_user_id) REFERENCES users(id),
                FOREIGN KEY (followee_team_id) REFERENCES teams(id)
            );";

        using var command = new NpgsqlCommand(createTableCommand, connection);
        await command.ExecuteNonQueryAsync();
        
        Console.WriteLine("✅ User follow team table created/verified");
    }

    public async Task GetSchemaStatusAsync()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var tables = new[] { "companies", "users", "teams", "hashtags", "transactions", "posts", "comments", "reactions", "user_follow_user", "user_follow_team" };

        Console.WriteLine("\n📊 Database Schema Status:");
        Console.WriteLine("=========================");

        foreach (var tableName in tables)
        {
            var query = @"
                SELECT COUNT(*) 
                FROM information_schema.tables 
                WHERE table_name = @table_name";

            using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@table_name", tableName);
            var exists = Convert.ToInt32(await command.ExecuteScalarAsync()) > 0;

            if (exists)
            {
                // Get row count
                var countQuery = $"SELECT COUNT(*) FROM {tableName}";
                using var countCommand = new NpgsqlCommand(countQuery, connection);
                var rowCount = await countCommand.ExecuteScalarAsync();
                
                Console.WriteLine($"✅ {tableName} - {rowCount} rows");
            }
            else
            {
                Console.WriteLine($"❌ {tableName} - Table does not exist");
            }
        }
    }
}
