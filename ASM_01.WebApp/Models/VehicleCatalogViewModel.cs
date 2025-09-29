namespace ASM_01.WebApp.Models;

public class VehicleCatalogViewModel
{
    public int VehicleId { get; set; }
    public int ModelId { get; set; }
    public string ModelName { get; set; } = string.Empty;
    public string TrimName { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

