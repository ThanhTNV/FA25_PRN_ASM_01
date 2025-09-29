using ASM_01.DataAccessLayer.Entities.Warehouse;

namespace ASM_01.DataAccessLayer.Entities.VehicleModels;

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
    public ICollection<VehicleStock> VehicleStocks { get; set; } = new List<VehicleStock>();
    public ICollection<DistributionRequest> DistributionRequests { get; set; } = new List<DistributionRequest>();
}