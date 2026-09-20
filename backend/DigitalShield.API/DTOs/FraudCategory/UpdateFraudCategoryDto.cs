namespace DigitalShield.API.DTOs.FraudCategory;

public class UpdateFraudCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}
