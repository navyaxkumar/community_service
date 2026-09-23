using DigitalShield.API.Models;

namespace DigitalShield.API.Interfaces.Repositories;

public interface ILearningModuleRepository
{
    Task<LearningModule?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<LearningModule>> GetPublishedAsync(CancellationToken cancellationToken = default);
    Task<List<LearningModule>> GetByCategoryAsync(int fraudCategoryId, CancellationToken cancellationToken = default);
    Task AddAsync(LearningModule learningModule, CancellationToken cancellationToken = default);
    Task UpdateAsync(LearningModule learningModule, CancellationToken cancellationToken = default);
}
