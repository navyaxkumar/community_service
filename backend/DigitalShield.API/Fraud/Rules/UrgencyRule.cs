using System.Text.RegularExpressions;
using DigitalShield.API.Fraud.Analyzer;

namespace DigitalShield.API.Fraud.Rules;

public class UrgencyRule : IFraudRule
{
    private static readonly Regex UrgencyPattern = new(
        @"\b(urgent|urgently|immediately|act now|within\s+\d+\s+(minutes?|hours?)|last warning|final notice|immediate action|right now|within\s+24\s+hours?)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.NonBacktracking);

    // Benign everyday / calendar / workplace communication contexts
    private static readonly Regex BenignContextPattern = new(
        @"\b(meeting|appointment|rescheduled|calendar|sync|lunch|dinner|flight|gate|agenda|slide deck|homework|class|office)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.NonBacktracking);

    // Threatening / Coercive context indicators
    private static readonly Regex ThreatContextPattern = new(
        @"\b(suspend|block|freeze|cancel|terminate|arrest|police|legal|court|penalt|fine|lawsuit|forfeit|locked|expire|deactivat|otp|pin|password|upi|bank|kyc)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.NonBacktracking);

    public FraudRuleResult Evaluate(FraudAnalysisInput input)
    {
        if (input.Type != FraudInputType.Message || string.IsNullOrWhiteSpace(input.Content))
        {
            return new FraudRuleResult("URGENCY", "Urgency or threat", 10,
                "No artificial urgency detected.", "Pressure", false);
        }

        var text = input.Content.Trim();
        var match = UrgencyPattern.Match(text);
        if (!match.Success)
        {
            return new FraudRuleResult("URGENCY", "Urgency or threat", 10,
                "No artificial urgency detected.", "Pressure", false);
        }

        // If the message is a meeting/workplace scheduling note without any threat context,
        // do not treat conversational urgency as fraud pressure.
        if (BenignContextPattern.IsMatch(text) && !ThreatContextPattern.IsMatch(text))
        {
            return new FraudRuleResult("URGENCY", "Urgency or threat", 10,
                "The message contains conversational urgency in a benign context (e.g. meeting or appointment scheduling).",
                "Pressure", false, match.Value, "Low");
        }

        return new FraudRuleResult("URGENCY", "Urgency or threat", 10,
            "Artificial pressure to act quickly or immediate threats were detected.",
            "Pressure", true, match.Value, "Medium");
    }
}
