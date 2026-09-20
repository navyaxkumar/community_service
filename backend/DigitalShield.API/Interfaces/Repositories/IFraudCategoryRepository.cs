using DigitalShield.API.Models;

namespace DigitalShield.API.Interfaces.Repositories;

public interface IFraudCategoryRepository
{
    Task<FraudCategory?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<FraudCategory>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<FraudCategory>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task AddAsync(FraudCategory category, CancellationToken cancellationToken = default);
    Task UpdateAsync(FraudCategory category, CancellationToken cancellationToken = default);
}
