using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using DigitalShield.API.Authorization;
using DigitalShield.API.Constants;
using DigitalShield.API.Data;
using DigitalShield.API.Data.Seed;
using DigitalShield.API.DTOs.Quiz;
using DigitalShield.API.DTOs.User;
using DigitalShield.API.Fraud.Analyzer;
using DigitalShield.API.Fraud.Recommendations;
using DigitalShield.API.Fraud.Rules;
using DigitalShield.API.Fraud.Scoring;
using DigitalShield.API.Middleware;
using DigitalShield.API.Models;
using DigitalShield.API.Repositories;
using DigitalShield.API.Services;
using DigitalShield.API.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace DigitalShield.Tests;

public class SecurityRegressionTests
{
    [Fact]
    [Trait("Category", "Security")]
    public async Task User_CannotAccessOrUpdateAnotherUsersProfile()
    {
        await using var context = CreateContext();
        context.Users.AddRange(
            new User { Id = 1, Name = "Alice", Email = "alice@example.test", PasswordHash = "hash-a", Role = Roles.User },
            new User { Id = 2, Name = "Bob", Email = "bob@example.test", PasswordHash = "hash-b", Role = Roles.User });
        await context.SaveChangesAsync();
        var service = new UserService(new UserRepository(context), new TestCurrentUserService(2, Roles.User));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.GetByIdAsync(1));
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.UpdateAsync(1, new UpdateUserDto
        {
            Name = "Changed",
            Email = "changed@example.test"
        }));
    }

    [Fact]
    [Trait("Category", "Security")]
    public async Task UserUpdate_MaliciousJsonCannotMassAssignServerControlledFields()
    {
        await using var context = CreateContext();
        var createdAt = DateTime.UtcNow.AddDays(-2);
        context.Users.Add(new User
        {
            Id = 1,
            Name = "Alice",
            Email = "alice@example.test",
            PasswordHash = "original-hash",
            Role = Roles.User,
            IsActive = true,
            CreatedAt = createdAt
        });
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        var service = new UserService(new UserRepository(context), new TestCurrentUserService(1, Roles.User));
        var request = JsonSerializer.Deserialize<UpdateUserDto>("""
        {
          "id": 999,
          "name": "Alice Updated",
          "email": "alice.updated@example.test",
          "role": "Admin",
          "isActive": false,
          "passwordHash": "attacker-hash",
          "createdAt": "2099-01-01T00:00:00Z"
        }
        """, JsonOptions())!;

        var result = await service.UpdateAsync(1, request);
        var stored = await context.Users.SingleAsync();

        Assert.Equal("Alice Updated", result!.Name);
        Assert.Equal(Roles.User, stored.Role);
        Assert.True(stored.IsActive);
        Assert.Equal("original-hash", stored.PasswordHash);
        Assert.Equal(createdAt, stored.CreatedAt);
    }

    [Fact]
    [Trait("Category", "Security")]
    public async Task QuizAttempt_ClientCannotManipulateScoreOrCorrectness()
    {
        await using var context = CreateContext();
        var quiz = CreatePublishedQuiz();
        context.Quizzes.Add(quiz);
        await context.SaveChangesAsync();
        var question = quiz.Questions.Single();
        var wrongOption = question.Options.Single(option => !option.IsCorrect);
        var request = JsonSerializer.Deserialize<SubmitQuizAttemptDto>($$"""
        {
          "quizId": {{quiz.Id}},
          "score": 100,
          "totalQuestions": 1,
          "isCorrect": true,
          "userId": 999,
          "answers": [
            {
              "questionId": {{question.Id}},
              "optionId": {{wrongOption.Id}},
              "isCorrect": true,
              "score": 100
            }
          ]
        }
        """, JsonOptions())!;
        var service = new QuizAttemptService(
            new QuizAttemptRepository(context),
            new QuizRepository(context),
            new TestCurrentUserService(7, Roles.User));

        var result = await service.SubmitAttemptAsync(7, request);

        Assert.Equal(0, result.Score);
        Assert.Equal(1, result.TotalQuestions);
        Assert.Equal(7, (await context.QuizAttempts.SingleAsync()).UserId);
    }

    [Fact]
    [Trait("Category", "Security")]
    public async Task QuizDetail_UserResponseDoesNotSerializeCorrectAnswerMetadata()
    {
        await using var context = CreateContext();
        var quiz = CreatePublishedQuiz();
        context.Quizzes.Add(quiz);
        await context.SaveChangesAsync();
        var service = new QuizService(new FraudCategoryRepository(context), new QuizRepository(context));

        var result = await service.GetByIdAsync(quiz.Id);
        var json = JsonSerializer.Serialize(result, JsonOptions());

        Assert.NotNull(result);
        Assert.DoesNotContain("isCorrect", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("IsCorrect", json, StringComparison.OrdinalIgnoreCase);
        Assert.Null(typeof(QuizOptionForUserDto).GetProperty("IsCorrect"));
    }

    [Fact]
    [Trait("Category", "Security")]
    public async Task SeededQuiz_ServerCalculatesScoreFromStoredCorrectOptions()
    {
        await using var context = CreateContext();
        var category = FraudCategorySeed.CreateAll().Single(category => category.Name == FraudCategorySeed.PhishingOtpScamsName);
        context.FraudCategories.Add(category);
        var quiz = QuizSeed.All.Single(seed => seed.CategoryName == FraudCategorySeed.PhishingOtpScamsName).Create(category);
        context.Quizzes.Add(quiz);
        await context.SaveChangesAsync();

        var answers = quiz.Questions.Select(question => new QuizAnswerDto
        {
            QuestionId = question.Id,
            OptionId = question.Options.Single(option => option.IsCorrect).Id
        }).ToList();
        var service = new QuizAttemptService(
            new QuizAttemptRepository(context),
            new QuizRepository(context),
            new TestCurrentUserService(7, Roles.User));

        var result = await service.SubmitAttemptAsync(7, new SubmitQuizAttemptDto
        {
            QuizId = quiz.Id,
            Answers = answers
        });

        Assert.Equal(quiz.Questions.Count, result.Score);
        Assert.Equal(quiz.Questions.Count, result.TotalQuestions);
        Assert.Equal(7, (await context.QuizAttempts.SingleAsync()).UserId);
    }

    [Fact]
    [Trait("Category", "Security")]
    public async Task QuizAttempt_RejectsDuplicateAnswersAndCrossQuizQuestions()
    {
        await using var context = CreateContext();
        var quiz = CreatePublishedQuiz();
        var otherQuiz = CreatePublishedQuiz("Other quiz");
        context.Quizzes.AddRange(quiz, otherQuiz);
        await context.SaveChangesAsync();
        var question = quiz.Questions.Single();
        var option = question.Options.First();
        var otherQuestion = otherQuiz.Questions.Single();
        var otherOption = otherQuestion.Options.First();
        var service = new QuizAttemptService(
            new QuizAttemptRepository(context),
            new QuizRepository(context),
            new TestCurrentUserService(7, Roles.User));

        await Assert.ThrowsAsync<ArgumentException>(() => service.SubmitAttemptAsync(7, new SubmitQuizAttemptDto
        {
            QuizId = quiz.Id,
            Answers =
            [
                new QuizAnswerDto { QuestionId = question.Id, OptionId = option.Id },
                new QuizAnswerDto { QuestionId = question.Id, OptionId = option.Id }
            ]
        }));
        await Assert.ThrowsAsync<ArgumentException>(() => service.SubmitAttemptAsync(7, new SubmitQuizAttemptDto
        {
            QuizId = quiz.Id,
            Answers = [new QuizAnswerDto { QuestionId = otherQuestion.Id, OptionId = otherOption.Id }]
        }));
    }

    [Fact]
    [Trait("Category", "Security")]
    public async Task QuizAttempt_UserCannotReadAnotherUsersAttempt()
    {
        await using var context = CreateContext();
        var user = new User { Name = "Alice", Email = "alice@example.test", PasswordHash = "hash-a", Role = Roles.User };
        var quiz = new Quiz { Title = "Safety Quiz", IsPublished = true };
        var attempt = new QuizAttempt { User = user, Quiz = quiz, Score = 1, TotalQuestions = 1 };
        context.QuizAttempts.Add(attempt);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        var service = new QuizAttemptService(
            new QuizAttemptRepository(context),
            new QuizRepository(context),
            new TestCurrentUserService(2, Roles.User));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.GetByIdAsync(attempt.Id));
    }

    [Fact]
    [Trait("Category", "Security")]
    public void FraudAnalyzer_TreatsSqlXssAndPathPayloadsAsData()
    {
        var service = new FraudAnalysisService(
            new FraudAnalyzer(new IFraudRule[] { new SensitiveInformationRule(), new SuspiciousUrlRule(), new UrgencyRule() }),
            new RiskScoringService(),
            new RecommendationService());
        var payload = "<script>alert('test')</script> ' OR '1'='1 ../../../../etc/passwd";

        var result = service.Analyze(new DigitalShield.API.DTOs.Fraud.FraudCheckRequestDto
        {
            Type = "Message",
            Content = payload
        });

        Assert.Equal("Low", result.RiskLevel);
        Assert.DoesNotContain(payload, JsonSerializer.Serialize(result), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("Category", "Security")]
    public void FraudUrlValidation_CoversUrlBoundaryAndMalformedCases()
    {
        var validator = new FraudCheckRequestValidator();

        Assert.True(validator.Validate(new DigitalShield.API.DTOs.Fraud.FraudCheckRequestDto { Type = "Url", Url = "https://example.com" }).IsValid);
        Assert.True(validator.Validate(new DigitalShield.API.DTOs.Fraud.FraudCheckRequestDto { Type = "Url", Url = "http://example.com" }).IsValid);
        Assert.True(validator.Validate(new DigitalShield.API.DTOs.Fraud.FraudCheckRequestDto { Type = "Url", Url = "http://127.0.0.1:8080/path?x=1" }).IsValid);
        Assert.True(validator.Validate(new DigitalShield.API.DTOs.Fraud.FraudCheckRequestDto { Type = "Url", Url = "https://xn--pple-43d.example" }).IsValid);
        Assert.False(validator.Validate(new DigitalShield.API.DTOs.Fraud.FraudCheckRequestDto { Type = "Url", Url = "javascript:alert(1)" }).IsValid);
        Assert.False(validator.Validate(new DigitalShield.API.DTOs.Fraud.FraudCheckRequestDto { Type = "Url", Url = "not-a-url" }).IsValid);
        Assert.False(validator.Validate(new DigitalShield.API.DTOs.Fraud.FraudCheckRequestDto { Type = "Url", Url = "https://example.test/" + new string('a', FraudAnalysisService.MaxUrlLength) }).IsValid);
    }

    [Fact]
    [Trait("Category", "Security")]
    public void FraudRegexSecurity_DifficultInputCompletesQuickly()
    {
        var rule = new SuspiciousUrlRule();
        var difficultInput = "https://" + new string('a', 2_000) + new string('.', 200) + " example";
        var stopwatch = Stopwatch.StartNew();

        var result = rule.Evaluate(new FraudAnalysisInput(FraudInputType.Message, difficultInput, null, null));

        stopwatch.Stop();
        Assert.False(result.Triggered);
        Assert.True(stopwatch.ElapsedMilliseconds < 500, $"URL rule took {stopwatch.ElapsedMilliseconds} ms.");
    }

    [Fact]
    [Trait("Category", "Security")]
    public void JwtValidation_RejectsMalformedTokenAndMissingRoleIsNotAdmin()
    {
        var handler = new JwtSecurityTokenHandler();
        Assert.Throws<SecurityTokenMalformedException>(() => handler.ValidateToken("not-a-jwt", CreateValidationParameters(), out _));

        var principal = new CurrentUserService(new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(
                    [new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "5")],
                    "TestAuth"))
            }
        });

        Assert.True(principal.IsAuthenticated);
        Assert.False(principal.IsAdmin);
        Assert.Null(principal.Role);
    }

    [Fact]
    [Trait("Category", "Security")]
    public async Task GlobalErrorHandler_DoesNotLogSensitiveExceptionDetails()
    {
        var logger = new CapturingLogger<GlobalExceptionHandler>();
        var handler = new GlobalExceptionHandler(logger);
        var context = new DefaultHttpContext();
        context.TraceIdentifier = "trace-secure-log";
        context.Response.Body = new MemoryStream();

        await handler.TryHandleAsync(context, new FormatException("password=secret; token=eyJ.fake; C:\\private\\file.cs"), CancellationToken.None);

        Assert.DoesNotContain(logger.Entries, entry => entry.Contains("password=secret", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(logger.Entries, entry => entry.Contains("eyJ.fake", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(logger.Entries, entry => entry.Contains("C:\\private", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(logger.Entries, entry => entry.Contains("trace-secure-log", StringComparison.OrdinalIgnoreCase));
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private static Quiz CreatePublishedQuiz(string title = "Safety Quiz")
    {
        return new Quiz
        {
            Title = title,
            IsPublished = true,
            Questions =
            [
                new QuizQuestion
                {
                    QuestionText = "What should you protect?",
                    Options =
                    [
                        new QuizOption { OptionText = "OTP", IsCorrect = true },
                        new QuizOption { OptionText = "Nothing", IsCorrect = false }
                    ]
                }
            ]
        };
    }

    private static JsonSerializerOptions JsonOptions() => new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static TokenValidationParameters CreateValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "DigitalShield.Tests",
            ValidateAudience = true,
            ValidAudience = "DigitalShield.Tests.Client",
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("test-only-secret-key-that-is-at-least-32-bytes-long")),
            ClockSkew = TimeSpan.Zero
        };
    }

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
        public bool IsAuthenticated => UserId.HasValue;
        public bool IsAdmin => Role == Roles.Admin;
    }

    private sealed class CapturingLogger<T> : ILogger<T>
    {
        public List<string> Entries { get; } = [];

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            Entries.Add(formatter(state, exception));
            if (exception is not null)
            {
                Entries.Add(exception.ToString());
            }
        }

        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();
            public void Dispose()
            {
            }
        }
    }
}
