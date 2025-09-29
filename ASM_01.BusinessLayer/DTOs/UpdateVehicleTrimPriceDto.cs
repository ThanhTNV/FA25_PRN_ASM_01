namespace ASM_01.BusinessLayer.DTOs;

public class UpdateVehicleTrimPriceDto
{
    public int EvTrimId { get; set; }
    public decimal NewListedPrice { get; set; }
    public DateTime? EffectiveDate { get; set; } // optional; defaults to now
}