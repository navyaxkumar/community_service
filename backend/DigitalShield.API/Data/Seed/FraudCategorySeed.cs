using DigitalShield.API.Models;

namespace DigitalShield.API.Data.Seed;

public static class FraudCategorySeed
{
    public const string DigitalSafetyBasicsName = "Digital Safety Basics";

    public static FraudCategory CreateDigitalSafetyBasics()
    {
        return new FraudCategory
        {
            Name = DigitalSafetyBasicsName,
            Description = "Foundation category for DigitalShield seed verification.",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }
}
