namespace DigitalShield.API.Configuration;

public sealed class DevelopmentUserSeedSettings
{
    public const string SectionName = "DevelopmentUserSeeding";

    public bool Enabled { get; set; }

    public string? AdminEmail { get; set; }
    public string? AdminPassword { get; set; }
    public string? UserEmail { get; set; }
    public string? UserPassword { get; set; }
}
