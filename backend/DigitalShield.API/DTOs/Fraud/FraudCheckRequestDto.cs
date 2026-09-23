namespace DigitalShield.API.DTOs.Fraud;

public class FraudCheckRequestDto
{
    public string Type { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string? Url { get; set; }
    public bool? SenderKnown { get; set; }
}
