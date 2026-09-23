using DigitalShield.API.DTOs.User;

namespace DigitalShield.API.Authentication;

public interface IAuthenticationService
{
    Task<UserResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
}

public sealed class AuthenticationException : Exception
{
    public AuthenticationException(string message)
        : base(message)
    {
    }
}
