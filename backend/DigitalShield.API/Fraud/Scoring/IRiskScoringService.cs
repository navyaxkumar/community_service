using DigitalShield.API.Fraud.Analyzer;
using DigitalShield.API.Fraud.Rules;

namespace DigitalShield.API.Fraud.Scoring;

public interface IRiskScoringService
{
    RiskScoreResult Score(IEnumerable<FraudRuleResult> triggeredRules);
    RiskScoreResult Score(IEnumerable<FraudRuleResult> triggeredRules, FraudAnalysisInput? input);
}
