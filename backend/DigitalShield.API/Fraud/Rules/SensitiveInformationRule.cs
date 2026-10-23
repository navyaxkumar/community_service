using System.Text.RegularExpressions;
using DigitalShield.API.Fraud.Analyzer;

namespace DigitalShield.API.Fraud.Rules;

public class SensitiveInformationRule : IFraudRule
{
    private static readonly Regex CredentialPattern = new(
        @"\b(otp|one[- ]time password|pin|password|cvv|card number|bank credentials?|upi pin)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
        TimeSpan.FromMilliseconds(250));

    // Negative / Educational warning indicators
    private static readonly Regex EducationalWarningPattern = new(
        @"\b(never|do not|don't|should not|shouldn't|will never|does not|never ever)\s+(share|disclose|give|send|ask for|request|demand)\b|" +
        @"\b(bank|rbi|police|support|officials?)\s+(will never|never|does not)\s+(ask|request|demand)\b|" +
        @"\b(beware\s+of|protect\s+yourself\s+from|safety\s+tip|security\s+awareness|fraud\s+alert|scam\s+alert)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
        TimeSpan.FromMilliseconds(250));

    // Active credential harvesting patterns
    private static readonly Regex ActiveHarvestPattern = new(
        @"\b(share|send|tell|give|provide|enter|submit|reply with|verify with|forward)\s+(us|me|your|the)?\s*(otp|pin|password|cvv|upi pin|code)\b|" +
        @"\b(otp|pin|password|cvv|credentials?)\s+(is|are)?\s*(required|needed|must be shared|to verify|to unlock|to restore)\b|" +
        @"\benter\s+(your\s+)?(netbanking\s+password|atm\s+pin|upi\s+pin)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
        TimeSpan.FromMilliseconds(250));

    public FraudRuleResult Evaluate(FraudAnalysisInput input)
    {
        if (input.Type != FraudInputType.Message || string.IsNullOrWhiteSpace(input.Content))
        {
            return new FraudRuleResult("SENSITIVE_INFO", "Sensitive information request", 30,
                "No credential request detected.", "Credential request", false);
        }

        var text = input.Content;
        var mentionsCredentials = CredentialPattern.IsMatch(text);
        if (!mentionsCredentials)
        {
            return new FraudRuleResult("SENSITIVE_INFO", "Sensitive information request", 30,
                "No credential request detected.", "Credential request", false);
        }

        var isEducationalWarning = EducationalWarningPattern.IsMatch(text);
        var isActiveHarvest = ActiveHarvestPattern.IsMatch(text);
        var hasUrl = UrlAnalyzer.ExtractFirstUrl(text) != null;

        // If it's an educational warning and doesn't contain an active harvest or link, do not trigger
        if (isEducationalWarning && !isActiveHarvest && !hasUrl)
        {
            return new FraudRuleResult("SENSITIVE_INFO", "Sensitive information advisory", 0,
                "The message contains an educational security advisory advising users NOT to share credentials.",
                "Credential request", false, "Educational security advice detected", "Low");
        }

        // Otherwise, if credentials are requested or demanded
        var match = CredentialPattern.Match(text);
        return new FraudRuleResult(
            "SENSITIVE_INFO",
            "Sensitive information request",
            30,
            "A request for credentials or sensitive financial verification data (such as an OTP, PIN, or password) was detected.",
            "Credential request",
            true,
            match.Value,
            "High");
    }
}
