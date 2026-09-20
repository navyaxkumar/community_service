using DigitalShield.API.Models;

namespace DigitalShield.API.Interfaces.Repositories;

public interface IBadgeRepository
{
    Task<Badge?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Badge>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Badge badge, CancellationToken cancellationToken = default);
    Task UpdateAsync(Badge badge, CancellationToken cancellationToken = default);
}
