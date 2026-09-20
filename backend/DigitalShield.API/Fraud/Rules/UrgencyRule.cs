using System.Text.RegularExpressions;
using DigitalShield.API.Fraud.Analyzer;

namespace DigitalShield.API.Fraud.Rules;

public class UrgencyRule : IFraudRule
{
    private static readonly Regex Pattern = new(
        @"\b(urgent|immediately|act now|within\s+\d+\s+minutes?|last warning)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.NonBacktracking);

    public FraudRuleResult Evaluate(FraudAnalysisInput input)
    {
        var triggered = input.Type == FraudInputType.Message && Pattern.IsMatch(input.Content);
        return new FraudRuleResult("URGENCY", "Urgency or threat", 10,
            "Pressure to act quickly was detected.", "Pressure", triggered);
    }
}
