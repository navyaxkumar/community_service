using DigitalShield.API.Fraud.Analyzer;

namespace DigitalShield.API.Fraud.Rules;

public class SuspiciousUrlRule : IFraudRule
{
    public FraudRuleResult Evaluate(FraudAnalysisInput input)
    {
        var rawUrl = input.Type == FraudInputType.Url
            ? input.Url
            : UrlAnalyzer.ExtractFirstUrl(input.Content);

        if (string.IsNullOrWhiteSpace(rawUrl))
        {
            return new FraudRuleResult("SUSPICIOUS_URL", "Suspicious URL", 25,
                "No suspicious link detected.", "Link", false);
        }

        var findings = UrlAnalyzer.Analyze(rawUrl);
        if (findings.Count == 0)
        {
            return new FraudRuleResult("SUSPICIOUS_URL", "Suspicious URL", 25,
                "The link structure has no obvious deception indicators.", "Link", false);
        }

        // Select the most severe finding
        var topFinding = findings.OrderByDescending(f => f.Score).First();

        return new FraudRuleResult(
            "SUSPICIOUS_URL",
            topFinding.Title,
            topFinding.Score,
            topFinding.Explanation,
            "Link",
            true,
            topFinding.Evidence,
            topFinding.Severity);
    }
}
