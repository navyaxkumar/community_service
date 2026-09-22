using DigitalShield.API.Data;
using DigitalShield.API.Data.Seed;
using DigitalShield.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;

namespace DigitalShield.Tests;

public class DatabaseSeederTests
{
    [Fact]
    public async Task SeedAsync_EmptyDatabase_AddsFraudCategoriesLearningModulesAndFoundationBadge()
    {
        await using var context = CreateInMemoryContext();
        var seeder = CreateSeeder(context, Environments.Development);

        await seeder.SeedAsync();

        var categoryCount = await context.FraudCategories.CountAsync();
        var moduleCount = await context.LearningModules.CountAsync();
        var badge = await context.Badges.SingleAsync();
        Assert.Equal(FraudCategorySeed.CreateAll().Count, categoryCount);
        Assert.Equal(LearningModuleSeed.All.Count, moduleCount);
        Assert.Equal(BadgeSeed.AwarenessStarterName, badge.Name);
        Assert.True(badge.IsActive);
        Assert.Equal(0, badge.RequiredPoints);
    }

    [Fact]
    public async Task SeedAsync_SecondRun_DoesNotCreateDuplicates()
    {
        await using var context = CreateInMemoryContext();
        var seeder = CreateSeeder(context, Environments.Development);

        await seeder.SeedAsync();
        await seeder.SeedAsync();

        Assert.Equal(FraudCategorySeed.CreateAll().Count, await context.FraudCategories.CountAsync());
        Assert.Equal(LearningModuleSeed.All.Count, await context.LearningModules.CountAsync());
        Assert.Equal(1, await context.Badges.CountAsync(badge =>
            badge.Name == BadgeSeed.AwarenessStarterName));
    }

    [Fact]
    public async Task SeedAsync_ExistingData_PreservesUsersProgressAttemptsAndContent()
    {
        await using var context = CreateInMemoryContext();
        var user = new User
        {
            Name = "Existing User",
            Email = "existing.user@example.test",
            PasswordHash = "existing-password-hash",
            Role = "User"
        };
        var category = new FraudCategory
        {
            Name = "Existing Category",
            Description = "Existing content category."
        };
        var module = new LearningModule
        {
            Title = "Existing Module",
            Content = "Existing educational content.",
            FraudCategory = category
        };
        var quiz = new Quiz
        {
            Title = "Existing Quiz",
            IsPublished = true,
            FraudCategory = category
        };

        context.Users.Add(user);
        context.LearningModules.Add(module);
        context.Quizzes.Add(quiz);
        await context.SaveChangesAsync();

        context.UserProgress.Add(new UserProgress
        {
            UserId = user.Id,
            LearningModuleId = module.Id,
            ProgressPercentage = 40
        });
        context.QuizAttempts.Add(new QuizAttempt
        {
            UserId = user.Id,
            QuizId = quiz.Id,
            Score = 1,
            TotalQuestions = 2
        });
        await context.SaveChangesAsync();

        var seeder = CreateSeeder(context, Environments.Development);

        await seeder.SeedAsync();

        Assert.Equal(1, await context.Users.CountAsync());
        Assert.Equal(1, await context.UserProgress.CountAsync());
        Assert.Equal(1, await context.QuizAttempts.CountAsync());
        Assert.True(await context.FraudCategories.AnyAsync(c => c.Name == "Existing Category"));
        Assert.True(await context.LearningModules.AnyAsync(m => m.Title == "Existing Module"));
        Assert.True(await context.Quizzes.AnyAsync(q => q.Title == "Existing Quiz"));
    }

    [Fact]
    public async Task SeedAsync_ExistingSeedRecords_DoesNotOverwriteCategoryOrModuleContent()
    {
        await using var context = CreateInMemoryContext();
        var category = new FraudCategory
        {
            Name = FraudCategorySeed.PhishingOtpScamsName,
            Description = "Admin edited category description.",
            IsActive = false
        };
        context.FraudCategories.Add(category);
        context.LearningModules.Add(new LearningModule
        {
            Title = LearningModuleSeed.All[0].Title,
            Description = "Admin edited module description.",
            Content = "Admin edited learning content.",
            Order = 99,
            IsPublished = false,
            FraudCategory = category
        });
        await context.SaveChangesAsync();

        var seeder = CreateSeeder(context, Environments.Development);

        await seeder.SeedAsync();

        var existingCategory = await context.FraudCategories.SingleAsync(c => c.Name == FraudCategorySeed.PhishingOtpScamsName);
        var existingModule = await context.LearningModules.SingleAsync(m => m.Title == LearningModuleSeed.All[0].Title);
        Assert.Equal("Admin edited category description.", existingCategory.Description);
        Assert.False(existingCategory.IsActive);
        Assert.Equal("Admin edited module description.", existingModule.Description);
        Assert.Equal("Admin edited learning content.", existingModule.Content);
        Assert.Equal(99, existingModule.Order);
        Assert.False(existingModule.IsPublished);
    }

    [Fact]
    public async Task SeedAsync_LearningModulesReferenceExpectedCategories()
    {
        await using var context = CreateInMemoryContext();
        var seeder = CreateSeeder(context, Environments.Development);

        await seeder.SeedAsync();

        var modules = await context.LearningModules
            .Include(module => module.FraudCategory)
            .ToListAsync();

        Assert.All(LearningModuleSeed.All, moduleDefinition =>
        {
            var module = modules.Single(m => m.Title == moduleDefinition.Title);
            Assert.NotNull(module.FraudCategory);
            Assert.Equal(moduleDefinition.CategoryName, module.FraudCategory.Name);
            Assert.Equal(moduleDefinition.Order, module.Order);
            Assert.True(module.IsPublished);
        });
    }

    [Fact]
    public void SeedContent_DoesNotContainObviousSensitiveOrOperationalAttackContent()
    {
        var sensitiveTerms = new[]
        {
            "password:",
            "api key",
            "secret token",
            "private key",
            "real otp",
            "card number",
            "cvv",
            "exploit code",
            "bypass authentication",
            "credential harvesting"
        };

        var searchableContent = string.Join(
            "\n",
            LearningModuleSeed.All.SelectMany(module => new[] { module.Title, module.Description, module.Content }));

        foreach (var term in sensitiveTerms)
        {
            Assert.DoesNotContain(term, searchableContent, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public async Task SeedAsync_Production_DoesNotCreateDevelopmentUsers()
    {
        await using var context = CreateInMemoryContext();
        var seeder = CreateSeeder(context, Environments.Production);

        await seeder.SeedAsync();

        Assert.Empty(await context.Users.ToListAsync());
        Assert.Equal(FraudCategorySeed.CreateAll().Count, await context.FraudCategories.CountAsync());
        Assert.Equal(LearningModuleSeed.All.Count, await context.LearningModules.CountAsync());
        Assert.Single(await context.Badges.ToListAsync());
    }

    [Fact]
    public async Task SeedAsync_UnavailableDatabase_ThrowsClearFailure()
    {
        await using var context = CreateUnavailableSqlServerContext();
        var seeder = CreateSeeder(context, Environments.Development);

        await Assert.ThrowsAnyAsync<Exception>(() => seeder.SeedAsync());
    }

    private static DatabaseSeeder CreateSeeder(ApplicationDbContext context, string environmentName)
    {
        return new DatabaseSeeder(
            context,
            new TestHostEnvironment(environmentName),
            NullLogger<DatabaseSeeder>.Instance);
    }

    private static ApplicationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    private static ApplicationDbContext CreateUnavailableSqlServerContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer("Server=127.0.0.1,1;Database=DigitalShieldSeederUnavailable;Trusted_Connection=True;Connect Timeout=1;TrustServerCertificate=True;Encrypt=False;")
            .Options;

        return new ApplicationDbContext(options);
    }

    private sealed class TestHostEnvironment : IHostEnvironment
    {
        public TestHostEnvironment(string environmentName)
        {
            EnvironmentName = environmentName;
        }

        public string EnvironmentName { get; set; }
        public string ApplicationName { get; set; } = "DigitalShield.Tests";
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
