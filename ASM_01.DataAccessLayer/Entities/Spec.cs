using ASM_01.DataAccessLayer.Enums;

namespace ASM_01.DataAccessLayer.Entities;

public class Spec
{
    public int SpecId { get; set; }
    public string SpecName { get; set; } = null!;
    public string? Unit { get; set; }
    public SpecCategory? Category { get; set; }

    public virtual ICollection<TrimSpec> TrimSpecs { get; set; } = new List<TrimSpec>();
}