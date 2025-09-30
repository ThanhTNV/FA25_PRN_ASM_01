namespace ASM_01.BusinessLayer.DTOs;

public class VehicleDto
{
    public int VehicleId { get; set; }
    public string ModelName { get; set; } = string.Empty;
    public string TrimName { get; set; } = string.Empty;
    public int TrimId { get; set; }
    public decimal Price { get; set; }
    public DateTime EffectiveDate { get; set; }
    public Dictionary<string, string> Specifications { get; set; } = new();
    public int? ModelYear { get; internal set; }
    public int ModelId { get; internal set; }
    public string? Description { get; internal set; }
    public string Status { get; internal set; }
}
