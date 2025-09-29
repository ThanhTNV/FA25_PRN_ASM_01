namespace ASM_01.DataAccessLayer.Entities.VehicleModels;

public class TrimSpec
{
    public int EvTrimId { get; set; }
    public int SpecId { get; set; }
    public string Value { get; set; } = null!;

    public virtual EvTrim EvTrim { get; set; } = null!;
    public virtual Spec Spec { get; set; } = null!;
}