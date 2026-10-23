using DigitalShield.API.Constants;
using DigitalShield.API.DTOs.Quiz;
using DigitalShield.API.Helpers;
using DigitalShield.API.Interfaces.Repositories;
using DigitalShield.API.Interfaces.Services;
using DigitalShield.API.Models;

namespace DigitalShield.API.Services;

public class QuizService : IQuizService
{
    private readonly IFraudCategoryRepository _categoryRepository;
    private readonly IQuizRepository _repository;

    public QuizService(IFraudCategoryRepository categoryRepository, IQuizRepository repository)
    {
        _categoryRepository = categoryRepository;
        _repository = repository;
    }

    public async Task<QuizDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var quiz = await _repository.GetByIdWithDetailsAsync(id, cancellationToken);
        if (quiz is null || !quiz.IsPublished)
        {
            return null;
        }

        return MapToUserDetail(quiz);
    }

    public async Task<List<QuizResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var quizzes = await _repository.GetAllAsync(cancellationToken);
        return quizzes.Select(MapToResponse).ToList();
    }

    public async Task<List<QuizResponseDto>> GetPublishedAsync(string? audience = null, CancellationToken cancellationToken = default)
    {
        var quizzes = await _repository.GetPublishedAsync(cancellationToken);
        var mapped = quizzes.Select(MapToResponse);

        if (!string.IsNullOrWhiteSpace(audience))
        {
            mapped = mapped.Where(q => AudienceConstants.MatchesAudience(q.TargetAudience, audience));
        }

        return mapped.ToList();
    }

    public async Task<List<QuizResponseDto>> GetByCategoryAsync(int fraudCategoryId, CancellationToken cancellationToken = default)
    {
        var quizzes = await _repository.GetByCategoryAsync(fraudCategoryId, cancellationToken);
        return quizzes.Where(quiz => quiz.IsPublished).Select(MapToResponse).ToList();
    }

    public async Task<QuizResponseDto> CreateAsync(CreateQuizDto request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await EnsureCategoryExistsAsync(request.FraudCategoryId, cancellationToken);

        if (request.IsPublished)
        {
            ValidatePublishRequirements(request.Questions);
        }

        var finalDescription = request.Description?.Trim();
        if (!string.IsNullOrWhiteSpace(request.TargetAudience) || !string.IsNullOrWhiteSpace(request.DifficultyLevel))
        {
            var aud = AudienceConstants.Normalize(request.TargetAudience);
            var diff = ContentMetadataParser.NormalizeDifficulty(request.DifficultyLevel);
            var cleanDesc = ContentMetadataParser.CleanDescription(finalDescription);
            finalDescription = $"[Audience: {aud} | Difficulty: {diff}] {cleanDesc}".Trim();
        }

        var quiz = new Quiz
        {
            Title = request.Title.Trim(),
            Description = finalDescription,
            IsPublished = request.IsPublished,
            FraudCategoryId = request.FraudCategoryId,
            CreatedAt = DateTime.UtcNow,
            Questions = request.Questions.Select(MapToEntity).ToList()
        };

        await _repository.AddAsync(quiz, cancellationToken);
        return MapToResponse(quiz);
    }

    public async Task<QuizResponseDto?> UpdateAsync(int id, UpdateQuizDto request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var quiz = await _repository.GetByIdWithDetailsAsync(id, cancellationToken);
        if (quiz is null)
        {
            return null;
        }

        await EnsureCategoryExistsAsync(request.FraudCategoryId, cancellationToken);

        if (request.IsPublished)
        {
            ValidatePublishRequirements(request.Questions);
        }

        var finalDescription = request.Description?.Trim();
        if (!string.IsNullOrWhiteSpace(request.TargetAudience) || !string.IsNullOrWhiteSpace(request.DifficultyLevel))
        {
            var aud = AudienceConstants.Normalize(request.TargetAudience);
            var diff = ContentMetadataParser.NormalizeDifficulty(request.DifficultyLevel);
            var cleanDesc = ContentMetadataParser.CleanDescription(finalDescription);
            finalDescription = $"[Audience: {aud} | Difficulty: {diff}] {cleanDesc}".Trim();
        }

        quiz.Title = request.Title.Trim();
        quiz.Description = finalDescription;
        quiz.IsPublished = request.IsPublished;
        quiz.FraudCategoryId = request.FraudCategoryId;
        quiz.UpdatedAt = DateTime.UtcNow;
        quiz.Questions.Clear();
        foreach (var question in request.Questions)
        {
            quiz.Questions.Add(MapToEntity(question));
        }

        await _repository.UpdateAsync(quiz, cancellationToken);
        return MapToResponse(quiz);
    }

    public async Task<QuizManagementDetailDto?> GetManagementDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        var quiz = await _repository.GetByIdWithDetailsAsync(id, cancellationToken);
        return quiz is null ? null : MapToManagementDetail(quiz);
    }

    public async Task<QuizResponseDto?> SetPublishStatusAsync(int id, bool isPublished, CancellationToken cancellationToken = default)
    {
        var quiz = await _repository.GetByIdWithDetailsAsync(id, cancellationToken);
        if (quiz is null)
        {
            return null;
        }

        if (isPublished)
        {
            ValidatePublishRequirements(quiz.Questions.Select(MapToCreateDto).ToList());
        }

        quiz.IsPublished = isPublished;
        quiz.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(quiz, cancellationToken);
        return MapToResponse(quiz);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var quiz = await _repository.GetByIdAsync(id, cancellationToken);
        if (quiz is null)
        {
            return false;
        }

        var hasAttempts = await _repository.HasQuizAttemptsAsync(id, cancellationToken);
        if (hasAttempts)
        {
            throw new InvalidOperationException("Cannot delete quiz with existing user attempts. Unpublish or archive it instead.");
        }

        await _repository.DeleteAsync(quiz, cancellationToken);
        return true;
    }

    private async Task EnsureCategoryExistsAsync(int? categoryId, CancellationToken cancellationToken)
    {
        if (categoryId.HasValue && await _categoryRepository.GetByIdAsync(categoryId.Value, cancellationToken) is null)
        {
            throw new InvalidOperationException("Selected fraud category was not found.");
        }
    }

    private static void ValidatePublishRequirements(List<CreateQuizQuestionDto> questions)
    {
        if (questions == null || questions.Count == 0)
        {
            throw new InvalidOperationException("Cannot publish a quiz with no questions. Please add at least one question.");
        }

        foreach (var question in questions)
        {
            if (string.IsNullOrWhiteSpace(question.QuestionText))
            {
                throw new InvalidOperationException("Every question in a published quiz must have non-empty question text.");
            }

            if (question.Options == null || question.Options.Count < 2)
            {
                throw new InvalidOperationException($"Question '{question.QuestionText}' must contain at least two answer options.");
            }

            var hasCorrect = question.Options.Any(o => o.IsCorrect);
            if (!hasCorrect)
            {
                throw new InvalidOperationException($"Question '{question.QuestionText}' must have at least one correct answer option marked.");
            }
        }
    }

    private static QuizQuestion MapToEntity(CreateQuizQuestionDto dto)
    {
        return new QuizQuestion
        {
            QuestionText = dto.QuestionText.Trim(),
            Explanation = dto.Explanation?.Trim(),
            Order = dto.Order,
            Options = dto.Options.Select(optionDto => new QuizOption
            {
                OptionText = optionDto.OptionText.Trim(),
                IsCorrect = optionDto.IsCorrect,
                Order = optionDto.Order
            }).ToList()
        };
    }

    private static CreateQuizQuestionDto MapToCreateDto(QuizQuestion question)
    {
        return new CreateQuizQuestionDto
        {
            QuestionText = question.QuestionText,
            Explanation = question.Explanation,
            Order = question.Order,
            Options = question.Options.Select(option => new CreateQuizOptionDto
            {
                OptionText = option.OptionText,
                IsCorrect = option.IsCorrect,
                Order = option.Order
            }).ToList()
        };
    }

    private static QuizResponseDto MapToResponse(Quiz quiz)
    {
        var meta = ContentMetadataParser.Parse(quiz.Description, quiz.Description, quiz.Title);
        var cleanDesc = ContentMetadataParser.CleanDescription(quiz.Description);

        return new QuizResponseDto
        {
            Id = quiz.Id,
            Title = quiz.Title,
            Description = string.IsNullOrWhiteSpace(cleanDesc) ? quiz.Description : cleanDesc,
            IsPublished = quiz.IsPublished,
            FraudCategoryId = quiz.FraudCategoryId,
            CreatedAt = quiz.CreatedAt,
            UpdatedAt = quiz.UpdatedAt,
            TargetAudience = meta.TargetAudience,
            DifficultyLevel = meta.DifficultyLevel
        };
    }

    private static QuizDetailDto MapToUserDetail(Quiz quiz)
    {
        var meta = ContentMetadataParser.Parse(quiz.Description, quiz.Description, quiz.Title);
        var cleanDesc = ContentMetadataParser.CleanDescription(quiz.Description);

        return new QuizDetailDto
        {
            Id = quiz.Id,
            Title = quiz.Title,
            Description = string.IsNullOrWhiteSpace(cleanDesc) ? quiz.Description : cleanDesc,
            IsPublished = quiz.IsPublished,
            FraudCategoryId = quiz.FraudCategoryId,
            CreatedAt = quiz.CreatedAt,
            UpdatedAt = quiz.UpdatedAt,
            TargetAudience = meta.TargetAudience,
            DifficultyLevel = meta.DifficultyLevel,
            Questions = quiz.Questions.OrderBy(question => question.Order).Select(question => new QuizQuestionForUserDto
            {
                Id = question.Id,
                QuestionText = question.QuestionText,
                Explanation = question.Explanation,
                Order = question.Order,
                Options = question.Options.OrderBy(option => option.Order).Select(option => new QuizOptionForUserDto
                {
                    Id = option.Id,
                    OptionText = option.OptionText,
                    Order = option.Order
                }).ToList()
            }).ToList()
        };
    }

    private static QuizManagementDetailDto MapToManagementDetail(Quiz quiz)
    {
        var meta = ContentMetadataParser.Parse(quiz.Description, quiz.Description, quiz.Title);
        var cleanDesc = ContentMetadataParser.CleanDescription(quiz.Description);

        return new QuizManagementDetailDto
        {
            Id = quiz.Id,
            Title = quiz.Title,
            Description = string.IsNullOrWhiteSpace(cleanDesc) ? quiz.Description : cleanDesc,
            IsPublished = quiz.IsPublished,
            FraudCategoryId = quiz.FraudCategoryId,
            CreatedAt = quiz.CreatedAt,
            UpdatedAt = quiz.UpdatedAt,
            TargetAudience = meta.TargetAudience,
            DifficultyLevel = meta.DifficultyLevel,
            Questions = quiz.Questions.OrderBy(question => question.Order).Select(question => new QuizQuestionDto
            {
                Id = question.Id,
                QuestionText = question.QuestionText,
                Explanation = question.Explanation,
                Order = question.Order,
                Options = question.Options.OrderBy(option => option.Order).Select(option => new QuizOptionDto
                {
                    Id = option.Id,
                    OptionText = option.OptionText,
                    IsCorrect = option.IsCorrect,
                    Order = option.Order
                }).ToList()
            }).ToList()
        };
    }
}
