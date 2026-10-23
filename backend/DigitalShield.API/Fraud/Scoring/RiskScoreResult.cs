namespace DigitalShield.API.Fraud.Scoring;

public sealed record RiskScoreResult(
    int Score,
    RiskLevel RiskLevel,
    string? Reason = null,
    bool IsInconclusive = false);
