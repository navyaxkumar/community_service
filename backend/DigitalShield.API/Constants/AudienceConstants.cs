namespace DigitalShield.API.Constants;

public static class AudienceConstants
{
    public const string All = "All";
    public const string EarlyLearners = "EarlyLearners";
    public const string Children = "Children";
    public const string Teens = "Teens";
    public const string YoungAdults = "YoungAdults";
    public const string Adults = "Adults";
    public const string Seniors = "Seniors";
    public const string Professionals = "Professionals";

    public static readonly IReadOnlyList<string> AllAudiences =
    [
        All,
        EarlyLearners,
        Children,
        Teens,
        YoungAdults,
        Adults,
        Seniors,
        Professionals
    ];

    public static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return All;
        }

        var trimmed = value.Trim().ToLowerInvariant().Replace("-", "").Replace("_", "").Replace(" ", "");

        return trimmed switch
        {
            "early" or "earlylearner" or "earlylearners" or "kids57" or "57" => EarlyLearners,
            "child" or "children" or "kids" or "kids812" or "812" => Children,
            "teen" or "teens" or "teenager" or "teenagers" or "1317" => Teens,
            "youngadult" or "youngadults" or "student" or "students" or "1824" => YoungAdults,
            "adult" or "adults" or "general" => Adults,
            "senior" or "seniors" or "elderly" or "pensioner" or "pensioners" => Seniors,
            "professional" or "professionals" or "business" or "corporate" or "organization" or "enterprise" => Professionals,
            _ => All
        };
    }

    public static bool MatchesAudience(string targetAudience, string? requestedAudience)
    {
        if (string.IsNullOrWhiteSpace(requestedAudience))
        {
            return true;
        }

        var normalizedRequested = Normalize(requestedAudience);
        if (string.Equals(normalizedRequested, All, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var normalizedTarget = Normalize(targetAudience);
        return string.Equals(normalizedTarget, All, StringComparison.OrdinalIgnoreCase) ||
               string.Equals(normalizedTarget, normalizedRequested, StringComparison.OrdinalIgnoreCase);
    }
}
