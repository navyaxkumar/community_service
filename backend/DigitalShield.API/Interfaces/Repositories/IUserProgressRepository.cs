using DigitalShield.API.Models;

namespace DigitalShield.API.Interfaces.Repositories;

public interface IUserProgressRepository
{
    Task<UserProgress?> GetByUserAndModuleAsync(int userId, int learningModuleId, CancellationToken cancellationToken = default);
    Task<List<UserProgress>> GetByUserAsync(int userId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int userId, int learningModuleId, CancellationToken cancellationToken = default);
    Task AddAsync(UserProgress progress, CancellationToken cancellationToken = default);
    Task UpdateAsync(UserProgress progress, CancellationToken cancellationToken = default);
}
