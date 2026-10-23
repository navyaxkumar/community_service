namespace DigitalShield.API.Fraud.ThreatIntelligence;

public class LocalThreatIntelligenceProvider : IThreatIntelligenceProvider
{
    public string ProviderName => "Local Threat Engine";
    public bool IsConfigured => false;

    public Task<ThreatIntelligenceResult> CheckUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        // Safe, deterministic local intelligence default.
        // Does not issue outbound HTTP requests to user-supplied URLs.
        // Clearly labels that no external database was queried.
        var result = new ThreatIntelligenceResult(
            IsChecked: false,
            IsMalicious: false,
            ThreatType: null,
            CheckedAt: DateTime.UtcNow,
            ProviderName: ProviderName,
            Details: "Evaluated using local heuristic and brand lookalike detection. No external threat intelligence reputation service is currently configured.");

        return Task.FromResult(result);
    }
}
