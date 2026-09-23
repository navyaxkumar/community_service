using System.Net;
using System.Text.RegularExpressions;
using DigitalShield.API.Fraud.Analyzer;

namespace DigitalShield.API.Fraud.Rules;

public class SuspiciousUrlRule : IFraudRule
{
    private static readonly Regex UrlPattern = new(
        @"https?://[^\s<>]+",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.NonBacktracking);
    private static readonly HashSet<string> ShortenerHosts = new(StringComparer.OrdinalIgnoreCase)
    {
        "bit.ly", "tinyurl.com", "t.co", "goo.gl", "ow.ly"
    };

    public FraudRuleResult Evaluate(FraudAnalysisInput input)
    {
        var candidate = input.Type == FraudInputType.Url ? input.Url : FindUrl(input.Content);
        var triggered = IsSuspicious(candidate);
        return new FraudRuleResult("SUSPICIOUS_URL", "Suspicious URL", 25,
            "The URL contains characteristics commonly associated with suspicious links. Verify it through an official source.", "Link", triggered);
    }

    private static string? FindUrl(string content)
    {
        var match = UrlPattern.Match(content);
        return match.Success ? match.Value.TrimEnd('.', ',', '!', ')', ']') : null;
    }

    private static bool IsSuspicious(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || !Uri.TryCreate(value, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            return false;
        }

        var host = uri.Host;
        var labels = host.Split('.', StringSplitOptions.RemoveEmptyEntries);
        return uri.Scheme == Uri.UriSchemeHttp ||
            IPAddress.TryParse(host, out _) ||
            labels.Length > 4 ||
            host.Contains("xn--", StringComparison.OrdinalIgnoreCase) ||
            ShortenerHosts.Contains(host) ||
            uri.Port is not (-1 or 80 or 443) ||
            value.Length > 512;
    }
}
