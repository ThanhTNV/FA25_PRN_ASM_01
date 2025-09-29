namespace ASM_01.WebApp.Models;

public class DealerStockViewModel
{
    public int EvTrimId { get; set; }
    public string TrimName { get; set; } = null!;
    public string ModelName { get; set; } = null!;
    public int? ModelYear { get; set; }
    public int Quantity { get; set; }
}
