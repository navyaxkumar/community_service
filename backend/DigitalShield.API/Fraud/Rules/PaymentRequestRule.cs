using System.Text.RegularExpressions;
using DigitalShield.API.Fraud.Analyzer;

namespace DigitalShield.API.Fraud.Rules;

public class PaymentRequestRule : IFraudRule
{
    private static readonly Regex Pattern = new(
        @"\b(send|transfer|pay)\s+(money|payment)|\bupi\s+payment\b|\bbank\s+transfer\b|\bpayment\s+required\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.NonBacktracking);

    public FraudRuleResult Evaluate(FraudAnalysisInput input)
    {
        var triggered = input.Type == FraudInputType.Message && Pattern.IsMatch(input.Content);
        return new FraudRuleResult("PAYMENT_REQUEST", "Payment request", 25,
            "A request to send money or make a payment was detected.", "Payment", triggered);
    }
}
