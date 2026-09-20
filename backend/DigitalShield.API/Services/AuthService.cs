using DigitalShield.API.Interfaces;

namespace DigitalShield.API.Services;

public class AuthService : IAuthService
{
    public Task<bool> ValidateCredentialsAsync(string username, string password)
    {
        // Placeholder implementation for future authentication logic.
        return Task.FromResult(!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password));
    }
}
