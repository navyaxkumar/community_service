namespace DigitalShield.API.Interfaces;

public interface IAuthService
{
    Task<bool> ValidateCredentialsAsync(string username, string password);
}
