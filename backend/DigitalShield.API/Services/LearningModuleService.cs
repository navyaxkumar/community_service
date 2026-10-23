using DigitalShield.API.Constants;
using DigitalShield.API.DTOs.Learning;
using DigitalShield.API.Helpers;
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

    public async Task<LearningModuleResponseDto?> GetAdminByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var module = await _repository.GetByIdAsync(id, cancellationToken);
        return module is null ? null : MapToResponse(module);
    }

    public async Task<List<LearningModuleResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var modules = await _repository.GetAllAsync(cancellationToken);
        return modules.Select(MapToResponse).ToList();
    }

    public async Task<List<LearningModuleResponseDto>> GetPublishedAsync(string? audience = null, CancellationToken cancellationToken = default)
    {
        var modules = await _repository.GetPublishedAsync(cancellationToken);
        var mapped = modules.Select(MapToResponse);

        if (!string.IsNullOrWhiteSpace(audience))
        {
            mapped = mapped.Where(m => AudienceConstants.MatchesAudience(m.TargetAudience, audience));
        }

        return mapped.ToList();
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

        var finalContent = request.Content.Trim();
        if (!string.IsNullOrWhiteSpace(request.TargetAudience) ||
            !string.IsNullOrWhiteSpace(request.DifficultyLevel) ||
            request.EstimatedMinutes.HasValue ||
            !string.IsNullOrWhiteSpace(request.LearningObjectives) ||
            !string.IsNullOrWhiteSpace(request.SafetyTips))
        {
            var meta = new ContentMetadata(
                AudienceConstants.Normalize(request.TargetAudience),
                ContentMetadataParser.NormalizeDifficulty(request.DifficultyLevel),
                request.EstimatedMinutes ?? 5,
                request.LearningObjectives,
                request.SafetyTips);
            finalContent = ContentMetadataParser.EmbedMetadata(finalContent, meta);
        }

        var module = new LearningModule
        {
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            Content = finalContent,
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

        var finalContent = request.Content.Trim();
        if (!string.IsNullOrWhiteSpace(request.TargetAudience) ||
            !string.IsNullOrWhiteSpace(request.DifficultyLevel) ||
            request.EstimatedMinutes.HasValue ||
            !string.IsNullOrWhiteSpace(request.LearningObjectives) ||
            !string.IsNullOrWhiteSpace(request.SafetyTips))
        {
            var meta = new ContentMetadata(
                AudienceConstants.Normalize(request.TargetAudience),
                ContentMetadataParser.NormalizeDifficulty(request.DifficultyLevel),
                request.EstimatedMinutes ?? 5,
                request.LearningObjectives,
                request.SafetyTips);
            finalContent = ContentMetadataParser.EmbedMetadata(finalContent, meta);
        }

        module.Title = request.Title.Trim();
        module.Description = request.Description?.Trim();
        module.Content = finalContent;
        module.Order = request.Order;
        module.IsPublished = request.IsPublished;
        module.FraudCategoryId = request.FraudCategoryId;
        module.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(module, cancellationToken);
        return MapToResponse(module);
    }

    public async Task<LearningModuleResponseDto?> SetPublishStatusAsync(int id, bool isPublished, CancellationToken cancellationToken = default)
    {
        var module = await _repository.GetByIdAsync(id, cancellationToken);
        if (module is null)
        {
            return null;
        }

        if (isPublished)
        {
            if (string.IsNullOrWhiteSpace(module.Title))
            {
                throw new InvalidOperationException("Cannot publish module with an empty title.");
            }
            if (string.IsNullOrWhiteSpace(module.Content))
            {
                throw new InvalidOperationException("Cannot publish module with empty content.");
            }
        }

        module.IsPublished = isPublished;
        module.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(module, cancellationToken);
        return MapToResponse(module);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var module = await _repository.GetByIdAsync(id, cancellationToken);
        if (module is null)
        {
            return false;
        }

        var hasProgress = await _repository.HasUserProgressAsync(id, cancellationToken);
        if (hasProgress)
        {
            throw new InvalidOperationException("Cannot delete learning module with active user learning progress. Unpublish or archive it instead.");
        }

        await _repository.DeleteAsync(module, cancellationToken);
        return true;
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
        var meta = ContentMetadataParser.Parse(module.Content, module.Description, module.Title);
        var cleanContent = ContentMetadataParser.StripMetadata(module.Content);

        return new LearningModuleResponseDto
        {
            Id = module.Id,
            Title = module.Title,
            Description = module.Description,
            Content = string.IsNullOrWhiteSpace(cleanContent) ? module.Content : cleanContent,
            Order = module.Order,
            IsPublished = module.IsPublished,
            FraudCategoryId = module.FraudCategoryId,
            CreatedAt = module.CreatedAt,
            UpdatedAt = module.UpdatedAt,
            TargetAudience = meta.TargetAudience,
            DifficultyLevel = meta.DifficultyLevel,
            EstimatedMinutes = meta.EstimatedMinutes,
            LearningObjectives = meta.LearningObjectives,
            SafetyTips = meta.SafetyTips
        };
    }
}
