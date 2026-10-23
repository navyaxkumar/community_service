using System.Text.RegularExpressions;
using DigitalShield.API.Fraud.Analyzer;

namespace DigitalShield.API.Fraud.Rules;

public class PaymentRequestRule : IFraudRule
{
    private static readonly Regex Pattern = new(
        @"\b(send|transfer|pay|deposit)\s+(money|payment|\d+|rs\.?\s*\d+|fee|charges?|charge|fine)\b|" +
        @"\b(upi\s+payment|bank\s+transfer|payment\s+required|registration\s+fee|redelivery\s+charge|clearance\s+fee)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
        TimeSpan.FromMilliseconds(250));

    public FraudRuleResult Evaluate(FraudAnalysisInput input)
    {
        var triggered = input.Type == FraudInputType.Message && Pattern.IsMatch(input.Content);
        return new FraudRuleResult("PAYMENT_REQUEST", "Payment request", 25,
            "A request to send money or make a payment was detected.", "Payment", triggered);
    }
}
