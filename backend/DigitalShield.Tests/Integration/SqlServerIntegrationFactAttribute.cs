using Microsoft.Data.SqlClient;

namespace DigitalShield.Tests.Integration;

[AttributeUsage(AttributeTargets.Method)]
public sealed class SqlServerIntegrationFactAttribute : FactAttribute
{
    public SqlServerIntegrationFactAttribute()
    {
        if (!SqlServerIntegrationEnvironment.IsAvailable)
        {
            Skip = SqlServerIntegrationEnvironment.BlockingReason;
        }
    }
}

internal static class SqlServerIntegrationEnvironment
{
    public const string ConnectionStringEnvironmentVariable = "ConnectionStrings__IntegrationTest";
    private const string DedicatedDatabasePrefix = "DigitalShield_Integration";
    private static readonly Lazy<IntegrationAvailability> Availability = new(CheckAvailability);

    public static bool IsAvailable => Availability.Value.IsAvailable;
    public static string BlockingReason => Availability.Value.BlockingReason;
    public static string ConnectionString => Availability.Value.ConnectionString
        ?? throw new InvalidOperationException(Availability.Value.BlockingReason);

    private static IntegrationAvailability CheckAvailability()
    {
        var connectionString = Environment.GetEnvironmentVariable(ConnectionStringEnvironmentVariable);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return IntegrationAvailability.Blocked(
                $"SQL Server integration is blocked: set the {ConnectionStringEnvironmentVariable} environment variable to a dedicated test database connection.");
        }

        SqlConnectionStringBuilder builder;
        try
        {
            builder = new SqlConnectionStringBuilder(connectionString);
        }
        catch (ArgumentException)
        {
            return IntegrationAvailability.Blocked("SQL Server integration is blocked: the dedicated test database connection string is invalid.");
        }

        if (string.IsNullOrWhiteSpace(builder.InitialCatalog) ||
            !builder.InitialCatalog.StartsWith(DedicatedDatabasePrefix, StringComparison.OrdinalIgnoreCase))
        {
            return IntegrationAvailability.Blocked(
                $"SQL Server integration is blocked: the configured database name must start with {DedicatedDatabasePrefix}.");
        }

        try
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return IntegrationAvailability.Available(connectionString);
        }
        catch (SqlException exception)
        {
            return IntegrationAvailability.Blocked(
                $"SQL Server integration is blocked: the dedicated test database could not be opened (SQL error {exception.Number}).");
        }
        catch (InvalidOperationException)
        {
            return IntegrationAvailability.Blocked("SQL Server integration is blocked: the dedicated test database connection could not be opened.");
        }
    }

    private sealed record IntegrationAvailability(bool IsAvailable, string BlockingReason, string? ConnectionString)
    {
        public static IntegrationAvailability Available(string connectionString) => new(true, string.Empty, connectionString);

        public static IntegrationAvailability Blocked(string reason) => new(false, reason, null);
    }
}
