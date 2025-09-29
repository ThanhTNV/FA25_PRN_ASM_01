using ASM_01.DataAccessLayer.Enums;

namespace ASM_01.BusinessLayer.DTOs;

public class UpdateVehicleModelStatusDto
{
    public int EvModelId { get; set; }
    public EvStatus Status { get; set; }
}
