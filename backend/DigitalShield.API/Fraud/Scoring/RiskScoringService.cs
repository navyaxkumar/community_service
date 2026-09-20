using DigitalShield.API.Fraud.Rules;

namespace DigitalShield.API.Fraud.Scoring;

public class RiskScoringService : IRiskScoringService
{
    public RiskScoreResult Score(IEnumerable<FraudRuleResult> triggeredRules)
    {
        var score = triggeredRules
            .Where(result => result.Triggered)
            .GroupBy(result => result.Code, StringComparer.Ordinal)
            .Select(group => group.First().Score)
            .Sum();
        var boundedScore = Math.Clamp(score, 0, 100);
        var riskLevel = boundedScore >= 60
            ? RiskLevel.High
            : boundedScore >= 30
                ? RiskLevel.Suspicious
                : RiskLevel.Low;
        return new RiskScoreResult(boundedScore, riskLevel);
    }
}
