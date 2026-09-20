namespace DigitalShield.API.Fraud.Rules;

public sealed record FraudRuleResult(
    string Code,
    string Title,
    int Score,
    string Explanation,
    string Category,
    bool Triggered);
