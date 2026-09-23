namespace DigitalShield.API.Fraud.Analyzer;

public enum FraudInputType
{
    Message,
    Url
}

public sealed record FraudAnalysisInput(
    FraudInputType Type,
    string Content,
    string? Url,
    bool? SenderKnown);
