using DigitalShield.API.Fraud.Rules;

namespace DigitalShield.API.Fraud.Scoring;

public interface IRiskScoringService
{
    RiskScoreResult Score(IEnumerable<FraudRuleResult> triggeredRules);
}
