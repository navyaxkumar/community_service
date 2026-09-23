using DigitalShield.API.DTOs.Badge;
using DigitalShield.API.Interfaces.Repositories;
using DigitalShield.API.Interfaces.Services;
using DigitalShield.API.Models;

namespace DigitalShield.API.Services;

public class BadgeService : IBadgeService
{
    private readonly IBadgeRepository _repository;

    public BadgeService(IBadgeRepository repository)
    {
        _repository = repository;
    }

    public async Task<BadgeResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var badge = await _repository.GetByIdAsync(id, cancellationToken);
        return badge is null ? null : MapToResponse(badge);
    }

    public async Task<List<BadgeResponseDto>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var badges = await _repository.GetActiveAsync(cancellationToken);
        return badges.Select(MapToResponse).ToList();
    }

    public async Task<BadgeResponseDto> CreateAsync(CreateBadgeDto request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var badge = new Badge
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            Icon = request.Icon?.Trim(),
            RequiredPoints = request.RequiredPoints,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(badge, cancellationToken);
        return MapToResponse(badge);
    }

    public async Task<BadgeResponseDto?> UpdateAsync(int id, UpdateBadgeDto request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var badge = await _repository.GetByIdAsync(id, cancellationToken);
        if (badge is null)
        {
            return null;
        }

        badge.Name = request.Name.Trim();
        badge.Description = request.Description?.Trim();
        badge.Icon = request.Icon?.Trim();
        badge.RequiredPoints = request.RequiredPoints;
        badge.IsActive = request.IsActive;

        await _repository.UpdateAsync(badge, cancellationToken);
        return MapToResponse(badge);
    }

    private static BadgeResponseDto MapToResponse(Badge badge)
    {
        return new BadgeResponseDto
        {
            Id = badge.Id,
            Name = badge.Name,
            Description = badge.Description,
            Icon = badge.Icon,
            RequiredPoints = badge.RequiredPoints,
            IsActive = badge.IsActive,
            CreatedAt = badge.CreatedAt
        };
    }
}
