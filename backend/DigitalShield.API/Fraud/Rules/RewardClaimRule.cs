using System.Text.RegularExpressions;
using DigitalShield.API.Fraud.Analyzer;

namespace DigitalShield.API.Fraud.Rules;

public class RewardClaimRule : IFraudRule
{
    private static readonly Regex Pattern = new(
        @"\b(you\s+won|claim\s+(your\s+)?prize|cash\s+reward|lottery|gift|reward)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.NonBacktracking);

    public FraudRuleResult Evaluate(FraudAnalysisInput input)
    {
        var triggered = input.Type == FraudInputType.Message && Pattern.IsMatch(input.Content);
        return new FraudRuleResult("REWARD_CLAIM", "Reward or prize claim", 15,
            "A reward, prize, or lottery claim indicator was detected.", "Reward", triggered);
    }
}
