using DigitalShield.API.Models;

namespace DigitalShield.API.Interfaces.Repositories;

public interface IQuizRepository
{
    Task<Quiz?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Quiz?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Quiz>> GetPublishedAsync(CancellationToken cancellationToken = default);
    Task<List<Quiz>> GetByCategoryAsync(int fraudCategoryId, CancellationToken cancellationToken = default);
    Task AddAsync(Quiz quiz, CancellationToken cancellationToken = default);
    Task UpdateAsync(Quiz quiz, CancellationToken cancellationToken = default);
}
