using Microsoft.Extensions.Configuration;
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
