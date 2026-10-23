using System.Net;
using System.Text.RegularExpressions;

namespace DigitalShield.API.Fraud.Analyzer;

public sealed record UrlFinding(
    string Code,
    string Title,
    int Score,
    string Explanation,
    string Category,
    string Evidence,
    string Severity);

public static class UrlAnalyzer
{
    private static readonly Regex UrlExtractRegex = new(
        @"https?://[^\s<>""']+",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
        TimeSpan.FromMilliseconds(250));

    private static readonly HashSet<string> ShortenerHosts = new(StringComparer.OrdinalIgnoreCase)
    {
        "bit.ly", "tinyurl.com", "t.co", "goo.gl", "ow.ly", "is.gd", "buff.ly", "cutt.ly", "rb.gy"
    };

    private static readonly string[] KnownMultiPartTlds =
    [
        ".co.uk", ".org.uk", ".gov.uk", ".ac.uk",
        ".co.in", ".net.in", ".gov.in", ".ac.in", ".res.in",
        ".com.au", ".net.au", ".edu.au", ".gov.au",
        ".co.nz", ".co.jp", ".com.sg", ".co.za", ".com.br"
    ];

    private static readonly Dictionary<string, HashSet<string>> OfficialBrandDomains = new(StringComparer.OrdinalIgnoreCase)
    {
        ["sbi"] = new(StringComparer.OrdinalIgnoreCase) { "sbi.co.in", "onlinesbi.sbi", "statebankofindia.com" },
        ["statebank"] = new(StringComparer.OrdinalIgnoreCase) { "sbi.co.in", "onlinesbi.sbi", "statebankofindia.com" },
        ["hdfc"] = new(StringComparer.OrdinalIgnoreCase) { "hdfcbank.com", "hdfc.com" },
        ["icici"] = new(StringComparer.OrdinalIgnoreCase) { "icicibank.com" },
        ["axis"] = new(StringComparer.OrdinalIgnoreCase) { "axisbank.com" },
        ["paytm"] = new(StringComparer.OrdinalIgnoreCase) { "paytm.com" },
        ["phonepe"] = new(StringComparer.OrdinalIgnoreCase) { "phonepe.com" },
        ["paypal"] = new(StringComparer.OrdinalIgnoreCase) { "paypal.com", "paypal.me" },
        ["google"] = new(StringComparer.OrdinalIgnoreCase) { "google.com", "google.co.in", "google.co.uk" },
        ["microsoft"] = new(StringComparer.OrdinalIgnoreCase) { "microsoft.com", "live.com", "office.com" },
        ["apple"] = new(StringComparer.OrdinalIgnoreCase) { "apple.com", "icloud.com" },
        ["amazon"] = new(StringComparer.OrdinalIgnoreCase) { "amazon.com", "amazon.in", "amazon.co.uk" },
        ["netflix"] = new(StringComparer.OrdinalIgnoreCase) { "netflix.com" },
        ["whatsapp"] = new(StringComparer.OrdinalIgnoreCase) { "whatsapp.com" },
        ["telegram"] = new(StringComparer.OrdinalIgnoreCase) { "telegram.org", "t.me" },
        ["indiapost"] = new(StringComparer.OrdinalIgnoreCase) { "indiapost.gov.in" },
        ["fedex"] = new(StringComparer.OrdinalIgnoreCase) { "fedex.com" },
        ["dhl"] = new(StringComparer.OrdinalIgnoreCase) { "dhl.com" }
    };

    private static readonly HashSet<string> PhishingKeywordTokens = new(StringComparer.OrdinalIgnoreCase)
    {
        "kyc", "verify", "verification", "login", "signin", "update", "secure", "security",
        "account", "portal", "support", "billing", "bill", "gift", "reward", "track", "tracking",
        "pay", "alert", "banking", "service", "claim", "pan", "aadhar", "otp"
    };

    public static string? ExtractFirstUrl(string content)
    {
        if (string.IsNullOrWhiteSpace(content)) return null;
        var match = UrlExtractRegex.Match(content);
        return match.Success ? match.Value.TrimEnd('.', ',', '!', ')', ']', ';') : null;
    }

    public static List<UrlFinding> Analyze(string rawUrl)
    {
        var findings = new List<UrlFinding>();
        if (string.IsNullOrWhiteSpace(rawUrl)) return findings;

        var urlToParse = rawUrl.Trim();
        if (!urlToParse.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !urlToParse.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            urlToParse = "https://" + urlToParse;
        }

        if (!Uri.TryCreate(urlToParse, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            return findings;
        }

        var host = uri.Host;
        if (string.IsNullOrWhiteSpace(host)) return findings;

        // 1. Embedded Credentials in URL Authority (e.g., https://google.com@phishing-target.com)
        if (!string.IsNullOrWhiteSpace(uri.UserInfo))
        {
            findings.Add(new UrlFinding(
                "EMBEDDED_CREDENTIALS",
                "Misleading URL authority and embedded credentials",
                35,
                $"The URL authority contains user information ('{uri.UserInfo}') designed to mislead users into believing the destination is different from the actual host '{host}'.",
                "Link",
                $"{uri.UserInfo}@{host}",
                "High"));
        }

        // 2. IP Address Host (e.g., http://192.168.1.10/login)
        if (IPAddress.TryParse(host, out _))
        {
            findings.Add(new UrlFinding(
                "IP_ADDRESS_HOST",
                "Numerical IP address host",
                30,
                "The link connects directly to a numerical IP address rather than a registered domain name, a pattern commonly used in transient phishing and malware hosts.",
                "Link",
                host,
                "High"));
        }

        // 3. Punycode / Internationalized Domain Name (Homograph lookalikes)
        if (host.StartsWith("xn--", StringComparison.OrdinalIgnoreCase) ||
            host.Contains(".xn--", StringComparison.OrdinalIgnoreCase))
        {
            findings.Add(new UrlFinding(
                "PUNYCODE_DOMAIN",
                "Punycode encoded domain (Homograph risk)",
                30,
                "The domain utilizes internationalized punycode encoding ('xn--'), which can visually emulate legitimate brand characters using foreign scripts.",
                "Link",
                host,
                "Medium"));
        }

        // 4. Link Shortener
        if (ShortenerHosts.Contains(host))
        {
            findings.Add(new UrlFinding(
                "SHORTENED_URL",
                "Shortened link with obscured destination",
                15,
                "The URL uses a link shortening service that masks the real destination. Proceed with caution and verify the source.",
                "Link",
                host,
                "Medium"));
        }

        // 5. Unusual Non-Standard Port
        if (uri.Port > 0 && uri.Port != 80 && uri.Port != 443)
        {
            findings.Add(new UrlFinding(
                "UNUSUAL_PORT",
                "Non-standard web port",
                15,
                $"The link connects on non-standard port {uri.Port}, which is uncommon for legitimate consumer web services.",
                "Link",
                $"{host}:{uri.Port}",
                "Medium"));
        }

        // 6. Plain HTTP Transport
        if (uri.Scheme == Uri.UriSchemeHttp)
        {
            findings.Add(new UrlFinding(
                "INSECURE_HTTP",
                "Unencrypted HTTP communication",
                15,
                "The URL uses unencrypted HTTP instead of secure HTTPS, exposing transmitted data to interception.",
                "Link",
                uri.Scheme,
                "Medium"));
        }

        // 7. Registrable Domain & Brand Lookalike / Typo-Squatting Analysis
        var registrableDomain = GetRegistrableDomain(host);
        CheckBrandLookalike(registrableDomain, host, findings);

        // 8. Excessive Subdomain Hierarchy (relative to registrable root domain)
        var subdomainPrefix = host.Length > registrableDomain.Length
            ? host[..^(registrableDomain.Length + 1)]
            : string.Empty;
        var subdomainLevels = string.IsNullOrEmpty(subdomainPrefix)
            ? 0
            : subdomainPrefix.Split('.', StringSplitOptions.RemoveEmptyEntries).Length;

        if (subdomainLevels >= 4)
        {
            findings.Add(new UrlFinding(
                "EXCESSIVE_SUBDOMAINS",
                "Excessive subdomain chain",
                30,
                $"The host has {subdomainLevels} subdomain levels above '{registrableDomain}'. Deeply nested subdomain chains are frequently used in phishing redirects to conceal the real root domain.",
                "Link",
                host,
                "Medium"));
        }

        return findings;
    }

    public static string GetRegistrableDomain(string host)
    {
        if (string.IsNullOrWhiteSpace(host)) return string.Empty;
        var cleanHost = host.Trim().TrimEnd('.');
        if (IPAddress.TryParse(cleanHost, out _)) return cleanHost;

        foreach (var suffix in KnownMultiPartTlds)
        {
            if (cleanHost.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            {
                var withoutSuffix = cleanHost[..^suffix.Length];
                var parts = withoutSuffix.Split('.', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0)
                {
                    return $"{parts[^1]}{suffix}";
                }
                return cleanHost;
            }
        }

        var labels = cleanHost.Split('.', StringSplitOptions.RemoveEmptyEntries);
        if (labels.Length >= 2)
        {
            return $"{labels[^2]}.{labels[^1]}";
        }

        return cleanHost;
    }

    private static void CheckBrandLookalike(string registrableDomain, string fullHost, List<UrlFinding> findings)
    {
        if (string.IsNullOrWhiteSpace(registrableDomain)) return;
        var domainLower = registrableDomain.ToLowerInvariant();
        var domainNamePart = domainLower.Split('.')[0];

        foreach (var (brandKey, officialDomains) in OfficialBrandDomains)
        {
            // If the registrable domain is an official domain of the brand, it is legitimate
            if (officialDomains.Contains(registrableDomain))
            {
                continue;
            }

            // A. Brand name present in domain with phishing tokens (e.g. sbi-kyc-update.com, netflix-billing.net)
            if (domainNamePart.Contains(brandKey, StringComparison.OrdinalIgnoreCase))
            {
                var tokens = domainNamePart.Split(new[] { '-', '_', '.' }, StringSplitOptions.RemoveEmptyEntries);
                var hasPhishingToken = tokens.Any(t => PhishingKeywordTokens.Contains(t));

                if (hasPhishingToken || !officialDomains.Contains(registrableDomain))
                {
                    findings.Add(new UrlFinding(
                        "BRAND_LOOKALIKE",
                        $"Suspicious brand lookalike domain ({brandKey.ToUpperInvariant()})",
                        35,
                        $"The domain '{registrableDomain}' mimics the brand '{brandKey.ToUpperInvariant()}' but is not an authorized official domain of the organization.",
                        "Link",
                        fullHost,
                        "High"));
                    return; // Avoid multiple brand flags on single domain
                }
            }

            // B. Typo-squatting substitutions (e.g., paypa1, arnazon, g00gle)
            if (IsTypoSquatting(domainNamePart, brandKey))
            {
                findings.Add(new UrlFinding(
                    "BRAND_LOOKALIKE",
                    $"Typo-squatting lookalike domain ({brandKey.ToUpperInvariant()})",
                    35,
                    $"The domain '{registrableDomain}' contains visual character substitutions (typo-squatting) designed to resemble '{brandKey.ToUpperInvariant()}'.",
                    "Link",
                    fullHost,
                    "High"));
                return;
            }
        }
    }

    private static bool IsTypoSquatting(string domainName, string brand)
    {
        if (brand.Length < 4) return false;

        // Common substitutions: 1 -> l/i, 0 -> o, rn -> m, vv -> w
        var normalized = domainName
            .Replace("1", "l")
            .Replace("0", "o")
            .Replace("rn", "m")
            .Replace("vv", "w");

        if (normalized.Contains(brand, StringComparison.OrdinalIgnoreCase) &&
            !domainName.Contains(brand, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return false;
    }
}
