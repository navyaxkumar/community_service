using DigitalShield.API.DTOs.Scenario;

namespace DigitalShield.API.Interfaces.Services;

public interface IScenarioService
{
    Task<ScenarioResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ScenarioResponseDto?> GetAdminByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<ScenarioResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<ScenarioResponseDto>> GetPublishedAsync(string? audience = null, CancellationToken cancellationToken = default);
    Task<List<ScenarioResponseDto>> GetByCategoryAsync(int fraudCategoryId, CancellationToken cancellationToken = default);
    Task<ScenarioResponseDto> CreateAsync(CreateScenarioDto request, CancellationToken cancellationToken = default);
    Task<ScenarioResponseDto?> UpdateAsync(int id, UpdateScenarioDto request, CancellationToken cancellationToken = default);
    Task<ScenarioResponseDto?> SetPublishStatusAsync(int id, bool isPublished, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
