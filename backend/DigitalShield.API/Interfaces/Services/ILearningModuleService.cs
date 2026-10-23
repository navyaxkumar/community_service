using DigitalShield.API.DTOs.Learning;

namespace DigitalShield.API.Interfaces.Services;

public interface ILearningModuleService
{
    Task<LearningModuleResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<LearningModuleResponseDto?> GetAdminByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<LearningModuleResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<LearningModuleResponseDto>> GetPublishedAsync(string? audience = null, CancellationToken cancellationToken = default);
    Task<List<LearningModuleResponseDto>> GetByCategoryAsync(int fraudCategoryId, CancellationToken cancellationToken = default);
    Task<LearningModuleResponseDto> CreateAsync(CreateLearningModuleDto request, CancellationToken cancellationToken = default);
    Task<LearningModuleResponseDto?> UpdateAsync(int id, UpdateLearningModuleDto request, CancellationToken cancellationToken = default);
    Task<LearningModuleResponseDto?> SetPublishStatusAsync(int id, bool isPublished, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
