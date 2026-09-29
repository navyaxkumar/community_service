using System.Security.Cryptography;
using DigitalShield.API.Authentication;
using DigitalShield.API.Configuration;
using DigitalShield.API.Constants;
using DigitalShield.API.Data;
using DigitalShield.API.Data.Seed;
using DigitalShield.API.DTOs.User;
using DigitalShield.API.Models;
using DigitalShield.API.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace DigitalShield.Tests;

public class DatabaseSeederTests
{
    [Fact]
    public async Task SeedAsync_EmptyDatabase_AddsReferenceContentAndFoundationBadge()
    {
        await using var context = CreateInMemoryContext();
        var seeder = CreateSeeder(context, Environments.Development);

        await seeder.SeedAsync();

        var categoryCount = await context.FraudCategories.CountAsync();
        var moduleCount = await context.LearningModules.CountAsync();
        var scenarioCount = await context.Scenarios.CountAsync();
        var quizCount = await context.Quizzes.CountAsync();
        var questionCount = await context.QuizQuestions.CountAsync();
        var optionCount = await context.QuizOptions.CountAsync();
        var badge = await context.Badges.SingleAsync();
        Assert.Equal(FraudCategorySeed.CreateAll().Count, categoryCount);
        Assert.Equal(LearningModuleSeed.All.Count, moduleCount);
        Assert.Equal(ScenarioSeed.All.Count, scenarioCount);
        Assert.Equal(QuizSeed.All.Count, quizCount);
        Assert.Equal(QuizSeed.All.Sum(quiz => quiz.Questions.Count), questionCount);
        Assert.Equal(QuizSeed.All.Sum(quiz => quiz.Questions.Sum(question => question.Options.Count)), optionCount);
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
        Assert.Equal(ScenarioSeed.All.Count, await context.Scenarios.CountAsync());
        Assert.Equal(QuizSeed.All.Count, await context.Quizzes.CountAsync());
        Assert.Equal(QuizSeed.All.Sum(quiz => quiz.Questions.Count), await context.QuizQuestions.CountAsync());
        Assert.Equal(QuizSeed.All.Sum(quiz => quiz.Questions.Sum(question => question.Options.Count)), await context.QuizOptions.CountAsync());
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
    public async Task SeedAsync_ExistingScenarioAndQuiz_DoesNotOverwriteSeededContent()
    {
        await using var context = CreateInMemoryContext();
        var category = new FraudCategory
        {
            Name = FraudCategorySeed.PhishingOtpScamsName,
            Description = "Existing category."
        };
        context.FraudCategories.Add(category);
        context.Scenarios.Add(new Scenario
        {
            Title = ScenarioSeed.All[0].Title,
            Description = "Admin edited scenario description.",
            Situation = "Admin edited situation.",
            CorrectAction = "Admin edited action.",
            IsPublished = false,
            FraudCategory = category
        });
        context.Quizzes.Add(new Quiz
        {
            Title = QuizSeed.All[0].Title,
            Description = "Admin edited quiz description.",
            IsPublished = false,
            FraudCategory = category,
            Questions =
            [
                new QuizQuestion
                {
                    QuestionText = "Admin edited question?",
                    Explanation = "Admin edited explanation.",
                    Order = 99,
                    Options =
                    [
                        new QuizOption { OptionText = "Admin edited option", IsCorrect = true, Order = 99 }
                    ]
                }
            ]
        });
        await context.SaveChangesAsync();

        var seeder = CreateSeeder(context, Environments.Development);

        await seeder.SeedAsync();

        var existingScenario = await context.Scenarios.SingleAsync(s => s.Title == ScenarioSeed.All[0].Title);
        var existingQuiz = await context.Quizzes
            .Include(quiz => quiz.Questions)
            .ThenInclude(question => question.Options)
            .SingleAsync(quiz => quiz.Title == QuizSeed.All[0].Title);
        Assert.Equal("Admin edited scenario description.", existingScenario.Description);
        Assert.Equal("Admin edited situation.", existingScenario.Situation);
        Assert.Equal("Admin edited action.", existingScenario.CorrectAction);
        Assert.False(existingScenario.IsPublished);
        Assert.Equal("Admin edited quiz description.", existingQuiz.Description);
        Assert.False(existingQuiz.IsPublished);
        Assert.Single(existingQuiz.Questions);
        Assert.Equal("Admin edited question?", existingQuiz.Questions.Single().QuestionText);
        Assert.Equal(99, existingQuiz.Questions.Single().Order);
        Assert.Single(existingQuiz.Questions.Single().Options);
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
    public async Task SeedAsync_ScenariosAndQuizzesReferenceExpectedCategories()
    {
        await using var context = CreateInMemoryContext();
        var seeder = CreateSeeder(context, Environments.Development);

        await seeder.SeedAsync();

        var scenarios = await context.Scenarios
            .Include(scenario => scenario.FraudCategory)
            .ToListAsync();
        var quizzes = await context.Quizzes
            .Include(quiz => quiz.FraudCategory)
            .Include(quiz => quiz.Questions)
            .ThenInclude(question => question.Options)
            .ToListAsync();

        Assert.All(ScenarioSeed.All, scenarioDefinition =>
        {
            var scenario = scenarios.Single(candidate => candidate.Title == scenarioDefinition.Title);
            Assert.NotNull(scenario.FraudCategory);
            Assert.Equal(scenarioDefinition.CategoryName, scenario.FraudCategory.Name);
            Assert.True(scenario.IsPublished);
        });

        Assert.All(QuizSeed.All, quizDefinition =>
        {
            var quiz = quizzes.Single(candidate => candidate.Title == quizDefinition.Title);
            Assert.NotNull(quiz.FraudCategory);
            Assert.Equal(quizDefinition.CategoryName, quiz.FraudCategory.Name);
            Assert.True(quiz.IsPublished);
            Assert.Equal(quizDefinition.Questions.Count, quiz.Questions.Count);
        });
    }

    [Fact]
    public async Task SeedAsync_SeededQuizQuestionsHaveDeterministicOrderingAndOneCorrectOption()
    {
        await using var context = CreateInMemoryContext();
        var seeder = CreateSeeder(context, Environments.Development);

        await seeder.SeedAsync();

        var questions = await context.QuizQuestions
            .Include(question => question.Options)
            .ToListAsync();

        Assert.All(questions, question =>
        {
            Assert.InRange(question.Order, 1, 5);
            Assert.Equal(4, question.Options.Count);
            Assert.Equal(1, question.Options.Count(option => option.IsCorrect));
            Assert.Equal([1, 2, 3, 4], question.Options.OrderBy(option => option.Order).Select(option => option.Order).ToArray());
            Assert.False(string.IsNullOrWhiteSpace(question.Explanation));
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
            LearningModuleSeed.All.SelectMany(module => new[] { module.Title, module.Description, module.Content })
                .Concat(ScenarioSeed.All.SelectMany(scenario => new[] { scenario.Title, scenario.Description, scenario.Situation, scenario.CorrectAction }))
                .Concat(QuizSeed.All.SelectMany(quiz => new[] { quiz.Title, quiz.Description }
                    .Concat(quiz.Questions.SelectMany(question => new[] { question.QuestionText, question.Explanation }
                        .Concat(question.Options.Select(option => option.OptionText)))))));

        foreach (var term in sensitiveTerms)
        {
            Assert.DoesNotContain(term, searchableContent, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public async Task SeedAsync_Production_DoesNotCreateDevelopmentUsers()
    {
        await using var context = CreateInMemoryContext();
        var seeder = CreateSeeder(
            context,
            Environments.Production,
            CreateDevelopmentUserSeedSettings());

        await seeder.SeedAsync();

        Assert.Empty(await context.Users.ToListAsync());
        Assert.Equal(FraudCategorySeed.CreateAll().Count, await context.FraudCategories.CountAsync());
        Assert.Equal(LearningModuleSeed.All.Count, await context.LearningModules.CountAsync());
        Assert.Equal(ScenarioSeed.All.Count, await context.Scenarios.CountAsync());
        Assert.Equal(QuizSeed.All.Count, await context.Quizzes.CountAsync());
        Assert.Single(await context.Badges.ToListAsync());
    }

    [Fact]
    public async Task SeedAsync_DevelopmentUserSeedingDisabled_DoesNotCreateUsers()
    {
        await using var context = CreateInMemoryContext();
        var settings = CreateDevelopmentUserSeedSettings();
        settings.Enabled = false;
        var seeder = CreateSeeder(context, Environments.Development, settings);

        await seeder.SeedAsync();

        Assert.Empty(await context.Users.ToListAsync());
    }

    [Fact]
    public async Task SeedAsync_DevelopmentUserSeedingEnabled_CreatesHashedAdminAndUser()
    {
        await using var context = CreateInMemoryContext();
        var settings = CreateDevelopmentUserSeedSettings();
        var logger = new CapturingLogger<DatabaseSeeder>();
        var seeder = CreateSeeder(context, Environments.Development, settings, logger);

        await seeder.SeedAsync();

        var users = await context.Users.OrderBy(user => user.Role).ToListAsync();
        var admin = users.Single(user => user.Role == Roles.Admin);
        var regularUser = users.Single(user => user.Role == Roles.User);
        var passwordHasher = new PasswordHasherService();

        Assert.Equal(2, users.Count);
        Assert.Equal(EmailNormalizer.Normalize(settings.AdminEmail!), admin.Email);
        Assert.Equal(EmailNormalizer.Normalize(settings.UserEmail!), regularUser.Email);
        Assert.True(admin.IsActive);
        Assert.True(regularUser.IsActive);
        Assert.True(passwordHasher.VerifyPassword(admin.PasswordHash, settings.AdminPassword!));
        Assert.True(passwordHasher.VerifyPassword(regularUser.PasswordHash, settings.UserPassword!));
        Assert.NotEqual(settings.AdminPassword, admin.PasswordHash);
        Assert.NotEqual(settings.UserPassword, regularUser.PasswordHash);
        Assert.Null(typeof(User).GetProperty("Password"));
        Assert.DoesNotContain(logger.Messages, message => message.Contains(settings.AdminPassword!, StringComparison.Ordinal));
        Assert.DoesNotContain(logger.Messages, message => message.Contains(settings.UserPassword!, StringComparison.Ordinal));
        Assert.DoesNotContain(logger.Messages, message => message.Contains(admin.PasswordHash, StringComparison.Ordinal));
        Assert.DoesNotContain(logger.Messages, message => message.Contains(regularUser.PasswordHash, StringComparison.Ordinal));
    }

    [Fact]
    public async Task SeedAsync_DevelopmentUserSeedingEnabledWithoutCredentials_FailsBeforeWritingRecords()
    {
        await using var context = CreateInMemoryContext();
        var settings = CreateDevelopmentUserSeedSettings();
        settings.AdminPassword = null;
        var seeder = CreateSeeder(context, Environments.Development, settings);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => seeder.SeedAsync());

        Assert.Contains("required credentials are missing", exception.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Empty(await context.Users.ToListAsync());
        Assert.Empty(await context.FraudCategories.ToListAsync());
    }

    [Fact]
    public async Task SeedAsync_DevelopmentUsers_AreIdempotentAndAuthenticateThroughNormalService()
    {
        await using var context = CreateInMemoryContext();
        var settings = CreateDevelopmentUserSeedSettings();
        var seeder = CreateSeeder(context, Environments.Development, settings);

        await seeder.SeedAsync();
        await seeder.SeedAsync();

        var authenticationService = CreateAuthenticationService(context);
        var login = await authenticationService.LoginAsync(new LoginRequestDto
        {
            Email = settings.AdminEmail!,
            Password = settings.AdminPassword!
        });

        Assert.Equal(2, await context.Users.CountAsync());
        Assert.Equal(Roles.Admin, login.User.Role);
        Assert.Equal(EmailNormalizer.Normalize(settings.AdminEmail!), login.User.Email);
    }

    [Fact]
    public async Task SeedAsync_ExistingConfiguredUser_IsPreservedAndNotElevated()
    {
        await using var context = CreateInMemoryContext();
        var settings = CreateDevelopmentUserSeedSettings();
        var passwordHasher = new PasswordHasherService();
        var originalPasswordHash = passwordHasher.HashPassword(CreateTestSecret());
        var originalUpdatedAt = DateTime.UtcNow.AddDays(-1);
        var existingUser = new User
        {
            Name = "Existing local account",
            Email = EmailNormalizer.Normalize(settings.AdminEmail!),
            PasswordHash = originalPasswordHash,
            Role = Roles.User,
            IsActive = false,
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            UpdatedAt = originalUpdatedAt
        };
        context.Users.Add(existingUser);
        await context.SaveChangesAsync();

        var seeder = CreateSeeder(context, Environments.Development, settings);
        await seeder.SeedAsync();

        var preservedUser = await context.Users.SingleAsync(user => user.Id == existingUser.Id);
        Assert.Equal(2, await context.Users.CountAsync());
        Assert.Equal("Existing local account", preservedUser.Name);
        Assert.Equal(originalPasswordHash, preservedUser.PasswordHash);
        Assert.Equal(Roles.User, preservedUser.Role);
        Assert.False(preservedUser.IsActive);
        Assert.Equal(originalUpdatedAt, preservedUser.UpdatedAt);
    }

    [Fact]
    public async Task SeedAsync_UnavailableDatabase_ThrowsClearFailure()
    {
        await using var context = CreateUnavailableSqlServerContext();
        var seeder = CreateSeeder(context, Environments.Development);

        await Assert.ThrowsAnyAsync<Exception>(() => seeder.SeedAsync());
    }

    [Fact]
    public async Task SeedAsync_Failure_LogsOnlyTheExceptionType()
    {
        await using var context = CreateInMemoryContext();
        var settings = CreateDevelopmentUserSeedSettings();
        settings.AdminPassword = null;
        var logger = new CapturingLogger<DatabaseSeeder>();
        var seeder = CreateSeeder(context, Environments.Development, settings, logger);

        await Assert.ThrowsAsync<InvalidOperationException>(() => seeder.SeedAsync());

        Assert.Equal("Database seed failed with InvalidOperationException.", Assert.Single(logger.Messages));
        Assert.DoesNotContain(logger.Exceptions, exception => exception is not null);
    }

    private static DatabaseSeeder CreateSeeder(
        ApplicationDbContext context,
        string environmentName,
        DevelopmentUserSeedSettings? developmentUserSeedSettings = null,
        ILogger<DatabaseSeeder>? logger = null)
    {
        return new DatabaseSeeder(
            context,
            new TestHostEnvironment(environmentName),
            new PasswordHasherService(),
            Options.Create(developmentUserSeedSettings ?? new DevelopmentUserSeedSettings()),
            logger ?? NullLogger<DatabaseSeeder>.Instance);
    }

    private static DevelopmentUserSeedSettings CreateDevelopmentUserSeedSettings()
    {
        return new DevelopmentUserSeedSettings
        {
            Enabled = true,
            AdminEmail = "  development-admin@seed.test  ",
            AdminPassword = CreateTestSecret(),
            UserEmail = "  development-user@seed.test  ",
            UserPassword = CreateTestSecret()
        };
    }

    private static AuthenticationService CreateAuthenticationService(ApplicationDbContext context)
    {
        return new AuthenticationService(
            new UserRepository(context),
            new PasswordHasherService(),
            new JwtTokenService(Options.Create(new JwtSettings
            {
                Issuer = "DigitalShield.Tests",
                Audience = "DigitalShield.Tests",
                SecretKey = "test-only-signing-key-with-at-least-thirty-two-bytes",
                ExpiryMinutes = 60
            })));
    }

    private static string CreateTestSecret()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
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

    private sealed class CapturingLogger<T> : ILogger<T>
    {
        public List<Exception?> Exceptions { get; } = [];
        public List<string> Messages { get; } = [];

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            Exceptions.Add(exception);
            Messages.Add(formatter(state, exception));
        }
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
