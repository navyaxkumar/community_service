using DigitalShield.API.Data;
using DigitalShield.API.DTOs.Badge;
using DigitalShield.API.DTOs.FraudCategory;
using DigitalShield.API.DTOs.Learning;
using DigitalShield.API.DTOs.Progress;
using DigitalShield.API.DTOs.Quiz;
using DigitalShield.API.DTOs.Scenario;
using DigitalShield.API.DTOs.User;
using DigitalShield.API.Models;
using DigitalShield.API.Repositories;
using DigitalShield.API.Services;
using Microsoft.EntityFrameworkCore;

namespace DigitalShield.Tests;

public class ServiceLayerTests
{
    [Fact]
    public async Task UserService_GetById_MapsUserWithoutPasswordHash()
    {
        await using var context = CreateContext();
        context.Users.Add(new User
        {
            Name = "Alice",
            Email = "alice@example.com",
            PasswordHash = "secret-hash",
            Role = "User"
        });
        await context.SaveChangesAsync();

        var result = await new UserService(new UserRepository(context)).GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Alice", result.Name);
        Assert.Null(result.GetType().GetProperty("PasswordHash"));
    }

    [Fact]
    public async Task UserService_Create_RefusesPlaintextPasswordWorkflow()
    {
        await using var context = CreateContext();
        var service = new UserService(new UserRepository(context));

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(new CreateUserDto
        {
            Name = "Alice",
            Email = "alice@example.com",
            Password = "StrongPass1"
        }));
    }

    [Fact]
    public async Task FraudCategoryService_CreateAndGetActive_MapsDtos()
    {
        await using var context = CreateContext();
        var service = new FraudCategoryService(new FraudCategoryRepository(context));

        var created = await service.CreateAsync(new CreateFraudCategoryDto { Name = "Phishing", Description = "Scams" });
        var active = await service.GetActiveAsync();

        Assert.Equal("Phishing", created.Name);
        Assert.Single(active);
        Assert.Equal("Scams", active[0].Description);
    }

    [Fact]
    public async Task LearningModuleService_GetByCategory_ExcludesDrafts()
    {
        await using var context = CreateContext();
        var category = new FraudCategory { Name = "Phishing" };
        context.FraudCategories.Add(category);
        context.LearningModules.AddRange(
            new LearningModule { Title = "Published", Content = "Content", IsPublished = true, Order = 1, FraudCategory = category },
            new LearningModule { Title = "Draft", Content = "Content", IsPublished = false, Order = 2, FraudCategory = category });
        await context.SaveChangesAsync();

        var service = new LearningModuleService(new FraudCategoryRepository(context), new LearningModuleRepository(context));
        var result = await service.GetByCategoryAsync(category.Id);

        Assert.Single(result);
        Assert.Equal("Published", result[0].Title);
    }

    [Fact]
    public async Task LearningModuleService_GetById_DoesNotExposeDraft()
    {
        await using var context = CreateContext();
        var module = new LearningModule { Title = "Draft", Content = "Content", IsPublished = false };
        context.LearningModules.Add(module);
        await context.SaveChangesAsync();

        var service = new LearningModuleService(new FraudCategoryRepository(context), new LearningModuleRepository(context));

        Assert.Null(await service.GetByIdAsync(module.Id));
    }

    [Fact]
    public async Task ScenarioService_GetPublished_ExcludesDrafts()
    {
        await using var context = CreateContext();
        context.Scenarios.AddRange(
            new Scenario { Title = "Published", Situation = "Situation", CorrectAction = "Action", IsPublished = true },
            new Scenario { Title = "Draft", Situation = "Situation", CorrectAction = "Action", IsPublished = false });
        await context.SaveChangesAsync();

        var service = new ScenarioService(new FraudCategoryRepository(context), new ScenarioRepository(context));
        var result = await service.GetPublishedAsync();

        Assert.Single(result);
        Assert.Equal("Published", result[0].Title);
    }

    [Fact]
    public async Task ScenarioService_GetById_DoesNotExposeDraft()
    {
        await using var context = CreateContext();
        var scenario = new Scenario { Title = "Draft", Situation = "Situation", CorrectAction = "Action", IsPublished = false };
        context.Scenarios.Add(scenario);
        await context.SaveChangesAsync();

        var service = new ScenarioService(new FraudCategoryRepository(context), new ScenarioRepository(context));

        Assert.Null(await service.GetByIdAsync(scenario.Id));
    }

    [Fact]
    public async Task QuizService_UserDetailDoesNotExposeCorrectAnswers()
    {
        await using var context = CreateContext();
        var quiz = new Quiz
        {
            Title = "Safety Quiz",
            IsPublished = true,
            Questions = new List<QuizQuestion>
            {
                new()
                {
                    QuestionText = "Protect what?",
                    Options = new List<QuizOption>
                    {
                        new() { OptionText = "OTP", IsCorrect = true, Order = 1 },
                        new() { OptionText = "Nothing", IsCorrect = false, Order = 2 }
                    }
                }
            }
        };
        context.Quizzes.Add(quiz);
        await context.SaveChangesAsync();

        var service = new QuizService(new FraudCategoryRepository(context), new QuizRepository(context));
        var result = await service.GetByIdAsync(quiz.Id);

        Assert.NotNull(result);
        Assert.NotNull(result.Questions[0].Options[0]);
        Assert.Null(result.Questions[0].Options[0].GetType().GetProperty("IsCorrect"));
    }

    [Fact]
    public async Task QuizAttemptService_CalculatesScoreFromStoredAnswers()
    {
        await using var context = CreateContext();
        var quiz = new Quiz
        {
            Title = "Safety Quiz",
            IsPublished = true,
            Questions = new List<QuizQuestion>
            {
                new()
                {
                    QuestionText = "Protect what?",
                    Options = new List<QuizOption>
                    {
                        new() { OptionText = "OTP", IsCorrect = true },
                        new() { OptionText = "Nothing", IsCorrect = false }
                    }
                }
            }
        };
        context.Quizzes.Add(quiz);
        await context.SaveChangesAsync();
        var question = quiz.Questions.Single();
        var correctOption = question.Options.Single(option => option.IsCorrect);

        var service = new QuizAttemptService(new QuizAttemptRepository(context), new QuizRepository(context));
        var result = await service.SubmitAttemptAsync(5, new SubmitQuizAttemptDto
        {
            QuizId = quiz.Id,
            Answers = new List<QuizAnswerDto> { new() { QuestionId = question.Id, OptionId = correctOption.Id } }
        });

        Assert.Equal(1, result.Score);
        Assert.Equal(1, result.TotalQuestions);
        Assert.Equal(1, await context.QuizAttempts.CountAsync());
    }

    [Fact]
    public async Task UserProgressService_RejectsDuplicateUserAndModule()
    {
        await using var context = CreateContext();
        var module = new LearningModule { Title = "Safety", Content = "Content" };
        context.LearningModules.Add(module);
        await context.SaveChangesAsync();
        var service = new UserProgressService(new LearningModuleRepository(context), new UserProgressRepository(context));
        var request = new CreateUserProgressDto { LearningModuleId = module.Id, ProgressPercentage = 50 };

        await service.CreateAsync(7, request);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(7, request));
    }

    [Fact]
    public async Task UserProgressService_Marks100PercentComplete()
    {
        await using var context = CreateContext();
        var module = new LearningModule { Title = "Safety", Content = "Content" };
        context.LearningModules.Add(module);
        await context.SaveChangesAsync();
        var service = new UserProgressService(new LearningModuleRepository(context), new UserProgressRepository(context));

        var result = await service.CreateAsync(7, new CreateUserProgressDto
        {
            LearningModuleId = module.Id,
            ProgressPercentage = 100
        });

        Assert.True(result.IsCompleted);
        Assert.NotNull(result.CompletedAt);
    }

    [Fact]
    public async Task BadgeService_GetActive_MapsBadge()
    {
        await using var context = CreateContext();
        context.Badges.Add(new Badge { Name = "Starter", RequiredPoints = 10, IsActive = true });
        context.Badges.Add(new Badge { Name = "Hidden", RequiredPoints = 20, IsActive = false });
        await context.SaveChangesAsync();

        var result = await new BadgeService(new BadgeRepository(context)).GetActiveAsync();

        Assert.Single(result);
        Assert.Equal("Starter", result[0].Name);
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }
}
