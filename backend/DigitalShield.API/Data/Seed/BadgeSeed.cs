using DigitalShield.API.Models;

namespace DigitalShield.API.Data.Seed;

public static class BadgeSeed
{
    public const string AwarenessStarterName = "Awareness Starter";

    public static Badge CreateAwarenessStarter()
    {
        return new Badge
        {
            Name = AwarenessStarterName,
            Description = "Foundation badge for DigitalShield seed verification.",
            Icon = "shield-check",
            RequiredPoints = 0,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }
}
