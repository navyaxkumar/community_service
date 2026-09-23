using DigitalShield.API.DTOs.Learning;
using DigitalShield.API.Interfaces.Repositories;
using DigitalShield.API.Interfaces.Services;
using DigitalShield.API.Models;

namespace DigitalShield.API.Services;

public class LearningModuleService : ILearningModuleService
{
    private readonly IFraudCategoryRepository _categoryRepository;
    private readonly ILearningModuleRepository _repository;

    public LearningModuleService(IFraudCategoryRepository categoryRepository, ILearningModuleRepository repository)
    {
        _categoryRepository = categoryRepository;
        _repository = repository;
    }

    public async Task<LearningModuleResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var module = await _repository.GetByIdAsync(id, cancellationToken);
        return module is null || !module.IsPublished ? null : MapToResponse(module);
    }

    public async Task<List<LearningModuleResponseDto>> GetPublishedAsync(CancellationToken cancellationToken = default)
    {
        var modules = await _repository.GetPublishedAsync(cancellationToken);
        return modules.Select(MapToResponse).ToList();
    }

    public async Task<List<LearningModuleResponseDto>> GetByCategoryAsync(int fraudCategoryId, CancellationToken cancellationToken = default)
    {
        var modules = await _repository.GetByCategoryAsync(fraudCategoryId, cancellationToken);
        return modules.Where(module => module.IsPublished).Select(MapToResponse).ToList();
    }

    public async Task<LearningModuleResponseDto> CreateAsync(CreateLearningModuleDto request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await EnsureCategoryExistsAsync(request.FraudCategoryId, cancellationToken);

        var module = new LearningModule
        {
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            Content = request.Content.Trim(),
            Order = request.Order,
            IsPublished = request.IsPublished,
            FraudCategoryId = request.FraudCategoryId,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(module, cancellationToken);
        return MapToResponse(module);
    }

    public async Task<LearningModuleResponseDto?> UpdateAsync(int id, UpdateLearningModuleDto request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var module = await _repository.GetByIdAsync(id, cancellationToken);
        if (module is null)
        {
            return null;
        }

        await EnsureCategoryExistsAsync(request.FraudCategoryId, cancellationToken);
        module.Title = request.Title.Trim();
        module.Description = request.Description?.Trim();
        module.Content = request.Content.Trim();
        module.Order = request.Order;
        module.IsPublished = request.IsPublished;
        module.FraudCategoryId = request.FraudCategoryId;
        module.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(module, cancellationToken);
        return MapToResponse(module);
    }

    private async Task EnsureCategoryExistsAsync(int? categoryId, CancellationToken cancellationToken)
    {
        if (categoryId.HasValue && await _categoryRepository.GetByIdAsync(categoryId.Value, cancellationToken) is null)
        {
            throw new InvalidOperationException("Selected fraud category was not found.");
        }
    }

    private static LearningModuleResponseDto MapToResponse(LearningModule module)
    {
        return new LearningModuleResponseDto
        {
            Id = module.Id,
            Title = module.Title,
            Description = module.Description,
            Content = module.Content,
            Order = module.Order,
            IsPublished = module.IsPublished,
            FraudCategoryId = module.FraudCategoryId,
            CreatedAt = module.CreatedAt,
            UpdatedAt = module.UpdatedAt
        };
    }
}
