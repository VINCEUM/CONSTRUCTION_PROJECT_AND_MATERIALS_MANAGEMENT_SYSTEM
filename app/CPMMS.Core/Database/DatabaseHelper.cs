using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace CPMMS.Core;

/// <summary>
/// Connection string and query helpers - the single place the application
/// talks to MySQL from.
/// The connection string lives in appsettings.json next to the .exe.
/// </summary>
public static class DatabaseHelper
{
    private static string? _connectionString;

    static DatabaseHelper()
    {
        // maps column current_stock -> property CurrentStock, so queries can
        // select the real column names without aliasing every one of them
        DefaultTypeMap.MatchNamesWithUnderscores = true;
    }

    public static string ConnectionString => _connectionString ??= Load();

    private static string Load()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        return config.GetConnectionString("MySql")
            ?? throw new InvalidOperationException(
                "ConnectionStrings:MySql is missing from appsettings.json.");
    }

    public static MySqlConnection Open()
    {
        var connection = new MySqlConnection(ConnectionString);
        connection.Open();
        return connection;
    }


    // ---------------------------------------------------------------- helpers
    // Short reads and one-line writes go through these. Anything that needs a
    // transaction (issuance, receiving, returns, counts) opens its own
    // connection with Open() instead, because the whole unit of work has to
    // share one connection.

    public static IReadOnlyList<T> Query<T>(string sql, object? param = null)
    {
        using var cn = Open();
        return cn.Query<T>(sql, param).ToList();
    }

    public static T? QueryOne<T>(string sql, object? param = null)
    {
        using var cn = Open();
        return cn.QuerySingleOrDefault<T>(sql, param);
    }

    public static int Execute(string sql, object? param = null)
    {
        using var cn = Open();
        return cn.Execute(sql, param);
    }

    public static T? Scalar<T>(string sql, object? param = null)
    {
        using var cn = Open();
        return cn.ExecuteScalar<T>(sql, param);
    }

    /// <summary>Used by the login screen to give a clear message when MySQL is not running.</summary>
    public static (bool Ok, string Message) TestConnection()
    {
        try
        {
            using var cn = Open();
            var name = cn.ExecuteScalar<string>("SELECT DATABASE()");
            return (true, $"Connected to {name}");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
}
