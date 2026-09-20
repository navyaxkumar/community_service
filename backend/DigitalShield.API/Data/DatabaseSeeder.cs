using DigitalShield.API.Data.Seed;
using DigitalShield.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DigitalShield.API.Data;

public class DatabaseSeeder : IDatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly IHostEnvironment _environment;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        ApplicationDbContext context,
        IHostEnvironment environment,
        ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _environment = environment;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var recordsAdded = 0;

            recordsAdded += await SeedFraudCategoriesAsync(cancellationToken);
            recordsAdded += await SeedBadgesAsync(cancellationToken);

            if (_environment.IsDevelopment())
            {
                recordsAdded += await SeedDevelopmentDataAsync(cancellationToken);
            }

            if (recordsAdded > 0)
            {
                await _context.SaveChangesAsync(cancellationToken);
            }

            _logger.LogInformation("Database seed completed. Added {RecordCount} records.", recordsAdded);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Database seed failed.");
            throw;
        }
    }

    private async Task<int> SeedFraudCategoriesAsync(CancellationToken cancellationToken)
    {
        if (await _context.FraudCategories.AnyAsync(
            category => category.Name == FraudCategorySeed.DigitalSafetyBasicsName,
            cancellationToken))
        {
            return 0;
        }

        _context.FraudCategories.Add(FraudCategorySeed.CreateDigitalSafetyBasics());
        return 1;
    }

    private async Task<int> SeedBadgesAsync(CancellationToken cancellationToken)
    {
        if (await _context.Badges.AnyAsync(
            badge => badge.Name == BadgeSeed.AwarenessStarterName,
            cancellationToken))
        {
            return 0;
        }

        _context.Badges.Add(BadgeSeed.CreateAwarenessStarter());
        return 1;
    }

    private static Task<int> SeedDevelopmentDataAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(0);
    }

}
