using DigitalShield.API.Data.Seed;
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

            var categoriesAdded = await SeedFraudCategoriesAsync(cancellationToken);
            recordsAdded += categoriesAdded;
            if (categoriesAdded > 0)
            {
                await _context.SaveChangesAsync(cancellationToken);
            }

            recordsAdded += await SeedLearningModulesAsync(cancellationToken);
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
        var existingCategoryNames = await _context.FraudCategories
            .Select(category => category.Name)
            .ToListAsync(cancellationToken);
        var existingCategoryNameSet = existingCategoryNames.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var recordsAdded = 0;

        foreach (var category in FraudCategorySeed.CreateAll())
        {
            if (existingCategoryNameSet.Contains(category.Name))
            {
                continue;
            }

            _context.FraudCategories.Add(category);
            existingCategoryNameSet.Add(category.Name);
            recordsAdded++;
        }

        return recordsAdded;
    }

    private async Task<int> SeedLearningModulesAsync(CancellationToken cancellationToken)
    {
        var categories = await _context.FraudCategories
            .ToDictionaryAsync(category => category.Name, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var existingModuleTitles = await _context.LearningModules
            .Select(module => module.Title)
            .ToListAsync(cancellationToken);
        var existingModuleTitleSet = existingModuleTitles.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var recordsAdded = 0;

        foreach (var moduleDefinition in LearningModuleSeed.All)
        {
            if (existingModuleTitleSet.Contains(moduleDefinition.Title))
            {
                continue;
            }

            if (!categories.TryGetValue(moduleDefinition.CategoryName, out var category))
            {
                throw new InvalidOperationException("Required seed category is missing.");
            }

            _context.LearningModules.Add(moduleDefinition.Create(category));
            existingModuleTitleSet.Add(moduleDefinition.Title);
            recordsAdded++;
        }

        return recordsAdded;
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
