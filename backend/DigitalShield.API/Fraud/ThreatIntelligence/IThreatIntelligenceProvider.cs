namespace DigitalShield.API.Fraud.ThreatIntelligence;

public interface IThreatIntelligenceProvider
{
    string ProviderName { get; }
    bool IsConfigured { get; }
    Task<ThreatIntelligenceResult> CheckUrlAsync(string url, CancellationToken cancellationToken = default);
}
