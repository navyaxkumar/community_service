using System.Text.RegularExpressions;
using DigitalShield.API.Fraud.Analyzer;

namespace DigitalShield.API.Fraud.Rules;

public class EducationalContextRule : IFraudRule
{
    private static readonly Regex AdvisoryPattern = new(
        @"\b(safety tip|security advisory|cybercrime awareness|fraud awareness|psa|rbi\s+notice|educational purpose|safe banking habits?|remember that your bank will never|never share your (otp|pin|password)|how to (identify|spot|avoid) (fake|scams?)|legitimate (banks?|employers?|companies|institutions?) (never|will never))\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
        TimeSpan.FromMilliseconds(250));

    public FraudRuleResult Evaluate(FraudAnalysisInput input)
    {
        if (input.Type != FraudInputType.Message || string.IsNullOrWhiteSpace(input.Content))
        {
            return new FraudRuleResult("EDUCATIONAL_CONTEXT", "Educational safety context", 0,
                "No educational framing detected.", "Education", false);
        }

        var match = AdvisoryPattern.Match(input.Content);
        if (!match.Success)
        {
            return new FraudRuleResult("EDUCATIONAL_CONTEXT", "Educational safety context", 0,
                "No educational framing detected.", "Education", false);
        }

        return new FraudRuleResult(
            "EDUCATIONAL_CONTEXT",
            "Educational security awareness context",
            0,
            "The content is phrased as a security guideline, consumer warning, or fraud awareness tip.",
            "Education",
            true,
            match.Value,
            "Info");
    }
}
