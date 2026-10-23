using System.Text.RegularExpressions;
using DigitalShield.API.Fraud.Analyzer;

namespace DigitalShield.API.Fraud.Rules;

public class BrandImpersonationRule : IFraudRule
{
    private static readonly Regex BrandEntityPattern = new(
        @"\b(state bank of india|sbi|hdfc\s+bank|icici\s+bank|axis\s+bank|rbi|reserve bank|india post|fedex|dhl|income tax|epfo|customs\s+department|electricity\s+board|power\s+department)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
        TimeSpan.FromMilliseconds(250));

    private static readonly Regex ThreatActionPattern = new(
        @"\b(locked|blocked|suspended|deactivated|disconnected|arrest|customs duty|clearance fee|unpaid (parcel|package|bill|fee)|penalty|fine|warrant|kyc\s+(expired|pending))\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
        TimeSpan.FromMilliseconds(250));

    public FraudRuleResult Evaluate(FraudAnalysisInput input)
    {
        if (input.Type != FraudInputType.Message || string.IsNullOrWhiteSpace(input.Content))
        {
            return new FraudRuleResult("BRAND_IMPERSONATION", "Institution impersonation", 20,
                "No institutional impersonation detected.", "Impersonation", false);
        }

        var text = input.Content;
        var brandMatch = BrandEntityPattern.Match(text);
        var threatMatch = ThreatActionPattern.Match(text);

        if (brandMatch.Success && threatMatch.Success)
        {
            return new FraudRuleResult(
                "BRAND_IMPERSONATION",
                $"Apparent institutional impersonation ({brandMatch.Value})",
                20,
                $"The message claims to originate from {brandMatch.Value} while presenting urgent threats or fee demands ({threatMatch.Value}). Impersonating trusted authorities is a core social-engineering tactic.",
                "Impersonation",
                true,
                $"{brandMatch.Value} + {threatMatch.Value}",
                "High");
        }

        return new FraudRuleResult("BRAND_IMPERSONATION", "Institution impersonation", 20,
            "No institutional impersonation detected.", "Impersonation", false);
    }
}
