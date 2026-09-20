using DigitalShield.API.DTOs.Badge;
using DigitalShield.API.DTOs.Learning;
using DigitalShield.API.DTOs.Progress;
using DigitalShield.API.DTOs.Quiz;
using DigitalShield.API.DTOs.User;
using DigitalShield.API.Validators;

namespace DigitalShield.Tests;

public class DtoValidationTests
{
    [Fact]
    public void CreateUserDto_ValidInput_PassesValidation()
    {
        var result = new CreateUserDtoValidator().Validate(new CreateUserDto
        {
            Name = "Alice Example",
            Email = "alice@example.com",
            Password = "StrongPass1"
        });

        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreateUserDto_InvalidInput_FailsValidation()
    {
        var validator = new CreateUserDtoValidator();

        Assert.False(validator.Validate(new CreateUserDto { Email = "not-an-email", Password = "weak" }).IsValid);
        Assert.False(validator.Validate(new CreateUserDto { Name = "Alice", Password = "StrongPass1" }).IsValid);
        Assert.False(validator.Validate(new CreateUserDto { Name = "Alice", Email = "alice@example.com", Password = "weak" }).IsValid);
    }

    [Fact]
    public void CreateLearningModuleDto_MissingTitleOrNegativeOrder_FailsValidation()
    {
        var validator = new CreateLearningModuleDtoValidator();

        Assert.False(validator.Validate(new CreateLearningModuleDto
        {
            Content = "Content",
            Order = 0
        }).IsValid);
        Assert.False(validator.Validate(new CreateLearningModuleDto
        {
            Title = "Safety",
            Content = "Content",
            Order = -1
        }).IsValid);
    }

    [Fact]
    public void CreateQuizDto_InvalidQuestionAndOption_FailsValidation()
    {
        var validator = new CreateQuizDtoValidator();
        var result = validator.Validate(new CreateQuizDto
        {
            Title = "Safety Quiz",
            Questions = new List<CreateQuizQuestionDto>
            {
                new()
                {
                    Options = new List<CreateQuizOptionDto>
                    {
                        new() { OptionText = string.Empty }
                    }
                }
            }
        });

        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateQuizDto_ValidNestedQuiz_PassesValidation()
    {
        var result = new CreateQuizDtoValidator().Validate(new CreateQuizDto
        {
            Title = "Safety Quiz",
            Questions = new List<CreateQuizQuestionDto>
            {
                new()
                {
                    QuestionText = "What should you protect?",
                    Options = new List<CreateQuizOptionDto>
                    {
                        new() { OptionText = "Your OTP", IsCorrect = true }
                    }
                }
            }
        });

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(50)]
    [InlineData(100)]
    public void UserProgressDto_ValidPercentages_PassValidation(int percentage)
    {
        var result = new UpdateUserProgressDtoValidator().Validate(new UpdateUserProgressDto
        {
            ProgressPercentage = percentage
        });

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void UserProgressDto_OutOfRangePercentages_FailValidation(int percentage)
    {
        var result = new UpdateUserProgressDtoValidator().Validate(new UpdateUserProgressDto
        {
            ProgressPercentage = percentage
        });

        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateBadgeDto_NegativeRequiredPoints_FailsValidation()
    {
        var result = new CreateBadgeDtoValidator().Validate(new CreateBadgeDto
        {
            Name = "Starter",
            RequiredPoints = -1
        });

        Assert.False(result.IsValid);
    }

    [Fact]
    public void SensitiveFields_AreNotPresentOnOrdinaryWriteDtos()
    {
        var writeDtoTypes = new[]
        {
            typeof(CreateUserDto),
            typeof(UpdateUserDto),
            typeof(CreateUserProgressDto),
            typeof(UpdateUserProgressDto),
            typeof(SubmitQuizAttemptDto)
        };

        foreach (var dtoType in writeDtoTypes)
        {
            Assert.Null(dtoType.GetProperty("PasswordHash"));
            Assert.Null(dtoType.GetProperty("Role"));
            Assert.Null(dtoType.GetProperty("UserId"));
            Assert.Null(dtoType.GetProperty("Score"));
            Assert.Null(dtoType.GetProperty("IsCorrect"));
            Assert.Null(dtoType.GetProperty("CreatedAt"));
            Assert.Null(dtoType.GetProperty("UpdatedAt"));
        }

        Assert.Null(typeof(QuizOptionForUserDto).GetProperty("IsCorrect"));
        Assert.Null(typeof(QuizOptionForUserDto).GetProperty("PasswordHash"));
    }
}
