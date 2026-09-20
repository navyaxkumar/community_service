using DigitalShield.API.Models;

namespace DigitalShield.API.Authentication;

public interface IJwtTokenService
{
    JwtTokenResult CreateToken(User user);
}

public sealed record JwtTokenResult(string AccessToken, DateTime ExpiresAt);
