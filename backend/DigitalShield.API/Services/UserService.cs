using DigitalShield.API.DTOs.User;
using DigitalShield.API.Authorization;
using DigitalShield.API.Interfaces.Repositories;
using DigitalShield.API.Interfaces.Services;
using DigitalShield.API.Models;

namespace DigitalShield.API.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService? _currentUserService;

    public UserService(IUserRepository userRepository)
        : this(userRepository, null)
    {
    }

    public UserService(IUserRepository userRepository, ICurrentUserService? currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<UserResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        EnsureOwnership(id);
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        return user is null ? null : MapToResponse(user);
    }

    public async Task<UserResponseDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        var user = await _userRepository.GetByEmailAsync(email.Trim(), cancellationToken);
        return user is null ? null : MapToResponse(user);
    }

    public Task<UserResponseDto> CreateAsync(CreateUserDto request, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException(
            "User creation is deferred until the authentication phase provides a password-hashing workflow.");
    }

    public async Task<UserResponseDto?> UpdateAsync(int id, UpdateUserDto request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        EnsureOwnership(id);

        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            return null;
        }

        var email = request.Email.Trim();
        var existingUser = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (existingUser is not null && existingUser.Id != id)
        {
            throw new InvalidOperationException("A user with this email already exists.");
        }

        user.Name = request.Name.Trim();
        user.Email = email;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user, cancellationToken);
        return MapToResponse(user);
    }

    private static UserResponseDto MapToResponse(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            IsActive = user.IsActive
        };
    }

    private void EnsureOwnership(int userId)
    {
        if (_currentUserService is not null &&
            (!_currentUserService.IsAuthenticated || _currentUserService.UserId != userId))
        {
            throw new UnauthorizedAccessException("The authenticated user cannot access this profile.");
        }
    }
}
