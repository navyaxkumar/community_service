namespace DigitalShield.API.DTOs.Fraud;

public class FraudIndicatorDto
{
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Score { get; set; }
    public string Explanation { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}
