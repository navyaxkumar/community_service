using DigitalShield.API.Fraud.Rules;
using DigitalShield.API.Fraud.Scoring;

namespace DigitalShield.API.Fraud.Recommendations;

public class RecommendationService : IRecommendationService
{
    public IReadOnlyList<SafetyRecommendation> GetRecommendations(IEnumerable<FraudRuleResult> indicators, RiskLevel riskLevel)
    {
        var codes = indicators.Where(indicator => indicator.Triggered).Select(indicator => indicator.Code).ToHashSet(StringComparer.Ordinal);
        var recommendations = new List<SafetyRecommendation>();
        if (codes.Contains("SENSITIVE_INFO"))
        {
            recommendations.Add(new("PROTECT_CREDENTIALS", "Never share an OTP, PIN, password, or card details in response to an unexpected request."));
        }
        if (codes.Contains("PAYMENT_REQUEST"))
        {
            recommendations.Add(new("VERIFY_PAYMENT", "Do not transfer money because of an unexpected message. Verify the request independently."));
        }
        if (codes.Contains("SUSPICIOUS_URL"))
        {
            recommendations.Add(new("VERIFY_LINK", "Open the official website or app directly instead of clicking the suspicious link."));
        }
        if (codes.Contains("UNKNOWN_SENDER"))
        {
            recommendations.Add(new("VERIFY_SENDER", "Verify the sender through an official contact channel."));
        }
        if (riskLevel == RiskLevel.High || riskLevel == RiskLevel.Suspicious)
        {
            recommendations.Add(new("PAUSE_AND_VERIFY", "Pause before acting and contact the organization through an official channel."));
        }
        return recommendations;
    }
}
