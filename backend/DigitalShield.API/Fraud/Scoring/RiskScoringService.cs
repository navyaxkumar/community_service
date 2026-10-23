using DigitalShield.API.Fraud.Analyzer;
using DigitalShield.API.Fraud.Rules;

namespace DigitalShield.API.Fraud.Scoring;

public class RiskScoringService : IRiskScoringService
{
    // Documented, centralized risk thresholds
    public const int LowThresholdMax = 29;
    public const int SuspiciousThresholdMin = 30;
    public const int SuspiciousThresholdMax = 59;
    public const int HighThresholdMin = 60;
    public const int CriticalThresholdMin = 85;

    public RiskScoreResult Score(IEnumerable<FraudRuleResult> triggeredRules)
    {
        return ScoreInternal(triggeredRules, null);
    }

    public RiskScoreResult Score(IEnumerable<FraudRuleResult> triggeredRules, FraudAnalysisInput? input)
    {
        return ScoreInternal(triggeredRules, input);
    }

    private static RiskScoreResult ScoreInternal(IEnumerable<FraudRuleResult> triggeredRules, FraudAnalysisInput? input)
    {
        var activeRules = triggeredRules
            .Where(result => result.Triggered)
            .GroupBy(result => result.Code, StringComparer.Ordinal)
            .Select(group => group.First())
            .ToList();

        if (activeRules.Count == 0)
        {
            if (input != null && input.Type == FraudInputType.Message && string.IsNullOrWhiteSpace(input.Content))
            {
                return new RiskScoreResult(0, RiskLevel.UnableToDetermine, "No evaluable content was provided.", true);
            }

            return new RiskScoreResult(0, RiskLevel.Low, "No active deception or fraud indicators were detected.");
        }

        var activeCodes = activeRules.Select(r => r.Code).ToHashSet(StringComparer.Ordinal);
        var isEducationalContext = activeCodes.Contains("EDUCATIONAL_CONTEXT");

        // High-severity deception indicators that override educational framing if genuinely present
        var highSeverityCodes = new HashSet<string>(StringComparer.Ordinal)
        {
            "SENSITIVE_INFO",
            "SUSPICIOUS_URL",
            "REMOTE_ACCESS",
            "QR_PAYMENT_SCAM",
            "JOB_SCAM",
            "BRAND_IMPERSONATION"
        };

        var hasHighSeverity = activeRules.Any(r => highSeverityCodes.Contains(r.Code));

        // If the content is flagged as educational and lacks any high-severity active attacks, dampen score to Low
        if (isEducationalContext && !hasHighSeverity)
        {
            return new RiskScoreResult(0, RiskLevel.Low,
                "The content represents educational security awareness or consumer guidance rather than an active fraudulent solicitation.");
        }

        // Base score from unique triggered rules
        var baseScore = activeRules
            .Where(r => r.Code != "EDUCATIONAL_CONTEXT")
            .Sum(r => r.Score);

        // Contextual Compound Synergies (multi-vector attacks amplify risk)
        var compoundBonus = 0;
        var reasons = new List<string>();

        // Synergy 1: Credential demand + Account suspension/Urgency + Link (Classic Phishing)
        if (activeCodes.Contains("SENSITIVE_INFO") &&
            (activeCodes.Contains("ACCOUNT_SUSPENSION") || activeCodes.Contains("URGENCY")) &&
            activeCodes.Contains("SUSPICIOUS_URL"))
        {
            compoundBonus += 25;
            reasons.Add("Compound phishing attack vector: urgent account threat paired with credential harvesting link.");
        }
        // Synergy 2: Credential demand + Account suspension
        else if (activeCodes.Contains("SENSITIVE_INFO") && activeCodes.Contains("ACCOUNT_SUSPENSION"))
        {
            compoundBonus += 15;
            reasons.Add("Urgent account threat combined with direct credential request.");
        }

        // Synergy 3: QR Code diversion + Reward / Payment
        if (activeCodes.Contains("QR_PAYMENT_SCAM") &&
            (activeCodes.Contains("REWARD_CLAIM") || activeCodes.Contains("PAYMENT_REQUEST")))
        {
            compoundBonus += 20;
            reasons.Add("Deceptive QR payment diversion combined with fraudulent reward or cashback promise.");
        }

        // Synergy 4: Remote access + Account threat / Payment request / Unknown sender
        if (activeCodes.Contains("REMOTE_ACCESS") &&
            (activeCodes.Contains("ACCOUNT_SUSPENSION") || activeCodes.Contains("PAYMENT_REQUEST") || activeCodes.Contains("BRAND_IMPERSONATION") || activeCodes.Contains("UNKNOWN_SENDER")))
        {
            compoundBonus += 25;
            reasons.Add("Social engineering remote desktop takeover requested by an unverified contact or combined with coercive threats.");
        }

        // Synergy 5: Fake Job + Advance Fee / Deposit demand or task scam
        if (activeCodes.Contains("JOB_SCAM"))
        {
            if (activeCodes.Contains("PAYMENT_REQUEST") || activeCodes.Contains("UNKNOWN_SENDER"))
            {
                compoundBonus += 25;
                reasons.Add("Advance-fee employment trap: upfront deposit demanded for promised job or task payout.");
            }
            else
            {
                compoundBonus += 25;
                reasons.Add("High-risk employment scam soliciting upfront fees or fraudulent tasks.");
            }
        }

        // Synergy 6: Lottery / Prize / Reward claim from unknown sender or paired with urgency
        if (activeCodes.Contains("REWARD_CLAIM") &&
            (activeCodes.Contains("UNKNOWN_SENDER") || activeCodes.Contains("URGENCY") || activeCodes.Contains("PAYMENT_REQUEST")))
        {
            compoundBonus += 20;
            reasons.Add("Unsolicited prize or lottery claim from an unverified sender with coercive reward framing.");
        }

        var totalScore = Math.Clamp(baseScore + compoundBonus, 0, 100);

        RiskLevel riskLevel;
        if (totalScore >= CriticalThresholdMin)
        {
            riskLevel = RiskLevel.Critical;
        }
        else if (totalScore >= HighThresholdMin)
        {
            riskLevel = RiskLevel.High;
        }
        else if (totalScore >= SuspiciousThresholdMin)
        {
            riskLevel = RiskLevel.Suspicious;
        }
        else
        {
            riskLevel = RiskLevel.Low;
        }

        var primaryReason = reasons.Count > 0
            ? string.Join(" ", reasons)
            : GetDefaultReasonForLevel(riskLevel, activeRules);

        return new RiskScoreResult(totalScore, riskLevel, primaryReason);
    }

    private static string GetDefaultReasonForLevel(RiskLevel level, IReadOnlyList<FraudRuleResult> rules)
    {
        return level switch
        {
            RiskLevel.Critical => "Critical threat detected: multiple verified deception and credential-harvesting indicators.",
            RiskLevel.High => "High risk: message contains correlated scam indicators commonly associated with digital fraud.",
            RiskLevel.Suspicious => "Medium risk: suspicious indicators detected requiring caution and independent verification.",
            RiskLevel.Low => "Low risk: no high-confidence fraud signals detected in the submitted content.",
            _ => "Assessment inconclusive based on available evidence."
        };
    }
}
