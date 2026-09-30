using DigitalShield.API.Authentication;
using DigitalShield.API.Configuration;
using DigitalShield.API.Data;
using DigitalShield.API.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace DigitalShield.Tests.Integration;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class SqlServerIntegrationCollection : ICollectionFixture<SqlServerIntegrationFixture>
{
    public const string Name = "SQL Server integration";
}

public sealed class SqlServerIntegrationFixture : IAsyncLifetime
{
    private readonly DbContextOptions<ApplicationDbContext>? _options;

    public SqlServerIntegrationFixture()
    {
        if (SqlServerIntegrationEnvironment.IsAvailable)
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(SqlServerIntegrationEnvironment.ConnectionString)
                .Options;
        }
    }

    public async Task InitializeAsync()
    {
        if (!SqlServerIntegrationEnvironment.IsAvailable)
        {
            return;
        }

        await using var context = CreateContext();
        await context.Database.MigrateAsync();
        await CreateSeeder(context).SeedAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    public ApplicationDbContext CreateContext() => new(_options
        ?? throw new InvalidOperationException(SqlServerIntegrationEnvironment.BlockingReason));

    public DatabaseSeeder CreateSeeder(ApplicationDbContext context)
    {
        return new DatabaseSeeder(
            context,
            new IntegrationHostEnvironment(),
            new PasswordHasherService(),
            Options.Create(new DevelopmentUserSeedSettings { Enabled = false }),
            NullLogger<DatabaseSeeder>.Instance);
    }

    public AuthenticationService CreateAuthenticationService(ApplicationDbContext context)
    {
        return new AuthenticationService(
            new UserRepository(context),
            new PasswordHasherService(),
            new JwtTokenService(Options.Create(new JwtSettings
            {
                Issuer = "DigitalShield.Integration",
                Audience = "DigitalShield.Integration.Client",
                SecretKey = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(48)),
                ExpiryMinutes = 30
            })));
    }

    private sealed class IntegrationHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = Environments.Production;
        public string ApplicationName { get; set; } = "DigitalShield.Tests";
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
