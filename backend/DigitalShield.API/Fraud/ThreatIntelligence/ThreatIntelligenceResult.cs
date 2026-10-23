namespace DigitalShield.API.Fraud.ThreatIntelligence;

public sealed record ThreatIntelligenceResult(
    bool IsChecked,
    bool IsMalicious,
    string? ThreatType,
    DateTime? CheckedAt,
    string ProviderName,
    string? Details);
