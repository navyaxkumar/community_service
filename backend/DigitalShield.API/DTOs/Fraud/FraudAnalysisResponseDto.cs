namespace DigitalShield.API.DTOs.Fraud;

public class FraudAnalysisResponseDto
{
    public int Score { get; set; }
    public string RiskLevel { get; set; } = string.Empty;
    public string? RiskReason { get; set; }
    public List<FraudIndicatorDto> Indicators { get; set; } = new();
    public List<RecommendationDto> Recommendations { get; set; } = new();
    public bool ExternalReputationCheckPerformed { get; set; }
    public string? ThreatIntelligenceSource { get; set; }
    public string Disclaimer { get; set; } = string.Empty;
}
