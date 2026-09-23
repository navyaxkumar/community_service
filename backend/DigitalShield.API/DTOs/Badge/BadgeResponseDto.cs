namespace DigitalShield.API.DTOs.Badge;

public class BadgeResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public int RequiredPoints { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
