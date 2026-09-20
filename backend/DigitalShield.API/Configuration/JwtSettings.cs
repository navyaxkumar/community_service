namespace DigitalShield.API.Configuration;

using System.Text;

public class JwtSettings
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Issuer))
        {
            throw new InvalidOperationException("JWT issuer is not configured.");
        }

        if (string.IsNullOrWhiteSpace(Audience))
        {
            throw new InvalidOperationException("JWT audience is not configured.");
        }

        if (string.IsNullOrWhiteSpace(SecretKey) || Encoding.UTF8.GetByteCount(SecretKey) < 32)
        {
            throw new InvalidOperationException("JWT secret key must be configured and at least 32 bytes long.");
        }

        if (ExpiryMinutes <= 0)
        {
            throw new InvalidOperationException("JWT expiration must be greater than zero minutes.");
        }
    }
}
