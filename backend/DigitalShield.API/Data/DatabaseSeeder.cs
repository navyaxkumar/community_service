using DigitalShield.API.Authentication;
using DigitalShield.API.Configuration;
using DigitalShield.API.Constants;
using DigitalShield.API.Data.Seed;
using DigitalShield.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DigitalShield.API.Data;

public class DatabaseSeeder : IDatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly DevelopmentUserSeedSettings _developmentUserSeedSettings;
    private readonly IHostEnvironment _environment;
    private readonly ILogger<DatabaseSeeder> _logger;
    private readonly IPasswordHasherService _passwordHasher;

    public DatabaseSeeder(
        ApplicationDbContext context,
        IHostEnvironment environment,
        IPasswordHasherService passwordHasher,
        IOptions<DevelopmentUserSeedSettings> developmentUserSeedSettings,
        ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _environment = environment;
        _passwordHasher = passwordHasher;
        _developmentUserSeedSettings = developmentUserSeedSettings.Value;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var developmentUsers = GetDevelopmentUsersToSeed();
            var recordsAdded = 0;

            var categoriesAdded = await SeedFraudCategoriesAsync(cancellationToken);
            recordsAdded += categoriesAdded;
            if (categoriesAdded > 0)
            {
                await _context.SaveChangesAsync(cancellationToken);
            }

            recordsAdded += await SeedLearningModulesAsync(cancellationToken);
            recordsAdded += await SeedScenariosAsync(cancellationToken);
            recordsAdded += await SeedQuizzesAsync(cancellationToken);
            recordsAdded += await SeedBadgesAsync(cancellationToken);

            if (developmentUsers.Count > 0)
            {
                recordsAdded += await SeedDevelopmentUsersAsync(developmentUsers, cancellationToken);
            }

            if (recordsAdded > 0)
            {
                await _context.SaveChangesAsync(cancellationToken);
            }

            _logger.LogInformation("Database seed completed. Added {RecordCount} records.", recordsAdded);
        }
        catch (Exception exception)
        {
            _logger.LogError("Database seed failed with {ExceptionType}.", exception.GetType().Name);
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

    private async Task<int> SeedScenariosAsync(CancellationToken cancellationToken)
    {
        var categories = await _context.FraudCategories
            .ToDictionaryAsync(category => category.Name, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var existingScenarioTitles = await _context.Scenarios
            .Select(scenario => scenario.Title)
            .ToListAsync(cancellationToken);
        var existingScenarioTitleSet = existingScenarioTitles.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var recordsAdded = 0;

        foreach (var scenarioDefinition in ScenarioSeed.All)
        {
            if (existingScenarioTitleSet.Contains(scenarioDefinition.Title))
            {
                continue;
            }

            if (!categories.TryGetValue(scenarioDefinition.CategoryName, out var category))
            {
                throw new InvalidOperationException("Required seed category is missing.");
            }

            _context.Scenarios.Add(scenarioDefinition.Create(category));
            existingScenarioTitleSet.Add(scenarioDefinition.Title);
            recordsAdded++;
        }

        return recordsAdded;
    }

    private async Task<int> SeedQuizzesAsync(CancellationToken cancellationToken)
    {
        var categories = await _context.FraudCategories
            .ToDictionaryAsync(category => category.Name, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var existingQuizTitles = await _context.Quizzes
            .Select(quiz => quiz.Title)
            .ToListAsync(cancellationToken);
        var existingQuizTitleSet = existingQuizTitles.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var recordsAdded = 0;

        foreach (var quizDefinition in QuizSeed.All)
        {
            if (existingQuizTitleSet.Contains(quizDefinition.Title))
            {
                continue;
            }

            if (!categories.TryGetValue(quizDefinition.CategoryName, out var category))
            {
                throw new InvalidOperationException("Required seed category is missing.");
            }

            _context.Quizzes.Add(quizDefinition.Create(category));
            existingQuizTitleSet.Add(quizDefinition.Title);
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

    private async Task<int> SeedDevelopmentUsersAsync(
        IReadOnlyCollection<DevelopmentSeedUser> developmentUsers,
        CancellationToken cancellationToken)
    {
        var existingEmails = await _context.Users
            .Select(user => user.Email)
            .ToListAsync(cancellationToken);
        var existingEmailSet = existingEmails
            .Where(email => !string.IsNullOrWhiteSpace(email))
            .Select(EmailNormalizer.Normalize)
            .ToHashSet(StringComparer.Ordinal);
        var recordsAdded = 0;

        foreach (var developmentUser in developmentUsers)
        {
            if (existingEmailSet.Contains(developmentUser.Email))
            {
                continue;
            }

            _context.Users.Add(new User
            {
                Name = developmentUser.Name,
                Email = developmentUser.Email,
                PasswordHash = _passwordHasher.HashPassword(developmentUser.Password),
                Role = developmentUser.Role,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            });
            existingEmailSet.Add(developmentUser.Email);
            recordsAdded++;
        }

        return recordsAdded;
    }

    private IReadOnlyCollection<DevelopmentSeedUser> GetDevelopmentUsersToSeed()
    {
        if (!_environment.IsDevelopment() || !_developmentUserSeedSettings.Enabled)
        {
            return [];
        }

        var adminEmail = EmailNormalizer.Normalize(GetRequiredCredentialValue(_developmentUserSeedSettings.AdminEmail));
        var userEmail = EmailNormalizer.Normalize(GetRequiredCredentialValue(_developmentUserSeedSettings.UserEmail));
        if (string.Equals(adminEmail, userEmail, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Development user seed account emails must be distinct.");
        }

        return
        [
            new DevelopmentSeedUser(
                "Development Administrator",
                adminEmail,
                GetRequiredCredentialValue(_developmentUserSeedSettings.AdminPassword),
                Roles.Admin),
            new DevelopmentSeedUser(
                "Development User",
                userEmail,
                GetRequiredCredentialValue(_developmentUserSeedSettings.UserPassword),
                Roles.User)
        ];
    }

    private static string GetRequiredCredentialValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException(
                "Development user seeding is enabled but required credentials are missing. Configure them through User Secrets or environment variables.");
        }

        return value;
    }

    private sealed record DevelopmentSeedUser(string Name, string Email, string Password, string Role);
}
