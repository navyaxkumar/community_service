using System.Text.RegularExpressions;
using DigitalShield.API.Fraud.Analyzer;

namespace DigitalShield.API.Fraud.Rules;

public class SensitiveInformationRule : IFraudRule
{
    private static readonly Regex Pattern = new(
        @"\b(otp|one[- ]time password|pin|password|cvv|card number|bank credentials?|upi pin)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.NonBacktracking);

    public FraudRuleResult Evaluate(FraudAnalysisInput input)
    {
        var triggered = input.Type == FraudInputType.Message && Pattern.IsMatch(input.Content);
        return new FraudRuleResult("SENSITIVE_INFO", "Sensitive information request", 30,
            "A request for credentials or sensitive financial information was detected.", "Credential request", triggered);
    }
}
