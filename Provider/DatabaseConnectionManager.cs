using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Web;

namespace Provider;

public static class DatabaseConnectionManager
{
    public static string GetConnectionString(IConfiguration configuration)
    {
        var raw = configuration.GetConnectionString("PostgreSQL")
                  ?? throw new ArgumentNullException("PostgreSQL connection string not found");

        return NormalizePgConnectionString(raw);
    }

    private static string NormalizePgConnectionString(string value)
    {
        if (!value.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) &&
            !value.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
            return value; // already key=value; format

        var uri = new Uri(value);

        var userInfo = uri.UserInfo.Split(':', 2);
        var username = Uri.UnescapeDataString(userInfo[0]);
        var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";

        var db = uri.AbsolutePath.Trim('/');
        if (string.IsNullOrEmpty(db)) db = "postgres";

        var q = HttpUtility.ParseQueryString(uri.Query);

        // Build a safe Npgsql key-value string
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port,
            Database = db,
            Username = username,
            Password = password
        };

        // Carry over common query params if present
        var sslmode = q.Get("sslmode");
        if (!string.IsNullOrEmpty(sslmode) &&
            Enum.TryParse<Npgsql.SslMode>(sslmode, true, out var mode))
        {
            builder.SslMode = mode;
        }
        else
        {
            // Aiven typically needs TLS
            builder.SslMode = SslMode.Require;
        }

        // DEV convenience; remove in prod or install the CA cert from Aiven
        if (string.Equals(q.Get("trust_server_certificate"), "true", StringComparison.OrdinalIgnoreCase))
            builder.TrustServerCertificate = true;

        return builder.ConnectionString;
    }
}
