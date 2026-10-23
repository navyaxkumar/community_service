namespace DigitalShield.API.Fraud.Scoring;

public enum RiskLevel
{
    Low = 0,
    Suspicious = 1, // Retained for backward-compatibility with tests checking "Suspicious" (Medium)
    High = 2,
    Critical = 3,
    UnableToDetermine = 4
}
