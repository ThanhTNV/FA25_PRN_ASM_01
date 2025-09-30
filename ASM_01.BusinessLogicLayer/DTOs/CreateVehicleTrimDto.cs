namespace ASM_01.BusinessLayer.DTOs;

public class CreateVehicleTrimDto
{
    public int EvModelId { get; set; }           // FK to existing EV Model
    public string TrimName { get; set; } = null!;
    public int? ModelYear { get; set; }
    public string? Description { get; set; }
    public decimal? ListedPrice { get; set; }    // optional initial price
}