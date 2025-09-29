namespace ASM_01.WebApp.Models;

internal class DistRequestRowViewModel
{
    public int RequestId { get; set; }
    public string DealerName { get; set; } = null!;
    public string ModelName { get; set; } = null!;
    public string TrimName { get; set; } = null!;
    public int ModelYear { get; set; }
    public int RequestQuantity { get; set; }
    public int ApprovedQuantity { get; set; }
    public string Status { get; set; } = null!;
    public DateTime RequestDate { get; set; }
    public DateTime? ApprovalDate { get; set; }
}