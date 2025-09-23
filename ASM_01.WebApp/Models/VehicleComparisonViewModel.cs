using ASM_01.BusinessLayer.DTOs;

namespace ASM_01.WebApp.Models;

public class VehicleComparisonViewModel
{
    // Vehicles selected for comparison
    public List<VehicleComparisonDto> Vehicles { get; set; } = new();

    // Distinct list of specification names (used for table rows)
    public List<string> SpecificationKeys { get; set; } = new();
}
