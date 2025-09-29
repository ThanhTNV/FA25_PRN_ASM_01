using ASM_01.DataAccessLayer.Enums;

namespace ASM_01.BusinessLayer.DTOs;

public class CreateVehicleModelDto
{
    public string ModelName { get; set; } = null!;
    public string? Description { get; set; }
    public EvStatus Status { get; set; } = EvStatus.Unavailable;
}
