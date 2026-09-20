using DigitalShield.API.DTOs.User;

namespace DigitalShield.API.Interfaces.Services;

public interface IUserService
{
    Task<UserResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<UserResponseDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserResponseDto> CreateAsync(CreateUserDto request, CancellationToken cancellationToken = default);
    Task<UserResponseDto?> UpdateAsync(int id, UpdateUserDto request, CancellationToken cancellationToken = default);
}
