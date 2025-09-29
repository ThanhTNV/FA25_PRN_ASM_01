namespace ASM_01.DataAccessLayer.Entities.VehicleModels;

public class TrimPrice
{
    public int TrimPriceId { get; set; }
    public int EvTrimId { get; set; }
    public decimal ListedPrice { get; set; }
    public DateTime EffectiveDate { get; set; }

    public virtual EvTrim EvTrim { get; set; } = null!;
}