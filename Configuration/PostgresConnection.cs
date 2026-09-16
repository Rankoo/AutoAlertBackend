using System.Text;

namespace AutoAlertBackEnd.Configuration;

public static class PostgresConnection
{
    public static string Resolve(IConfiguration configuration)
    {
        var raw =
            configuration.GetConnectionString("DefaultConnection")
            ?? configuration.GetConnectionString("SQLConnectionStrings")
            ?? configuration["DATABASE_URL"]
            ?? Environment.GetEnvironmentVariable("DATABASE_URL");

        if (string.IsNullOrWhiteSpace(raw))
        {
            throw new InvalidOperationException(
                "Falta la cadena de conexión de PostgreSQL. Configure ConnectionStrings:DefaultConnection, ConnectionStrings:SQLConnectionStrings o DATABASE_URL.");
        }

        return Normalize(raw);
    }

    public static string Normalize(string connectionString)
    {
        if (!connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase)
            && !connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
        {
            return connectionString;
        }

        var uri = new Uri(connectionString);
        var userInfo = uri.UserInfo.Split(':', 2);
        var username = Uri.UnescapeDataString(userInfo[0]);
        var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty;
        var database = uri.AbsolutePath.Trim('/');
        var sslMode = IsLocalHost(uri.Host) ? "Disable" : "Require";

        var builder = new StringBuilder()
            .Append("Host=").Append(uri.Host)
            .Append(";Port=").Append(uri.Port > 0 ? uri.Port : 5432)
            .Append(";Database=").Append(database)
            .Append(";Username=").Append(username)
            .Append(";Password=").Append(password)
            .Append(";SSL Mode=").Append(sslMode);

        if (sslMode == "Require")
        {
            builder.Append(";Trust Server Certificate=true");
        }

        return builder.ToString();
    }

    private static bool IsLocalHost(string host) =>
        host.Equals("localhost", StringComparison.OrdinalIgnoreCase)
        || host.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase)
        || host.Equals("::1", StringComparison.OrdinalIgnoreCase);
}
