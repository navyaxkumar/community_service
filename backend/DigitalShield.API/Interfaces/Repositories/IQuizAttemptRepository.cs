using DigitalShield.API.Models;

namespace DigitalShield.API.Interfaces.Repositories;

public interface IQuizAttemptRepository
{
    Task<QuizAttempt?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<QuizAttempt>> GetByUserAsync(int userId, CancellationToken cancellationToken = default);
    Task<List<QuizAttempt>> GetByQuizAsync(int quizId, CancellationToken cancellationToken = default);
    Task AddAsync(QuizAttempt attempt, CancellationToken cancellationToken = default);
}
