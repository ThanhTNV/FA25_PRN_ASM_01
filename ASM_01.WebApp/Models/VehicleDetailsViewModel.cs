namespace ASM_01.WebApp.Models;

public class VehicleDetailsViewModel
{
    public int VehicleId { get; set; }
    public string ModelName { get; set; } = string.Empty;
    public string TrimName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime EffectiveDate { get; set; }
    public string? Description { get; set; }
    public Dictionary<string, string> Specifications { get; set; } = new();
}