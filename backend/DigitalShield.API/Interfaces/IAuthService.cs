using DigitalShield.API.DTOs;

namespace DigitalShield.API.Interfaces;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<bool> ValidateCredentialsAsync(string username, string password);
}
