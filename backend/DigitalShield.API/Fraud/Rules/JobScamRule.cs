using System.Text.RegularExpressions;
using DigitalShield.API.Fraud.Analyzer;

namespace DigitalShield.API.Fraud.Rules;

public class JobScamRule : IFraudRule
{
    private static readonly Regex TaskJobPattern = new(
        @"\b(part[- ]time\s+job|work\s+from\s+home|online\s+job|earn\s+(daily|money|rs\.?\s*\d+))\b.*?\b(liking|rating|reviewing|subscribe|youtube\s+videos?|hotel\s+reviews?|telegram\s+tasks?|prepaid\s+task)\b|" +
        @"\b(earn\s+rs\.?\s*\d+\s*(to|-)\s*rs\.?\s*\d+\s*(per\s+day|daily))\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
        TimeSpan.FromMilliseconds(250));

    private static readonly Regex AdvanceFeePattern = new(
        @"\b(pay|deposit|send|transfer)\s+.*?\b(registration\s+fee|security\s+deposit|training\s+charge|processing\s+fee|refundable\s+deposit)\b|" +
        @"\b(registration\s+fee|security\s+deposit|processing\s+charge)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
        TimeSpan.FromMilliseconds(250));

    private static readonly Regex WarningPattern = new(
        @"\b(legitimate\s+employers?\s+never|beware\s+of\s+fake\s+jobs?|no\s+real\s+company\s+asks\s+for\s+money|how\s+to\s+spot\s+fake\s+jobs?)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
        TimeSpan.FromMilliseconds(250));

    public FraudRuleResult Evaluate(FraudAnalysisInput input)
    {
        if (input.Type != FraudInputType.Message || string.IsNullOrWhiteSpace(input.Content))
        {
            return new FraudRuleResult("JOB_SCAM", "Fake job or employment task scam", 25,
                "No employment scam indicators detected.", "Employment", false);
        }

        var text = input.Content;

        // Skip educational warnings about fake jobs
        if (WarningPattern.IsMatch(text))
        {
            return new FraudRuleResult("JOB_SCAM", "Employment scam advisory", 0,
                "Educational discussion regarding employment fraud.", "Employment", false);
        }

        var taskMatch = TaskJobPattern.Match(text);
        var feeMatch = AdvanceFeePattern.Match(text);

        if (taskMatch.Success || feeMatch.Success)
        {
            var evidence = feeMatch.Success ? feeMatch.Value : taskMatch.Value;
            var explanation = feeMatch.Success
                ? "The offer demands an upfront registration, security, or training deposit to secure employment. Genuine employers never charge job applicants for appointment letters."
                : "The message promises lucrative daily earnings for simplistic tasks (e.g., liking videos or rating hotels), a hallmark of task-based deposit scams.";

            return new FraudRuleResult(
                "JOB_SCAM",
                "Fake job offer or task deposit scam",
                35,
                explanation,
                "Employment",
                true,
                evidence,
                "High");
        }

        return new FraudRuleResult("JOB_SCAM", "Fake job or employment task scam", 25,
            "No employment scam indicators detected.", "Employment", false);
    }
}
