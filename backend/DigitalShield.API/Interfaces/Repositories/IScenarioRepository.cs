using DigitalShield.API.Models;

namespace DigitalShield.API.Interfaces.Repositories;

public interface IScenarioRepository
{
    Task<Scenario?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Scenario>> GetPublishedAsync(CancellationToken cancellationToken = default);
    Task<List<Scenario>> GetByCategoryAsync(int fraudCategoryId, CancellationToken cancellationToken = default);
    Task AddAsync(Scenario scenario, CancellationToken cancellationToken = default);
    Task UpdateAsync(Scenario scenario, CancellationToken cancellationToken = default);
}
