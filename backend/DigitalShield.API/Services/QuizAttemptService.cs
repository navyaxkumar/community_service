using DigitalShield.API.DTOs.Quiz;
using DigitalShield.API.Authorization;
using DigitalShield.API.Interfaces.Repositories;
using DigitalShield.API.Interfaces.Services;
using DigitalShield.API.Models;

namespace DigitalShield.API.Services;

public class QuizAttemptService : IQuizAttemptService
{
    private readonly IQuizAttemptRepository _attemptRepository;
    private readonly IQuizRepository _quizRepository;
    private readonly ICurrentUserService? _currentUserService;

    public QuizAttemptService(IQuizAttemptRepository attemptRepository, IQuizRepository quizRepository)
        : this(attemptRepository, quizRepository, null)
    {
    }

    public QuizAttemptService(
        IQuizAttemptRepository attemptRepository,
        IQuizRepository quizRepository,
        ICurrentUserService? currentUserService)
    {
        _attemptRepository = attemptRepository;
        _quizRepository = quizRepository;
        _currentUserService = currentUserService;
    }

    public async Task<QuizAttemptResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var attempt = await _attemptRepository.GetByIdAsync(id, cancellationToken);
        if (attempt is not null)
        {
            EnsureOwnership(attempt.UserId);
        }
        return attempt is null ? null : MapToResponse(attempt);
    }

    public async Task<List<QuizAttemptResponseDto>> GetByUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        EnsureOwnership(userId);
        var attempts = await _attemptRepository.GetByUserAsync(userId, cancellationToken);
        return attempts.Select(MapToResponse).ToList();
    }

    public async Task<List<QuizAttemptResponseDto>> GetByQuizAsync(int quizId, CancellationToken cancellationToken = default)
    {
        var attempts = await _attemptRepository.GetByQuizAsync(quizId, cancellationToken);
        return attempts.Select(MapToResponse).ToList();
    }

    public async Task<QuizAttemptResponseDto> SubmitAttemptAsync(int userId, SubmitQuizAttemptDto request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        EnsureOwnership(userId);

        var quiz = await _quizRepository.GetByIdWithDetailsAsync(request.QuizId, cancellationToken);
        if (quiz is null || !quiz.IsPublished)
        {
            throw new InvalidOperationException("Published quiz was not found.");
        }

        var answersByQuestion = request.Answers
            .GroupBy(answer => answer.QuestionId)
            .ToList();
        if (answersByQuestion.Any(group => group.Count() > 1))
        {
            throw new ArgumentException("Only one answer may be submitted per question.", nameof(request));
        }

        var questionsById = quiz.Questions.ToDictionary(question => question.Id);
        var score = 0;
        foreach (var answer in request.Answers)
        {
            if (!questionsById.TryGetValue(answer.QuestionId, out var question))
            {
                throw new ArgumentException("An answer references a question outside this quiz.", nameof(request));
            }

            var option = question.Options.FirstOrDefault(candidate => candidate.Id == answer.OptionId);
            if (option is null)
            {
                throw new ArgumentException("An answer references an option outside its question.", nameof(request));
            }

            if (option.IsCorrect)
            {
                score++;
            }
        }

        var attempt = new QuizAttempt
        {
            UserId = userId,
            QuizId = quiz.Id,
            Score = score,
            TotalQuestions = quiz.Questions.Count,
            StartedAt = DateTime.UtcNow,
            CompletedAt = DateTime.UtcNow
        };

        await _attemptRepository.AddAsync(attempt, cancellationToken);
        return MapToResponse(attempt);
    }

    private static QuizAttemptResponseDto MapToResponse(QuizAttempt attempt)
    {
        return new QuizAttemptResponseDto
        {
            Id = attempt.Id,
            QuizId = attempt.QuizId,
            Score = attempt.Score,
            TotalQuestions = attempt.TotalQuestions,
            StartedAt = attempt.StartedAt,
            CompletedAt = attempt.CompletedAt
        };
    }

    private void EnsureOwnership(int userId)
    {
        if (_currentUserService is not null &&
            (!_currentUserService.IsAuthenticated || _currentUserService.UserId != userId))
        {
            throw new UnauthorizedAccessException("The authenticated user cannot access this quiz attempt.");
        }
    }
}
