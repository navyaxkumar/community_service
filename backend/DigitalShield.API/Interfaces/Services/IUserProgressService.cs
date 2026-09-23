using DigitalShield.API.DTOs.Progress;

namespace DigitalShield.API.Interfaces.Services;

public interface IUserProgressService
{
    Task<List<UserProgressResponseDto>> GetByUserAsync(int userId, CancellationToken cancellationToken = default);
    Task<UserProgressResponseDto?> GetByUserAndModuleAsync(int userId, int learningModuleId, CancellationToken cancellationToken = default);
    Task<UserProgressResponseDto> CreateAsync(int userId, CreateUserProgressDto request, CancellationToken cancellationToken = default);
    Task<UserProgressResponseDto?> UpdateAsync(int userId, int learningModuleId, UpdateUserProgressDto request, CancellationToken cancellationToken = default);
}
