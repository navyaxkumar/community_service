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
        if (codes.Contains("REMOTE_ACCESS"))
        {
            recommendations.Add(new("REJECT_REMOTE_ACCESS", "Never install remote screen-sharing tools (AnyDesk, TeamViewer) to receive a refund or customer support. Legitimate organizations never request screen control to credit funds."));
        }
        if (codes.Contains("QR_PAYMENT_SCAM"))
        {
            recommendations.Add(new("NEVER_PIN_TO_RECEIVE", "Remember: You NEVER need to enter your UPI PIN or scan a merchant QR code to receive money. A PIN is strictly for authorizing money leaving your account."));
        }
        if (codes.Contains("JOB_SCAM"))
        {
            recommendations.Add(new("AVOID_JOB_FEES", "Do not pay registration, security, or task deposits to obtain employment. Legitimate employers never ask candidates to pay for job placement."));
        }
        if (codes.Contains("BRAND_IMPERSONATION"))
        {
            recommendations.Add(new("CONTACT_OFFICIAL_CHANNEL", "Contact the organization directly through its verified website or customer service phone number, never using links or phone numbers provided in the message."));
        }
        if (codes.Contains("UNKNOWN_SENDER"))
        {
            recommendations.Add(new("VERIFY_SENDER", "Verify the sender through an official contact channel."));
        }

        if (riskLevel == RiskLevel.High || riskLevel == RiskLevel.Critical || riskLevel == RiskLevel.Suspicious)
        {
            recommendations.Add(new("PAUSE_AND_VERIFY", "Pause before acting and contact the organization through an official channel."));
        }

        if (riskLevel == RiskLevel.High || riskLevel == RiskLevel.Critical)
        {
            recommendations.Add(new("INCIDENT_RECOVERY", "If you have already shared an OTP, PIN, or banking details: Contact your bank immediately to freeze your account/cards, and file a report with your local cybercrime reporting portal (e.g. cybercrime.gov.in / 1930)."));
        }

        return recommendations;
    }
}
