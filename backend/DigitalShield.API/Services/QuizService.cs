using DigitalShield.API.DTOs.Quiz;
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

    public async Task<List<QuizResponseDto>> GetPublishedAsync(CancellationToken cancellationToken = default)
    {
        var quizzes = await _repository.GetPublishedAsync(cancellationToken);
        return quizzes.Select(MapToResponse).ToList();
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

        var quiz = new Quiz
        {
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
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
        quiz.Title = request.Title.Trim();
        quiz.Description = request.Description?.Trim();
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

    private async Task EnsureCategoryExistsAsync(int? categoryId, CancellationToken cancellationToken)
    {
        if (categoryId.HasValue && await _categoryRepository.GetByIdAsync(categoryId.Value, cancellationToken) is null)
        {
            throw new InvalidOperationException("Selected fraud category was not found.");
        }
    }

    private static QuizQuestion MapToEntity(CreateQuizQuestionDto request)
    {
        return new QuizQuestion
        {
            QuestionText = request.QuestionText.Trim(),
            Explanation = request.Explanation?.Trim(),
            Order = request.Order,
            Options = request.Options.Select(option => new QuizOption
            {
                OptionText = option.OptionText.Trim(),
                IsCorrect = option.IsCorrect,
                Order = option.Order
            }).ToList()
        };
    }

    private static QuizResponseDto MapToResponse(Quiz quiz)
    {
        return new QuizResponseDto
        {
            Id = quiz.Id,
            Title = quiz.Title,
            Description = quiz.Description,
            IsPublished = quiz.IsPublished,
            FraudCategoryId = quiz.FraudCategoryId,
            CreatedAt = quiz.CreatedAt,
            UpdatedAt = quiz.UpdatedAt
        };
    }

    private static QuizDetailDto MapToUserDetail(Quiz quiz)
    {
        return new QuizDetailDto
        {
            Id = quiz.Id,
            Title = quiz.Title,
            Description = quiz.Description,
            IsPublished = quiz.IsPublished,
            FraudCategoryId = quiz.FraudCategoryId,
            CreatedAt = quiz.CreatedAt,
            UpdatedAt = quiz.UpdatedAt,
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
        return new QuizManagementDetailDto
        {
            Id = quiz.Id,
            Title = quiz.Title,
            Description = quiz.Description,
            IsPublished = quiz.IsPublished,
            FraudCategoryId = quiz.FraudCategoryId,
            CreatedAt = quiz.CreatedAt,
            UpdatedAt = quiz.UpdatedAt,
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
