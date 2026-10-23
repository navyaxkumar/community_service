using System.Text.RegularExpressions;
using DigitalShield.API.Fraud.Analyzer;

namespace DigitalShield.API.Fraud.Rules;

public class RemoteAccessRule : IFraudRule
{
    private static readonly Regex RemoteSoftwarePattern = new(
        @"\b(anydesk|teamviewer|rustdesk|quicksupport|ultraviewer|airdroid|zoho\s+assist)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
        TimeSpan.FromMilliseconds(250));

    private static readonly Regex ActionableRequestPattern = new(
        @"\b(install|download|open|run|launch|set up)\s+.*?\b(anydesk|teamviewer|rustdesk|quicksupport|ultraviewer|airdroid)\b|" +
        @"\b(anydesk|teamviewer|rustdesk)\b.*?\b(install|download|share\s+(the\s+)?(9[- ]digit|code|id|access|screen))\b|" +
        @"\b(anydesk|teamviewer|rustdesk)\b.*?\b(to\s+(process|receive|verify|fix|unblock|inspect|refund|cancel))\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
        TimeSpan.FromMilliseconds(250));

    private static readonly Regex WarningAdvisoryPattern = new(
        @"\b(never|beware\s+of|scammers?\s+asking|do\s+not)\s+.*?\b(anydesk|teamviewer|rustdesk)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
        TimeSpan.FromMilliseconds(250));

    public FraudRuleResult Evaluate(FraudAnalysisInput input)
    {
        if (input.Type != FraudInputType.Message || string.IsNullOrWhiteSpace(input.Content))
        {
            return new FraudRuleResult("REMOTE_ACCESS", "Remote desktop software request", 30,
                "No remote desktop request detected.", "Device access", false);
        }

        var text = input.Content;
        if (!RemoteSoftwarePattern.IsMatch(text))
        {
            return new FraudRuleResult("REMOTE_ACCESS", "Remote desktop software request", 30,
                "No remote desktop request detected.", "Device access", false);
        }

        // If it's a warning about remote software, do not flag as an active scam demand
        if (WarningAdvisoryPattern.IsMatch(text))
        {
            return new FraudRuleResult("REMOTE_ACCESS", "Remote desktop advisory", 0,
                "Educational warning mentioning remote access software.", "Device access", false, "Advisory context", "Low");
        }

        // Must have actionable installation, code-sharing, or sensitive refund/inspection context
        if (!ActionableRequestPattern.IsMatch(text))
        {
            return new FraudRuleResult("REMOTE_ACCESS", "Remote desktop tool mentioned", 0,
                "Remote administration tool was mentioned in a non-coercive or conversational context.", "Device access", false);
        }

        var match = ActionableRequestPattern.Match(text);
        return new FraudRuleResult(
            "REMOTE_ACCESS",
            "Remote access software installation request",
            30,
            "An instruction to install or grant screen access via remote administration software (such as AnyDesk or TeamViewer) was detected. Fraudsters use this to hijack banking sessions and steal funds.",
            "Device access",
            true,
            match.Value,
            "High");
    }
}
