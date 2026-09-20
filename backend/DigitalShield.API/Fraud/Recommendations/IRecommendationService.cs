using DigitalShield.API.Fraud.Rules;
using DigitalShield.API.Fraud.Scoring;

namespace DigitalShield.API.Fraud.Recommendations;

public interface IRecommendationService
{
    IReadOnlyList<SafetyRecommendation> GetRecommendations(IEnumerable<FraudRuleResult> indicators, RiskLevel riskLevel);
}
