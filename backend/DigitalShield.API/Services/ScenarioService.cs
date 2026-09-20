using DigitalShield.API.DTOs.Scenario;
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

    public async Task<List<ScenarioResponseDto>> GetPublishedAsync(CancellationToken cancellationToken = default)
    {
        var scenarios = await _repository.GetPublishedAsync(cancellationToken);
        return scenarios.Select(MapToResponse).ToList();
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

        var scenario = new Scenario
        {
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            Situation = request.Situation.Trim(),
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
        scenario.Title = request.Title.Trim();
        scenario.Description = request.Description?.Trim();
        scenario.Situation = request.Situation.Trim();
        scenario.CorrectAction = request.CorrectAction.Trim();
        scenario.IsPublished = request.IsPublished;
        scenario.FraudCategoryId = request.FraudCategoryId;

        await _repository.UpdateAsync(scenario, cancellationToken);
        return MapToResponse(scenario);
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
        return new ScenarioResponseDto
        {
            Id = scenario.Id,
            Title = scenario.Title,
            Description = scenario.Description,
            Situation = scenario.Situation,
            CorrectAction = scenario.CorrectAction,
            IsPublished = scenario.IsPublished,
            FraudCategoryId = scenario.FraudCategoryId,
            CreatedAt = scenario.CreatedAt
        };
    }
}
