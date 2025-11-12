using Microsoft.Extensions.Configuration;
using Npgsql;
using System.CommandLine;

namespace Database;

public class DatabaseManager
{
    public static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("Cherish Database Schema Manager");

        var initCommand = new Command("init", "Initialize database schema with all tables");
        var statusCommand = new Command("status", "Show database schema status");

        rootCommand.AddCommand(initCommand);
        rootCommand.AddCommand(statusCommand);

        initCommand.SetHandler(async () =>
        {
            try
            {
                Console.WriteLine("🔧 Cherish Database Schema Manager");
                Console.WriteLine("==================================");
                
                var configuration = BuildConfiguration();
                await EnsureDatabaseExistsAsync(configuration);

                var schemaManager = new DatabaseSchemaManager(configuration);
                
                await schemaManager.InitializeDatabaseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Database initialization failed: {ex.Message}");
                Environment.Exit(1);
            }
        });

        statusCommand.SetHandler(async () =>
        {
            try
            {
                var configuration = BuildConfiguration();
                var schemaManager = new DatabaseSchemaManager(configuration);
                
                await schemaManager.GetSchemaStatusAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to get schema status: {ex.Message}");
                Environment.Exit(1);
            }
        });

        return await rootCommand.InvokeAsync(args);
    }

    private static async Task EnsureDatabaseExistsAsync(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgreSQL");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'PostgreSQL' not found in configuration.");
        }

        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        var databaseName = builder.Database;

        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new InvalidOperationException("Database name not specified in PostgreSQL connection string.");
        }

        // Connect to the default 'postgres' database to check/create the target database
        builder.Database = "postgres";

        using var adminConnection = new NpgsqlConnection(builder.ConnectionString);
        await adminConnection.OpenAsync();

        var checkCommandText = "SELECT 1 FROM pg_database WHERE datname = @dbname";
        using var checkCommand = new NpgsqlCommand(checkCommandText, adminConnection);
        checkCommand.Parameters.AddWithValue("@dbname", databaseName);

        var exists = await checkCommand.ExecuteScalarAsync();

        if (exists == null)
        {
            Console.WriteLine($"📦 Creating database '{databaseName}'...");
            var createCommandText = $"CREATE DATABASE \"{databaseName}\"";
            using var createCommand = new NpgsqlCommand(createCommandText, adminConnection);
            await createCommand.ExecuteNonQueryAsync();
            Console.WriteLine($"✅ Database '{databaseName}' created successfully.");
        }
        else
        {
            Console.WriteLine($"ℹ️ Database '{databaseName}' already exists.");
        }
    }

    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("RestApi/appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("RestApi/appsettings.Development.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }
}
