using System;
using System.Threading.Tasks;

namespace Scripts;

public class SeedDataRunner
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("🌱 Cherish Seed Data Generator");
        Console.WriteLine("==============================");
        
        var baseUrl = args.Length > 0 ? args[0] : "https://localhost:7000";
        Console.WriteLine($"📍 Target API: {baseUrl}");
        Console.WriteLine();

        using var seedScript = new SeedDataScript(baseUrl);
        
        try
        {
            await seedScript.RunAsync();
            
            Console.WriteLine();
            Console.WriteLine("🎉 Seed data generation completed!");
            Console.WriteLine();
            Console.WriteLine("📊 Summary:");
            Console.WriteLine("- 2 Companies created");
            Console.WriteLine("- 20 Users created (10 per company, 3 managers each)");
            Console.WriteLine("- 6 Teams created (3 per company)");
            Console.WriteLine("- 20 Hashtags created (10 per company)");
            Console.WriteLine("- 60 Posts created (30 per company)");
            Console.WriteLine("- 100+ Reactions created (5 emoji types)");
            Console.WriteLine("- 50+ Comments created (with mentions and points)");
            Console.WriteLine();
            Console.WriteLine("🚀 Your Cherish backend is now populated with realistic test data!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
            Console.WriteLine();
            Console.WriteLine("💡 Make sure:");
            Console.WriteLine("1. The Cherish API is running");
            Console.WriteLine("2. The database is initialized");
            Console.WriteLine("3. The API URL is correct");
            Environment.Exit(1);
        }
    }
}
