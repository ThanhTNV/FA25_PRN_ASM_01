using ASM_01.DataAccessLayer.Entities.VehicleModels;
using ASM_01.DataAccessLayer.Enums;

namespace ASM_01.DataAccessLayer.Entities.Warehouse;

public class DistributionRequest
{
    public int DistributionRequestId { get; set; }
    public int DealerId { get; set; }
    public int EvTrimId { get; set; }
    public int RequestedQuantity { get; set; }
    public int? ApprovedQuantity { get; set; }
    public DistributionStatus Status { get; set; } = DistributionStatus.Pending;
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ApprovedAt { get; set; }

    public virtual Dealer Dealer { get; set; } = null!;
    public virtual EvTrim EvTrim { get; set; } = null!;
}