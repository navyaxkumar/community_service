using DigitalShield.API.DTOs.Learning;

namespace DigitalShield.API.Interfaces.Services;

public interface ILearningModuleService
{
    Task<LearningModuleResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<LearningModuleResponseDto>> GetPublishedAsync(CancellationToken cancellationToken = default);
    Task<List<LearningModuleResponseDto>> GetByCategoryAsync(int fraudCategoryId, CancellationToken cancellationToken = default);
    Task<LearningModuleResponseDto> CreateAsync(CreateLearningModuleDto request, CancellationToken cancellationToken = default);
    Task<LearningModuleResponseDto?> UpdateAsync(int id, UpdateLearningModuleDto request, CancellationToken cancellationToken = default);
}
