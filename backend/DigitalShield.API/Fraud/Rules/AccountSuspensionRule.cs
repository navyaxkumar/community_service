using System.Text.RegularExpressions;
using DigitalShield.API.Fraud.Analyzer;

namespace DigitalShield.API.Fraud.Rules;

public class AccountSuspensionRule : IFraudRule
{
    private static readonly Regex Pattern = new(
        @"account\s+(will\s+be\s+)?(blocked|suspended)|kyc\s+required\s+immediately|verify\s+your\s+account\s+now",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.NonBacktracking);

    public FraudRuleResult Evaluate(FraudAnalysisInput input)
    {
        var triggered = input.Type == FraudInputType.Message && Pattern.IsMatch(input.Content);
        return new FraudRuleResult("ACCOUNT_SUSPENSION", "Account suspension warning", 15,
            "A warning about account suspension or urgent verification was detected.", "Account warning", triggered);
    }
}
