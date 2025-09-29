using ASM_01.DataAccessLayer.Entities.VehicleModels;

namespace ASM_01.DataAccessLayer.Entities.Warehouse;

public class VehicleStock
{
    public int VehicleStockId { get; set; }
    public int DealerId { get; set; }
    public int EvTrimId { get; set; }
    public int Quantity { get; set; }

    public virtual Dealer Dealer { get; set; } = null!;
    public virtual EvTrim EvTrim { get; set; } = null!;
}
