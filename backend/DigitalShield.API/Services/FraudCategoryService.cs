using DigitalShield.API.DTOs.FraudCategory;
using DigitalShield.API.Interfaces.Repositories;
using DigitalShield.API.Interfaces.Services;
using DigitalShield.API.Models;

namespace DigitalShield.API.Services;

public class FraudCategoryService : IFraudCategoryService
{
    private readonly IFraudCategoryRepository _repository;

    public FraudCategoryService(IFraudCategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<FraudCategoryResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _repository.GetByIdAsync(id, cancellationToken);
        return category is null ? null : MapToResponse(category);
    }

    public async Task<List<FraudCategoryResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _repository.GetAllAsync(cancellationToken);
        return categories.Select(MapToResponse).ToList();
    }

    public async Task<List<FraudCategoryResponseDto>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _repository.GetActiveAsync(cancellationToken);
        return categories.Select(MapToResponse).ToList();
    }

    public async Task<FraudCategoryResponseDto> CreateAsync(CreateFraudCategoryDto request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var name = request.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Category name is required.", nameof(request));
        }

        var categories = await _repository.GetAllAsync(cancellationToken);
        if (categories.Any(category => string.Equals(category.Name, name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("A category with this name already exists.");
        }

        var category = new FraudCategory
        {
            Name = name,
            Description = request.Description?.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(category, cancellationToken);
        return MapToResponse(category);
    }

    public async Task<FraudCategoryResponseDto?> UpdateAsync(int id, UpdateFraudCategoryDto request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var category = await _repository.GetByIdAsync(id, cancellationToken);
        if (category is null)
        {
            return null;
        }

        var name = request.Name.Trim();
        var categories = await _repository.GetAllAsync(cancellationToken);
        if (categories.Any(item => item.Id != id && string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("A category with this name already exists.");
        }

        category.Name = name;
        category.Description = request.Description?.Trim();
        category.IsActive = request.IsActive;

        await _repository.UpdateAsync(category, cancellationToken);
        return MapToResponse(category);
    }

    private static FraudCategoryResponseDto MapToResponse(FraudCategory category)
    {
        return new FraudCategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt
        };
    }
}
