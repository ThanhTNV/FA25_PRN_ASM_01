namespace ASM_01.BusinessLayer.DTOs;

public class DistributionRequestDto
{
    public int RequestId { get; set; }
    public int DealerId { get; set; }
    public string DealerName { get; set; } = null!;
    public int EvTrimId { get; set; }
    public string TrimName { get; set; } = null!;
    public string ModelName { get; set; } = null!;
    public int ModelYear { get; set; }
    public int RequestQuantity { get; set; }
    public int ApprovedQuantity { get; set; }
    public DateTime RequestDate { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string Status { get; set; } = null!;
}
