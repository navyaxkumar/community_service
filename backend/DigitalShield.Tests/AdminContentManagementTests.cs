using DigitalShield.API.Constants;
using DigitalShield.API.Controllers;
using DigitalShield.API.Data;
using DigitalShield.API.DTOs.Admin;
using DigitalShield.API.DTOs.Common;
using DigitalShield.API.DTOs.FraudCategory;
using DigitalShield.API.DTOs.Learning;
using DigitalShield.API.DTOs.Quiz;
using DigitalShield.API.DTOs.Scenario;
using DigitalShield.API.Models;
using DigitalShield.API.Repositories;
using DigitalShield.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DigitalShield.Tests;

public class AdminContentManagementTests
{
    [Fact]
    public async Task LearningModules_DraftSeparation_AdminCanViewDraft_OrdinaryUserCannotAccessDraft()
    {
        await using var context = CreateContext();
        var catRepo = new FraudCategoryRepository(context);
        var modRepo = new LearningModuleRepository(context);
        var service = new LearningModuleService(catRepo, modRepo);

        var draft = await service.CreateAsync(new CreateLearningModuleDto
        {
            Title = "Draft Lesson on Smishing",
            Content = "Educational text on SMS phishing.",
            Order = 1,
            IsPublished = false
        });

        // 1. Ordinary user querying public endpoint returns null for draft
        var publicResult = await service.GetByIdAsync(draft.Id);
        Assert.Null(publicResult);

        // 2. Ordinary user querying published list does not see draft
        var publishedList = await service.GetPublishedAsync();
        Assert.DoesNotContain(publishedList, m => m.Id == draft.Id);

        // 3. Authorized administrator querying admin endpoints can view and list draft
        var adminResult = await service.GetAdminByIdAsync(draft.Id);
        Assert.NotNull(adminResult);
        Assert.False(adminResult.IsPublished);
        Assert.Equal("Draft Lesson on Smishing", adminResult.Title);

        var allList = await service.GetAllAsync();
        Assert.Contains(allList, m => m.Id == draft.Id);
    }

    [Fact]
    public async Task LearningModules_AdminCanCreateAndEdit_PersistsChanges()
    {
        await using var context = CreateContext();
        var service = new LearningModuleService(new FraudCategoryRepository(context), new LearningModuleRepository(context));

        var created = await service.CreateAsync(new CreateLearningModuleDto
        {
            Title = "Initial Title",
            Content = "Initial Content",
            Order = 1,
            IsPublished = false
        });

        var updated = await service.UpdateAsync(created.Id, new UpdateLearningModuleDto
        {
            Title = "Updated Title by Admin",
            Description = "Updated Description",
            Content = "Updated Lesson Content",
            Order = 2,
            IsPublished = false
        });

        Assert.NotNull(updated);
        Assert.Equal("Updated Title by Admin", updated.Title);
        Assert.Equal("Updated Description", updated.Description);
        Assert.Equal(2, updated.Order);

        // Verify direct persistence in repository
        var reloaded = await service.GetAdminByIdAsync(created.Id);
        Assert.NotNull(reloaded);
        Assert.Equal("Updated Title by Admin", reloaded.Title);
        Assert.Equal("Updated Lesson Content", reloaded.Content);
    }

    [Fact]
    public async Task LearningModules_PublishAndUnpublishWorkflow_TogglesPublicVisibility()
    {
        await using var context = CreateContext();
        var service = new LearningModuleService(new FraudCategoryRepository(context), new LearningModuleRepository(context));

        var module = await service.CreateAsync(new CreateLearningModuleDto
        {
            Title = "Safe Banking Habits",
            Content = "Always verify banking URLs directly.",
            Order = 1,
            IsPublished = false
        });

        // Initially unpublished
        Assert.Null(await service.GetByIdAsync(module.Id));

        // Admin publishes module
        var published = await service.SetPublishStatusAsync(module.Id, true);
        Assert.NotNull(published);
        Assert.True(published.IsPublished);

        // Public learner can now access it
        var learnerView = await service.GetByIdAsync(module.Id);
        Assert.NotNull(learnerView);
        Assert.Equal("Safe Banking Habits", learnerView.Title);

        var publicModules = await service.GetPublishedAsync();
        Assert.Contains(publicModules, m => m.Id == module.Id);

        // Admin unpublishes module
        var unpublished = await service.SetPublishStatusAsync(module.Id, false);
        Assert.NotNull(unpublished);
        Assert.False(unpublished.IsPublished);

        // Public learner can no longer access it
        Assert.Null(await service.GetByIdAsync(module.Id));
        publicModules = await service.GetPublishedAsync();
        Assert.DoesNotContain(publicModules, m => m.Id == module.Id);
    }

    [Fact]
    public async Task LearningModules_CannotPublishModuleWithEmptyContent()
    {
        await using var context = CreateContext();
        var repo = new LearningModuleRepository(context);
        var service = new LearningModuleService(new FraudCategoryRepository(context), repo);

        var rawModule = new LearningModule
        {
            Title = "Incomplete Module",
            Content = "   ",
            IsPublished = false,
            Order = 1
        };
        await repo.AddAsync(rawModule);

        // Attempting to publish invalid/empty content must fail
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.SetPublishStatusAsync(rawModule.Id, true));
    }

    [Fact]
    public async Task LearningModules_SafeDeletion_CannotDeleteModuleWithUserProgress()
    {
        await using var context = CreateContext();
        var service = new LearningModuleService(new FraudCategoryRepository(context), new LearningModuleRepository(context));

        var module = await service.CreateAsync(new CreateLearningModuleDto
        {
            Title = "Module With Historical Progress",
            Content = "Important curriculum.",
            IsPublished = true
        });

        var user = new User { Name = "Learner", Email = "learner@example.com", PasswordHash = "hash", Role = Roles.User };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        context.UserProgress.Add(new UserProgress
        {
            UserId = user.Id,
            LearningModuleId = module.Id,
            ProgressPercentage = 100,
            IsCompleted = true
        });
        await context.SaveChangesAsync();

        // Safe deletion must reject deleting module with progress
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteAsync(module.Id));
        Assert.Contains("progress", ex.Message, StringComparison.OrdinalIgnoreCase);

        // Ensure module still exists
        Assert.NotNull(await service.GetAdminByIdAsync(module.Id));
    }

    [Fact]
    public async Task Quizzes_DraftSeparation_AdminCanViewDraftAndCorrectAnswers_OrdinaryUserCannot()
    {
        await using var context = CreateContext();
        var service = new QuizService(new FraudCategoryRepository(context), new QuizRepository(context));

        var quiz = await service.CreateAsync(new CreateQuizDto
        {
            Title = "Draft Quiz on UPI Scams",
            Description = "Assess UPI safety",
            IsPublished = false,
            Questions = new List<CreateQuizQuestionDto>
            {
                new()
                {
                    QuestionText = "Do you ever enter your UPI PIN to receive money?",
                    Explanation = "UPI PIN is strictly for debiting money.",
                    Order = 1,
                    Options = new List<CreateQuizOptionDto>
                    {
                        new() { OptionText = "No, never", IsCorrect = true, Order = 1 },
                        new() { OptionText = "Yes, always", IsCorrect = false, Order = 2 }
                    }
                }
            }
        });

        // 1. Learner querying public endpoint receives null for draft quiz
        var learnerView = await service.GetByIdAsync(quiz.Id);
        Assert.Null(learnerView);

        // 2. Admin querying management details receives full details with correct answer indicators
        var adminView = await service.GetManagementDetailsAsync(quiz.Id);
        Assert.NotNull(adminView);
        Assert.False(adminView.IsPublished);
        var question = Assert.Single(adminView.Questions);
        Assert.Equal(2, question.Options.Count);
        Assert.True(question.Options.Single(o => o.OptionText == "No, never").IsCorrect);

        // 3. Publish quiz and confirm learner view hides IsCorrect property
        await service.SetPublishStatusAsync(quiz.Id, true);
        var publishedLearnerView = await service.GetByIdAsync(quiz.Id);
        Assert.NotNull(publishedLearnerView);
        var learnerQuestion = Assert.Single(publishedLearnerView.Questions);
        Assert.Equal(2, learnerQuestion.Options.Count);
        // QuizOptionForUserDto has no IsCorrect property
        Assert.Null(typeof(QuizOptionForUserDto).GetProperty("IsCorrect"));
    }

    [Fact]
    public async Task Quizzes_CannotPublishQuizWithoutQuestionsOrWithoutCorrectOption()
    {
        await using var context = CreateContext();
        var repo = new QuizRepository(context);
        var service = new QuizService(new FraudCategoryRepository(context), repo);

        // Case A: Quiz with no questions cannot be published
        var emptyQuiz = new Quiz { Title = "Empty Quiz", IsPublished = false };
        await repo.AddAsync(emptyQuiz);
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.SetPublishStatusAsync(emptyQuiz.Id, true));

        // Case B: Quiz with question having no correct option cannot be published
        var invalidQuiz = new Quiz
        {
            Title = "Invalid Quiz",
            IsPublished = false,
            Questions = new List<QuizQuestion>
            {
                new()
                {
                    QuestionText = "Which one?",
                    Options = new List<QuizOption>
                    {
                        new() { OptionText = "Choice 1", IsCorrect = false },
                        new() { OptionText = "Choice 2", IsCorrect = false }
                    }
                }
            }
        };
        await repo.AddAsync(invalidQuiz);
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.SetPublishStatusAsync(invalidQuiz.Id, true));
        Assert.Contains("correct answer", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Quizzes_SafeDeletion_CannotDeleteQuizWithUserAttempts()
    {
        await using var context = CreateContext();
        var service = new QuizService(new FraudCategoryRepository(context), new QuizRepository(context));

        var quiz = await service.CreateAsync(new CreateQuizDto
        {
            Title = "Protected Quiz",
            IsPublished = true,
            Questions = new List<CreateQuizQuestionDto>
            {
                new()
                {
                    QuestionText = "Sample?",
                    Options = new List<CreateQuizOptionDto>
                    {
                        new() { OptionText = "A", IsCorrect = true },
                        new() { OptionText = "B", IsCorrect = false }
                    }
                }
            }
        });

        var user = new User { Name = "Quiz Taker", Email = "taker@example.com", PasswordHash = "hash", Role = Roles.User };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        context.QuizAttempts.Add(new QuizAttempt
        {
            UserId = user.Id,
            QuizId = quiz.Id,
            Score = 1,
            TotalQuestions = 1
        });
        await context.SaveChangesAsync();

        // Safe deletion must reject deleting quiz with recorded attempts
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteAsync(quiz.Id));
        Assert.Contains("attempts", ex.Message, StringComparison.OrdinalIgnoreCase);

        // Ensure quiz still exists
        Assert.NotNull(await service.GetManagementDetailsAsync(quiz.Id));
    }

    [Fact]
    public async Task Scenarios_PublishAndUnpublishWorkflow_ControlsPublicAccess()
    {
        await using var context = CreateContext();
        var service = new ScenarioService(new FraudCategoryRepository(context), new ScenarioRepository(context));

        var scenario = await service.CreateAsync(new CreateScenarioDto
        {
            Title = "Suspicious Tech Call",
            Situation = "Caller asks you to download AnyDesk.",
            CorrectAction = "Hang up immediately and do not install remote tools.",
            IsPublished = false
        });

        // Learner cannot access draft
        Assert.Null(await service.GetByIdAsync(scenario.Id));

        // Admin publishes
        await service.SetPublishStatusAsync(scenario.Id, true);
        var published = await service.GetByIdAsync(scenario.Id);
        Assert.NotNull(published);
        Assert.Equal("Suspicious Tech Call", published.Title);

        // Admin unpublishes
        await service.SetPublishStatusAsync(scenario.Id, false);
        Assert.Null(await service.GetByIdAsync(scenario.Id));
    }

    [Fact]
    public async Task FraudCategories_SafeDeletion_CannotDeleteCategoryWithLinkedModules()
    {
        await using var context = CreateContext();
        var catService = new FraudCategoryService(new FraudCategoryRepository(context));

        var category = await catService.CreateAsync(new CreateFraudCategoryDto
        {
            Name = "Active Phishing Vector",
            Description = "Phishing threats"
        });

        context.LearningModules.Add(new LearningModule
        {
            Title = "Module Under Category",
            Content = "Content",
            FraudCategoryId = category.Id
        });
        await context.SaveChangesAsync();

        // Safe deletion must reject deleting category that has linked content
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => catService.DeleteAsync(category.Id));
        Assert.Contains("associated", ex.Message, StringComparison.OrdinalIgnoreCase);

        // Ensure category still exists
        Assert.NotNull(await catService.GetByIdAsync(category.Id));
    }

    [Fact]
    public async Task AdminDashboardStats_ComputesRealPersistedCounts()
    {
        await using var context = CreateContext();

        // Add 2 published modules, 1 draft module
        context.LearningModules.AddRange(
            new LearningModule { Title = "Mod 1", Content = "C", IsPublished = true },
            new LearningModule { Title = "Mod 2", Content = "C", IsPublished = true },
            new LearningModule { Title = "Mod 3", Content = "C", IsPublished = false }
        );

        // Add 1 published quiz with 2 questions, 1 draft quiz with 1 question
        context.Quizzes.AddRange(
            new Quiz
            {
                Title = "Quiz 1",
                IsPublished = true,
                Questions = new List<QuizQuestion>
                {
                    new() { QuestionText = "Q1" },
                    new() { QuestionText = "Q2" }
                }
            },
            new Quiz
            {
                Title = "Quiz 2",
                IsPublished = false,
                Questions = new List<QuizQuestion>
                {
                    new() { QuestionText = "Q3" }
                }
            }
        );

        // Add 1 scenario
        context.Scenarios.Add(new Scenario { Title = "Scenario 1", Situation = "S", CorrectAction = "A", IsPublished = true });

        // Add 2 categories: 1 active, 1 inactive
        context.FraudCategories.AddRange(
            new FraudCategory { Name = "Active Cat", IsActive = true },
            new FraudCategory { Name = "Inactive Cat", IsActive = false }
        );

        // Add 1 badge
        context.Badges.Add(new Badge { Name = "Sentinel Badge", RequiredPoints = 50, IsActive = true });

        await context.SaveChangesAsync();

        var controller = new AdminDashboardController(context);
        var actionResult = await controller.GetDashboardStats(CancellationToken.None);
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var stats = Assert.IsType<AdminDashboardStatsDto>(okResult.Value);

        // Assert exact counts match persisted entities
        Assert.Equal(3, stats.TotalModules);
        Assert.Equal(2, stats.PublishedModules);
        Assert.Equal(1, stats.DraftModules);

        Assert.Equal(2, stats.TotalQuizzes);
        Assert.Equal(1, stats.PublishedQuizzes);
        Assert.Equal(1, stats.DraftQuizzes);
        Assert.Equal(3, stats.TotalQuizQuestions);

        Assert.Equal(1, stats.TotalScenarios);
        Assert.Equal(1, stats.PublishedScenarios);
        Assert.Equal(0, stats.DraftScenarios);

        Assert.Equal(2, stats.TotalCategories);
        Assert.Equal(1, stats.ActiveCategories);
        Assert.Equal(1, stats.InactiveCategories);

        Assert.Equal(1, stats.TotalBadges);
        Assert.Equal(1, stats.ActiveBadges);
    }

    [Theory]
    [InlineData(typeof(AdminDashboardController))]
    [InlineData(typeof(LearningModulesController), "GetAllAdmin")]
    [InlineData(typeof(LearningModulesController), "GetByIdAdmin")]
    [InlineData(typeof(LearningModulesController), "SetPublishStatus")]
    [InlineData(typeof(LearningModulesController), "Delete")]
    [InlineData(typeof(QuizzesController), "GetAllAdmin")]
    [InlineData(typeof(QuizzesController), "GetByIdAdmin")]
    [InlineData(typeof(QuizzesController), "SetPublishStatus")]
    [InlineData(typeof(QuizzesController), "Delete")]
    [InlineData(typeof(ScenariosController), "GetAllAdmin")]
    [InlineData(typeof(ScenariosController), "GetByIdAdmin")]
    [InlineData(typeof(ScenariosController), "SetPublishStatus")]
    [InlineData(typeof(ScenariosController), "Delete")]
    [InlineData(typeof(FraudCategoriesController), "GetAllAdmin")]
    [InlineData(typeof(FraudCategoriesController), "Delete")]
    [InlineData(typeof(BadgesController), "GetAllAdmin")]
    [InlineData(typeof(BadgesController), "Delete")]
    public void AdminControllers_EnforceAdminRole_OnManagementEndpoints(Type controllerType, string? actionName = null)
    {
        if (actionName == null)
        {
            var controllerAuthorize = controllerType.GetCustomAttribute<AuthorizeAttribute>();
            Assert.NotNull(controllerAuthorize);
            Assert.Equal(Roles.Admin, controllerAuthorize!.Roles);
        }
        else
        {
            var method = controllerType.GetMethod(actionName);
            Assert.NotNull(method);
            var actionAuthorize = method.GetCustomAttribute<AuthorizeAttribute>();
            Assert.NotNull(actionAuthorize);
            Assert.Equal(Roles.Admin, actionAuthorize!.Roles);
        }
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }
}
