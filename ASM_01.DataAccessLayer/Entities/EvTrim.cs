namespace ASM_01.DataAccessLayer.Entities;

public class EvTrim
{
    public int EvTrimId { get; set; }
    public int EvModelId { get; set; }
    public string TrimName { get; set; } = null!;
    public int? ModelYear { get; set; }
    public string? Description { get; set; }

    public virtual EvModel EvModel { get; set; } = null!;
    public virtual ICollection<TrimPrice> Prices { get; set; } = new List<TrimPrice>();
    public virtual ICollection<TrimSpec> TrimSpecs { get; set; } = new List<TrimSpec>();
}