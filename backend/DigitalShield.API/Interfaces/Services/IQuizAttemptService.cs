using DigitalShield.API.DTOs.Quiz;

namespace DigitalShield.API.Interfaces.Services;

public interface IQuizAttemptService
{
    Task<QuizAttemptResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<QuizAttemptResponseDto>> GetByUserAsync(int userId, CancellationToken cancellationToken = default);
    Task<List<QuizAttemptResponseDto>> GetByQuizAsync(int quizId, CancellationToken cancellationToken = default);
    Task<QuizAttemptResponseDto> SubmitAttemptAsync(int userId, SubmitQuizAttemptDto request, CancellationToken cancellationToken = default);
}
