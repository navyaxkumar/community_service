using System.Text.RegularExpressions;
using DigitalShield.API.Fraud.Analyzer;

namespace DigitalShield.API.Fraud.Rules;

public class QrPaymentRule : IFraudRule
{
    private static readonly Regex QrScamPattern = new(
        @"\b(scan|open)\s+.*?\b(qr\s*code|barcode)\s+.*?\b(to\s+(receive|credit|get|claim|accept|deposit)\s+(money|refund|payment|cashback|funds?)|authorize\s+payment)\b|" +
        @"\bto\s+(receive|credit|get|claim|accept|deposit)\s+.*?\b(scan|open)\s+.*?\b(qr\s*code|barcode)\b|" +
        @"\b(enter|type|input|share)\s+.*?\b(upi\s*pin|secret\s*pin|mpin|pin)\s+.*?\b(to\s+(receive|get|credit|claim|accept|deposit)|deposit\s+funds)\b|" +
        @"\b(accept|approve)\s+(the\s+)?(collect\s+request|payment\s+request)\s+.*?\b(to\s+receive|to\s+get\s+paid)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
        TimeSpan.FromMilliseconds(250));

    private static readonly Regex AdvisoryPattern = new(
        @"\b(upi\s+pin\s+is\s+only\s+used\s+for\s+sending|never\s+enter\s+upi\s+pin\s+to\s+receive|never\s+scan\s+qr\s+to\s+receive|scanning\s+a\s+qr\s+code\s+is\s+only\s+for\s+making\s+payments)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
        TimeSpan.FromMilliseconds(250));

    public FraudRuleResult Evaluate(FraudAnalysisInput input)
    {
        if (input.Type != FraudInputType.Message || string.IsNullOrWhiteSpace(input.Content))
        {
            return new FraudRuleResult("QR_PAYMENT_SCAM", "QR or UPI PIN receive scam", 30,
                "No QR or UPI diversion indicators detected.", "Payment", false);
        }

        var text = input.Content;
        if (AdvisoryPattern.IsMatch(text))
        {
            return new FraudRuleResult("QR_PAYMENT_SCAM", "UPI advisory", 0,
                "Educational note explaining that UPI PIN is never required to receive money.", "Payment", false);
        }

        var match = QrScamPattern.Match(text);
        if (!match.Success)
        {
            return new FraudRuleResult("QR_PAYMENT_SCAM", "QR or UPI PIN receive scam", 30,
                "No QR or UPI diversion indicators detected.", "Payment", false);
        }

        return new FraudRuleResult(
            "QR_PAYMENT_SCAM",
            "QR code or UPI PIN payment diversion",
            30,
            "The message claims that scanning a QR code or entering a UPI PIN will receive funds. In digital payment architecture, you NEVER enter a PIN or scan a QR code to receive money; doing so authorizes money to leave your account.",
            "Payment",
            true,
            match.Value,
            "High");
    }
}
