using DigitalShield.API.Fraud.Analyzer;

namespace DigitalShield.API.Fraud.Rules;

public class UnknownSenderRule : IFraudRule
{
    public FraudRuleResult Evaluate(FraudAnalysisInput input)
    {
        var triggered = input.Type == FraudInputType.Message && input.SenderKnown == false;
        return new FraudRuleResult("UNKNOWN_SENDER", "Unknown sender", 10,
            "The message is marked as coming from an unknown sender.", "Sender", triggered);
    }
}
