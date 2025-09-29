using ASM_01.DataAccessLayer.Enums;

namespace ASM_01.WebApp.Models;

public class EditVehicleModelViewModel
{
    public int EvModelId { get; set; }
    public string ModelName { get; set; } = null!;
    public string? Description { get; set; }
    public EvStatus Status { get; set; }
}
