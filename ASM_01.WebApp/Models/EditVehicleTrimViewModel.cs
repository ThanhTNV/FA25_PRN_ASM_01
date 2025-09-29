namespace ASM_01.WebApp.Models;

public class EditVehicleTrimViewModel
{
    public int EvTrimId { get; set; }
    public int EvModelId { get; set; }
    public string TrimName { get; set; } = null!;
    public int? ModelYear { get; set; }
    public string? Description { get; set; }

    // Optional: allow adding a new price entry during edit
    public decimal? NewListedPrice { get; set; }
}
