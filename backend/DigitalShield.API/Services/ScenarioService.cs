using DigitalShield.API.Constants;
using DigitalShield.API.DTOs.Scenario;
using DigitalShield.API.Helpers;
using DigitalShield.API.Interfaces.Repositories;
using DigitalShield.API.Interfaces.Services;
using DigitalShield.API.Models;

namespace DigitalShield.API.Services;

public class ScenarioService : IScenarioService
{
    private readonly IFraudCategoryRepository _categoryRepository;
    private readonly IScenarioRepository _repository;

    public ScenarioService(IFraudCategoryRepository categoryRepository, IScenarioRepository repository)
    {
        _categoryRepository = categoryRepository;
        _repository = repository;
    }

    public async Task<ScenarioResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var scenario = await _repository.GetByIdAsync(id, cancellationToken);
        return scenario is null || !scenario.IsPublished ? null : MapToResponse(scenario);
    }

    public async Task<ScenarioResponseDto?> GetAdminByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var scenario = await _repository.GetByIdAsync(id, cancellationToken);
        return scenario is null ? null : MapToResponse(scenario);
    }

    public async Task<List<ScenarioResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var scenarios = await _repository.GetAllAsync(cancellationToken);
        return scenarios.Select(MapToResponse).ToList();
    }

    public async Task<List<ScenarioResponseDto>> GetPublishedAsync(string? audience = null, CancellationToken cancellationToken = default)
    {
        var scenarios = await _repository.GetPublishedAsync(cancellationToken);
        var mapped = scenarios.Select(MapToResponse);

        if (!string.IsNullOrWhiteSpace(audience))
        {
            mapped = mapped.Where(s => AudienceConstants.MatchesAudience(s.TargetAudience, audience));
        }

        return mapped.ToList();
    }

    public async Task<List<ScenarioResponseDto>> GetByCategoryAsync(int fraudCategoryId, CancellationToken cancellationToken = default)
    {
        var scenarios = await _repository.GetByCategoryAsync(fraudCategoryId, cancellationToken);
        return scenarios.Where(scenario => scenario.IsPublished).Select(MapToResponse).ToList();
    }

    public async Task<ScenarioResponseDto> CreateAsync(CreateScenarioDto request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await EnsureCategoryExistsAsync(request.FraudCategoryId, cancellationToken);

        var finalSituation = request.Situation.Trim();
        if (!string.IsNullOrWhiteSpace(request.TargetAudience) ||
            !string.IsNullOrWhiteSpace(request.DifficultyLevel) ||
            !string.IsNullOrWhiteSpace(request.SafetyTips))
        {
            var meta = new ContentMetadata(
                AudienceConstants.Normalize(request.TargetAudience),
                ContentMetadataParser.NormalizeDifficulty(request.DifficultyLevel),
                3,
                null,
                request.SafetyTips);
            finalSituation = ContentMetadataParser.EmbedMetadata(finalSituation, meta);
        }

        var scenario = new Scenario
        {
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            Situation = finalSituation,
            CorrectAction = request.CorrectAction.Trim(),
            IsPublished = request.IsPublished,
            FraudCategoryId = request.FraudCategoryId,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(scenario, cancellationToken);
        return MapToResponse(scenario);
    }

    public async Task<ScenarioResponseDto?> UpdateAsync(int id, UpdateScenarioDto request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var scenario = await _repository.GetByIdAsync(id, cancellationToken);
        if (scenario is null)
        {
            return null;
        }

        await EnsureCategoryExistsAsync(request.FraudCategoryId, cancellationToken);

        var finalSituation = request.Situation.Trim();
        if (!string.IsNullOrWhiteSpace(request.TargetAudience) ||
            !string.IsNullOrWhiteSpace(request.DifficultyLevel) ||
            !string.IsNullOrWhiteSpace(request.SafetyTips))
        {
            var meta = new ContentMetadata(
                AudienceConstants.Normalize(request.TargetAudience),
                ContentMetadataParser.NormalizeDifficulty(request.DifficultyLevel),
                3,
                null,
                request.SafetyTips);
            finalSituation = ContentMetadataParser.EmbedMetadata(finalSituation, meta);
        }

        scenario.Title = request.Title.Trim();
        scenario.Description = request.Description?.Trim();
        scenario.Situation = finalSituation;
        scenario.CorrectAction = request.CorrectAction.Trim();
        scenario.IsPublished = request.IsPublished;
        scenario.FraudCategoryId = request.FraudCategoryId;

        await _repository.UpdateAsync(scenario, cancellationToken);
        return MapToResponse(scenario);
    }

    public async Task<ScenarioResponseDto?> SetPublishStatusAsync(int id, bool isPublished, CancellationToken cancellationToken = default)
    {
        var scenario = await _repository.GetByIdAsync(id, cancellationToken);
        if (scenario is null)
        {
            return null;
        }

        if (isPublished)
        {
            if (string.IsNullOrWhiteSpace(scenario.Title))
            {
                throw new InvalidOperationException("Cannot publish scenario with an empty title.");
            }
            if (string.IsNullOrWhiteSpace(scenario.Situation))
            {
                throw new InvalidOperationException("Cannot publish scenario with an empty situation.");
            }
            if (string.IsNullOrWhiteSpace(scenario.CorrectAction))
            {
                throw new InvalidOperationException("Cannot publish scenario with empty correct action.");
            }
        }

        scenario.IsPublished = isPublished;
        await _repository.UpdateAsync(scenario, cancellationToken);
        return MapToResponse(scenario);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var scenario = await _repository.GetByIdAsync(id, cancellationToken);
        if (scenario is null)
        {
            return false;
        }

        await _repository.DeleteAsync(scenario, cancellationToken);
        return true;
    }

    private async Task EnsureCategoryExistsAsync(int? categoryId, CancellationToken cancellationToken)
    {
        if (categoryId.HasValue && await _categoryRepository.GetByIdAsync(categoryId.Value, cancellationToken) is null)
        {
            throw new InvalidOperationException("Selected fraud category was not found.");
        }
    }

    private static ScenarioResponseDto MapToResponse(Scenario scenario)
    {
        var meta = ContentMetadataParser.Parse(scenario.Situation, scenario.Description, scenario.Title);
        var cleanSituation = ContentMetadataParser.StripMetadata(scenario.Situation);

        return new ScenarioResponseDto
        {
            Id = scenario.Id,
            Title = scenario.Title,
            Description = scenario.Description,
            Situation = string.IsNullOrWhiteSpace(cleanSituation) ? scenario.Situation : cleanSituation,
            CorrectAction = scenario.CorrectAction,
            IsPublished = scenario.IsPublished,
            FraudCategoryId = scenario.FraudCategoryId,
            CreatedAt = scenario.CreatedAt,
            TargetAudience = meta.TargetAudience,
            DifficultyLevel = meta.DifficultyLevel,
            SafetyTips = meta.SafetyTips
        };
    }
}
