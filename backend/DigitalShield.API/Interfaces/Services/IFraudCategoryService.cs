using DigitalShield.API.DTOs.FraudCategory;

namespace DigitalShield.API.Interfaces.Services;

public interface IFraudCategoryService
{
    Task<FraudCategoryResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<FraudCategoryResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<FraudCategoryResponseDto>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<FraudCategoryResponseDto> CreateAsync(CreateFraudCategoryDto request, CancellationToken cancellationToken = default);
    Task<FraudCategoryResponseDto?> UpdateAsync(int id, UpdateFraudCategoryDto request, CancellationToken cancellationToken = default);
}
