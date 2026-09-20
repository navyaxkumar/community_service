using DigitalShield.API.DTOs.Badge;

namespace DigitalShield.API.Interfaces.Services;

public interface IBadgeService
{
    Task<BadgeResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<BadgeResponseDto>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<BadgeResponseDto> CreateAsync(CreateBadgeDto request, CancellationToken cancellationToken = default);
    Task<BadgeResponseDto?> UpdateAsync(int id, UpdateBadgeDto request, CancellationToken cancellationToken = default);
}
