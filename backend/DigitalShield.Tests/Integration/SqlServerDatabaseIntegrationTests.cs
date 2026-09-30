using System.Data;
using DigitalShield.API.Authentication;
using DigitalShield.API.Authorization;
using DigitalShield.API.Constants;
using DigitalShield.API.Data;
using DigitalShield.API.DTOs.Progress;
using DigitalShield.API.DTOs.Quiz;
using DigitalShield.API.DTOs.User;
using DigitalShield.API.Models;
using DigitalShield.API.Repositories;
using DigitalShield.API.Services;
using Microsoft.EntityFrameworkCore;

namespace DigitalShield.Tests.Integration;

[Collection(SqlServerIntegrationCollection.Name)]
[Trait("Category", "Integration")]
public sealed class SqlServerDatabaseIntegrationTests
{
    private static readonly string[] ExpectedMigrations =
    [
        "20260920130618_InitialCreate",
        "20260920131521_AddInitialDomainModels",
        "20260920150244_RefineDatabaseSchema",
        "20260928181716_AddDatabasePerformanceIndexes"
    ];

    private static readonly string[] ExpectedTables =
    [
        "Users",
        "FraudCategories",
        "LearningModules",
        "Scenarios",
        "Quizzes",
        "QuizQuestions",
        "QuizOptions",
        "UserProgress",
        "QuizAttempts",
        "Badges"
    ];

    private readonly SqlServerIntegrationFixture _fixture;

    public SqlServerDatabaseIntegrationTests(SqlServerIntegrationFixture fixture)
    {
        _fixture = fixture;
    }

    [SqlServerIntegrationFact]
    public async Task Migrations_CreateExpectedSqlServerSchemaAndHistory()
    {
        await using var context = _fixture.CreateContext();

        Assert.Equal("Microsoft.EntityFrameworkCore.SqlServer", context.Database.ProviderName);
        Assert.True(await context.Database.CanConnectAsync());
        Assert.Equal(ExpectedMigrations, (await context.Database.GetAppliedMigrationsAsync()).ToArray());
        Assert.Empty(await context.Database.GetPendingMigrationsAsync());

        await context.Database.OpenConnectionAsync();
        HashSet<string> tableNames;
        try
        {
            var schema = context.Database.GetDbConnection().GetSchema("Tables");
            tableNames = schema.Rows.Cast<DataRow>()
                .Select(row => row.Field<string>("TABLE_NAME") ?? string.Empty)
                .Where(name => name.Length > 0)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }
        finally
        {
            await context.Database.CloseConnectionAsync();
        }

        Assert.Contains("__EFMigrationsHistory", tableNames);
        Assert.All(ExpectedTables, tableName => Assert.Contains(tableName, tableNames));
    }

    [SqlServerIntegrationFact]
    public async Task Seeder_IsIdempotentPreservesExistingDataAndDoesNotCreateDevelopmentUsers()
    {
        await using var context = _fixture.CreateContext();
        await using var transaction = await context.Database.BeginTransactionAsync();
        var before = await CountReferenceDataAsync(context);
        var category = await context.FraudCategories.OrderBy(item => item.Id).FirstAsync();
        category.Description = "Integration-test preservation marker";
        await context.SaveChangesAsync();

        await _fixture.CreateSeeder(context).SeedAsync();

        Assert.Equal(before, await CountReferenceDataAsync(context));
        Assert.Equal("Integration-test preservation marker", await context.FraudCategories
            .Where(item => item.Id == category.Id)
            .Select(item => item.Description)
            .SingleAsync());
        Assert.Empty(await context.Users.ToListAsync());
    }

    [SqlServerIntegrationFact]
    public async Task Authentication_PersistsNormalizedHashedUsersAndRejectsDuplicateOrInactiveLogin()
    {
        await using var context = _fixture.CreateContext();
        await using var transaction = await context.Database.BeginTransactionAsync();
        var service = _fixture.CreateAuthenticationService(context);
        var email = $"integration-{Guid.NewGuid():N}@example.test";
        var password = CreatePassword();

        var registered = await service.RegisterAsync(new RegisterRequestDto
        {
            Name = "Integration User",
            Email = $" {email.ToUpperInvariant()} ",
            Password = password
        });

        var storedUser = await context.Users.SingleAsync(user => user.Id == registered.Id);
        Assert.Equal(email, storedUser.Email);
        Assert.NotEqual(password, storedUser.PasswordHash);
        Assert.True(new PasswordHasherService().VerifyPassword(storedUser.PasswordHash, password));
        Assert.Null(registered.GetType().GetProperty("PasswordHash"));

        var login = await service.LoginAsync(new LoginRequestDto { Email = email, Password = password });
        Assert.False(string.IsNullOrWhiteSpace(login.AccessToken));
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.RegisterAsync(new RegisterRequestDto
        {
            Name = "Duplicate User",
            Email = email.ToUpperInvariant(),
            Password = CreatePassword()
        }));

        storedUser.IsActive = false;
        await context.SaveChangesAsync();
        await Assert.ThrowsAsync<AuthenticationException>(() => service.LoginAsync(new LoginRequestDto { Email = email, Password = password }));
    }

    [SqlServerIntegrationFact]
    public async Task PublishedContentAndQuizDetails_UseRelationalDataWithoutLeakingAnswers()
    {
        await using var context = _fixture.CreateContext();
        await using var transaction = await context.Database.BeginTransactionAsync();
        var category = await context.FraudCategories.OrderBy(item => item.Name).FirstAsync();
        context.LearningModules.Add(new LearningModule
        {
            Title = $"Draft module {Guid.NewGuid():N}",
            Content = "Draft content",
            IsPublished = false,
            Order = 999,
            FraudCategoryId = category.Id
        });
        context.Scenarios.Add(new Scenario
        {
            Title = $"Draft scenario {Guid.NewGuid():N}",
            Situation = "Draft situation",
            CorrectAction = "Draft action",
            IsPublished = false,
            FraudCategoryId = category.Id
        });
        await context.SaveChangesAsync();

        var moduleService = new LearningModuleService(new FraudCategoryRepository(context), new LearningModuleRepository(context));
        var scenarioService = new ScenarioService(new FraudCategoryRepository(context), new ScenarioRepository(context));
        var quizService = new QuizService(new FraudCategoryRepository(context), new QuizRepository(context));
        var modules = await moduleService.GetByCategoryAsync(category.Id);
        var scenarios = await scenarioService.GetByCategoryAsync(category.Id);
        var quiz = await context.Quizzes.Where(item => item.IsPublished).OrderBy(item => item.Id).FirstAsync();
        var detail = await quizService.GetByIdAsync(quiz.Id);

        Assert.NotEmpty(modules);
        Assert.All(modules, module => Assert.True(module.IsPublished));
        Assert.All(scenarios, scenario => Assert.True(scenario.IsPublished));
        Assert.NotNull(detail);
        Assert.NotEmpty(detail.Questions);
        Assert.NotEmpty(detail.Questions.SelectMany(question => question.Options));
        Assert.Null(typeof(QuizOptionForUserDto).GetProperty("IsCorrect"));
        Assert.True(await context.QuizOptions.AnyAsync(option => option.IsCorrect));
    }

    [SqlServerIntegrationFact]
    public async Task QuizAttempts_UseStoredAnswersCalculateScoresAndEnforceOwnership()
    {
        await using var context = _fixture.CreateContext();
        await using var transaction = await context.Database.BeginTransactionAsync();
        var user = await CreateUserAsync(context);
        var quiz = await context.Quizzes
            .Include(item => item.Questions)
            .ThenInclude(question => question.Options)
            .Where(item => item.IsPublished)
            .OrderBy(item => item.Id)
            .FirstAsync();
        var question = quiz.Questions.OrderBy(item => item.Id).First();
        var correctOption = question.Options.Single(option => option.IsCorrect);
        var service = new QuizAttemptService(
            new QuizAttemptRepository(context),
            new QuizRepository(context),
            new TestCurrentUserService(user.Id, Roles.User));

        var submitted = await service.SubmitAttemptAsync(user.Id, new SubmitQuizAttemptDto
        {
            QuizId = quiz.Id,
            Answers = [new QuizAnswerDto { QuestionId = question.Id, OptionId = correctOption.Id }]
        });

        var storedAttempt = await context.QuizAttempts.SingleAsync(item => item.Id == submitted.Id);
        Assert.Equal(1, submitted.Score);
        Assert.Equal(quiz.Questions.Count, submitted.TotalQuestions);
        Assert.Equal(user.Id, storedAttempt.UserId);
        Assert.NotNull(storedAttempt.CompletedAt);
        await Assert.ThrowsAsync<ArgumentException>(() => service.SubmitAttemptAsync(user.Id, new SubmitQuizAttemptDto
        {
            QuizId = quiz.Id,
            Answers = [new QuizAnswerDto { QuestionId = int.MaxValue, OptionId = correctOption.Id }]
        }));
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => new QuizAttemptService(
            new QuizAttemptRepository(context),
            new QuizRepository(context),
            new TestCurrentUserService(user.Id + 1, Roles.User)).GetByIdAsync(storedAttempt.Id));
    }

    [SqlServerIntegrationFact]
    public async Task Progress_UpdatesCompletionPreventsDuplicatesAndEnforcesOwnership()
    {
        await using var context = _fixture.CreateContext();
        await using var transaction = await context.Database.BeginTransactionAsync();
        var user = await CreateUserAsync(context);
        var module = await context.LearningModules.OrderBy(item => item.Id).FirstAsync();
        var service = new UserProgressService(
            new LearningModuleRepository(context),
            new UserProgressRepository(context),
            new TestCurrentUserService(user.Id, Roles.User));

        var created = await service.CreateAsync(user.Id, new CreateUserProgressDto
        {
            LearningModuleId = module.Id,
            ProgressPercentage = 50
        });
        var completed = await service.UpdateAsync(user.Id, module.Id, new UpdateUserProgressDto { ProgressPercentage = 100 });

        Assert.Equal(50, created.ProgressPercentage);
        Assert.NotNull(completed);
        Assert.True(completed.IsCompleted);
        Assert.NotNull(completed.CompletedAt);
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(user.Id, new CreateUserProgressDto
        {
            LearningModuleId = module.Id,
            ProgressPercentage = 10
        }));
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => new UserProgressService(
            new LearningModuleRepository(context),
            new UserProgressRepository(context),
            new TestCurrentUserService(user.Id + 1, Roles.User)).GetByUserAsync(user.Id));
    }

    [SqlServerIntegrationFact]
    public async Task SqlServerConstraints_RejectDuplicateEmailDuplicateProgressAndInvalidForeignKeys()
    {
        await using var context = _fixture.CreateContext();
        await using var transaction = await context.Database.BeginTransactionAsync();
        var user = await CreateUserAsync(context);
        var module = await context.LearningModules.OrderBy(item => item.Id).FirstAsync();
        context.Users.Add(new User
        {
            Name = "Duplicate email",
            Email = user.Email,
            PasswordHash = new PasswordHasherService().HashPassword(CreatePassword()),
            Role = Roles.User
        });

        await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        context.ChangeTracker.Clear();

        context.UserProgress.AddRange(
            new UserProgress { UserId = user.Id, LearningModuleId = module.Id, ProgressPercentage = 10 },
            new UserProgress { UserId = user.Id, LearningModuleId = module.Id, ProgressPercentage = 20 });
        await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        context.ChangeTracker.Clear();

        context.UserProgress.Add(new UserProgress
        {
            UserId = int.MaxValue,
            LearningModuleId = module.Id,
            ProgressPercentage = 10
        });
        await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
    }

    [SqlServerIntegrationFact]
    public async Task DeleteBehaviors_CascadeQuizChildrenSetCategoryNullAndRestrictUsers()
    {
        await using var context = _fixture.CreateContext();
        await using var transaction = await context.Database.BeginTransactionAsync();
        var category = new FraudCategory { Name = $"Integration category {Guid.NewGuid():N}" };
        var module = new LearningModule { Title = $"Integration module {Guid.NewGuid():N}", Content = "Content", FraudCategory = category };
        var quiz = new Quiz
        {
            Title = $"Integration quiz {Guid.NewGuid():N}",
            FraudCategory = category,
            Questions =
            [
                new QuizQuestion
                {
                    QuestionText = "Question",
                    Options = [new QuizOption { OptionText = "Option", IsCorrect = true }]
                }
            ]
        };
        context.AddRange(category, module, quiz);
        await context.SaveChangesAsync();
        var questionId = quiz.Questions.Single().Id;
        var optionId = quiz.Questions.Single().Options.Single().Id;
        context.ChangeTracker.Clear();

        context.Quizzes.Remove(new Quiz { Id = quiz.Id });
        await context.SaveChangesAsync();
        Assert.False(await context.QuizQuestions.AnyAsync(item => item.Id == questionId));
        Assert.False(await context.QuizOptions.AnyAsync(item => item.Id == optionId));

        context.FraudCategories.Remove(new FraudCategory { Id = category.Id });
        await context.SaveChangesAsync();
        Assert.Null(await context.LearningModules.Where(item => item.Id == module.Id)
            .Select(item => item.FraudCategoryId)
            .SingleAsync());

        var user = await CreateUserAsync(context);
        context.UserProgress.Add(new UserProgress { UserId = user.Id, LearningModuleId = module.Id, ProgressPercentage = 10 });
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        context.Users.Remove(new User { Id = user.Id });
        await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
    }

    [SqlServerIntegrationFact]
    public async Task Repositories_FilterOrderLoadRelationshipsAndUseNoTracking()
    {
        await using var context = _fixture.CreateContext();
        await using var transaction = await context.Database.BeginTransactionAsync();
        context.ChangeTracker.Clear();
        var categoryRepository = new FraudCategoryRepository(context);
        var moduleRepository = new LearningModuleRepository(context);
        var scenarioRepository = new ScenarioRepository(context);
        var quizRepository = new QuizRepository(context);
        var badgeRepository = new BadgeRepository(context);

        var categories = await categoryRepository.GetActiveAsync();
        var modules = await moduleRepository.GetPublishedAsync();
        var scenarios = await scenarioRepository.GetPublishedAsync();
        var quizId = await context.Quizzes
            .AsNoTracking()
            .OrderBy(item => item.Id)
            .Select(item => item.Id)
            .FirstAsync();
        var quiz = await quizRepository.GetByIdWithDetailsAsync(quizId);
        var badges = await badgeRepository.GetActiveAsync();

        Assert.Equal(
            categories.Select(item => item.Name).OrderBy(name => name, StringComparer.Ordinal),
            categories.Select(item => item.Name));
        Assert.NotEmpty(modules);
        Assert.NotEmpty(scenarios);
        Assert.NotNull(quiz);
        Assert.NotEmpty(quiz.Questions);
        Assert.NotEmpty(quiz.Questions.SelectMany(question => question.Options));
        Assert.NotEmpty(badges);
        Assert.Empty(context.ChangeTracker.Entries());
    }

    private static async Task<ReferenceDataCounts> CountReferenceDataAsync(ApplicationDbContext context)
    {
        return new ReferenceDataCounts(
            await context.FraudCategories.CountAsync(),
            await context.LearningModules.CountAsync(),
            await context.Scenarios.CountAsync(),
            await context.Quizzes.CountAsync(),
            await context.QuizQuestions.CountAsync(),
            await context.QuizOptions.CountAsync(),
            await context.Badges.CountAsync());
    }

    private static async Task<User> CreateUserAsync(ApplicationDbContext context)
    {
        var user = new User
        {
            Name = "Integration User",
            Email = $"user-{Guid.NewGuid():N}@example.test",
            PasswordHash = new PasswordHasherService().HashPassword(CreatePassword()),
            Role = Roles.User
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    private static string CreatePassword() => $"Integration!9{Guid.NewGuid():N}";

    private sealed record ReferenceDataCounts(
        int Categories,
        int LearningModules,
        int Scenarios,
        int Quizzes,
        int Questions,
        int Options,
        int Badges);

    private sealed class TestCurrentUserService : ICurrentUserService
    {
        public TestCurrentUserService(int userId, string role)
        {
            UserId = userId;
            Role = role;
        }

        public int? UserId { get; }
        public string? Email => null;
        public string? Role { get; }
        public bool IsAuthenticated => true;
        public bool IsAdmin => Role == Roles.Admin;
    }

}
